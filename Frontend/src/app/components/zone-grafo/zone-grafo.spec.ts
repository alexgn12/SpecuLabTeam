import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ZoneGrafo } from './zone-grafo';

describe('ZoneGrafo', () => {
  let component: ZoneGrafo;
  let fixture: ComponentFixture<ZoneGrafo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ZoneGrafo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ZoneGrafo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
