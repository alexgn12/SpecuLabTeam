import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
//import { environment } from 'src/environments/environment';

export interface Transaction {
  transactionId: number;
  transactionDate: string;   // ISO string
  amount: number;
  type: 'INGRESO' | 'GASTO';
  description: string;
  requestId?: number;
  apartmentId?: number;
}

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private baseUrl = `https://localhost:7092/api/transactions`;

  constructor(private http: HttpClient) {}

  /**
   * Obtiene transacciones desde la API con filtros opcionales.
   * @param opts filtros/paginación
   */
  getTransactions(opts?: {
    transactionType?: 'INGRESO' | 'GASTO';
    page?: number;
    size?: number;
    month?: string;
    year?: string;
  }): Observable<Transaction[]> {
    let params = new HttpParams();

    if (opts?.transactionType) params = params.set('transactionType', opts.transactionType);
    if (opts?.page) params = params.set('page', String(opts.page));
    if (opts?.size) params = params.set('size', String(opts.size));
    if (opts?.month) params = params.set('month', opts.month);
    if (opts?.year) params = params.set('year', opts.year);

    // Ajusta el tipo genérico al DTO que devuelva tu API.
    // Aquí supongo un DTO cercano a tu entity (PascalCase o camelCase).
    return this.http.get<any[]>(this.baseUrl, { params }).pipe(
      map(rows => rows.map(dto => this.mapDtoToTransaction(dto)))
    );
  }

  /**
   * Mapea el DTO de la API a nuestra interfaz de UI.
   * Soporta PascalCase o camelCase y varios esquemas de 'type'.
   */
  private mapDtoToTransaction(dto: any): Transaction {
    // Toma valores en camelCase o PascalCase
    const transactionId   = dto.transactionId ?? dto.TransactionId;
    const transactionDate = dto.transactionDate ?? dto.TransactionDate;
    const amount          = dto.amount ?? dto.Amount;
    const description     = dto.description ?? dto.Description;
    const requestId       = dto.requestId ?? dto.RequestId;
    const apartmentId     = dto.apartmentId ?? dto.ApartmentId;

    // El backend tiene TransactionTypeId + TransactionType (entidad).
    // Intentamos derivar un string 'INGRESO'/'GASTO' de forma robusta.
    const rawType =
      dto.type ??
      dto.transactionType ??
      dto.TransactionType ??
      dto.transactionsType?.name ??
      dto.TransactionsType?.Name ??
      dto.transactionTypeId ??
      dto.TransactionTypeId;

    const type = this.normalizeType(rawType);

    return {
      transactionId,
      transactionDate, // deja ISO string
      amount,
      type,
      description,
      requestId,
      apartmentId
    };
  }

  /** Normaliza cualquier variante a 'INGRESO' | 'GASTO' */
  private normalizeType(raw: any): 'INGRESO' | 'GASTO' {
    if (raw == null) return 'GASTO';
    if (typeof raw === 'number') {
      // Si tu API usa ids: 1 = INGRESO, 2 = GASTO (ajusta si es distinto)
      return raw === 1 ? 'INGRESO' : 'GASTO';
    }
    const s = String(raw).trim().toUpperCase();
    if (s.includes('INGRES')) return 'INGRESO';
    if (s.includes('GAST')) return 'GASTO';
    return s === '1' ? 'INGRESO' : 'GASTO';
  }
}
