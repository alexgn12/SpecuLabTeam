import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalleRequest } from './detalle-request';

describe('DetalleRequest', () => {
  let component: DetalleRequest;
  let fixture: ComponentFixture<DetalleRequest>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetalleRequest]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetalleRequest);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
