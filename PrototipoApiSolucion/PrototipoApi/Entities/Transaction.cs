using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrototipoApi.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        // Relaciones
        public int? RequestId { get; set; } // Ahora nullable
        [ForeignKey("RequestId")]
        public Request? Request { get; set; } // Ahora nullable
        public DateTime TransactionDate { get; set; }
        public double Amount { get; set; }

        // Tipo de transacción.
        // Valores posibles: "INGRESO", "GASTO".
        public int TransactionTypeId { get; set; }
        [ForeignKey("TransactionTypeId")]
        public TransactionType TransactionsType { get; set; } = null!;
        public string Description { get; set; } = string.Empty;

        // Relación con Apartment (opcional)
        public int? ApartmentId { get; set; }
        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        // Relación inversa: una transacción puede estar asociada a muchos presupuestos de gestión
        public ICollection<ManagementBudget> ManagementBudgets { get; set; } = new List<ManagementBudget>();
    }
}
