import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoManufacturerComponent } from './info-manufacturer.component';

describe('InfoManufacturerComponent', () => {
  let component: InfoManufacturerComponent;
  let fixture: ComponentFixture<InfoManufacturerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfoManufacturerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfoManufacturerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
