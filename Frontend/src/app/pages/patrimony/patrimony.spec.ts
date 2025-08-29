import { TestBed, ComponentFixture } from '@angular/core/testing';
import { PatrimonyComponent } from './patrimony';
import { PatrimonyService } from './patrimony.service';
import { of } from 'rxjs';

describe('PatrimonyComponent', () => {
  let fixture: ComponentFixture<PatrimonyComponent>;
  let component: PatrimonyComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatrimonyComponent],
      providers: [
        {
          provide: PatrimonyService,
          useValue: {
            getPatrimony: () => of({ approvedBuildings: [], incomeApartments: [] })
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatrimonyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
