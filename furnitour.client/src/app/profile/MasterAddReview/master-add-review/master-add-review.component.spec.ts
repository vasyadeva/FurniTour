import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterAddReviewComponent } from './master-add-review.component';

describe('MasterAddReviewComponent', () => {
  let component: MasterAddReviewComponent;
  let fixture: ComponentFixture<MasterAddReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterAddReviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MasterAddReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
