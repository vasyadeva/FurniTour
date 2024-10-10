import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerAddReviewComponent } from './manufacturer-add-review.component';

describe('ManufacturerAddReviewComponent', () => {
  let component: ManufacturerAddReviewComponent;
  let fixture: ComponentFixture<ManufacturerAddReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManufacturerAddReviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManufacturerAddReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
