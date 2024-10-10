import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerProfileComponent } from './manufacturer-profile.component';

describe('ManufacturerProfileComponent', () => {
  let component: ManufacturerProfileComponent;
  let fixture: ComponentFixture<ManufacturerProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManufacturerProfileComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManufacturerProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
