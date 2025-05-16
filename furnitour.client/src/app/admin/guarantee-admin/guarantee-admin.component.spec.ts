import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuaranteeAdminComponent } from './guarantee-admin.component';

describe('GuaranteeAdminComponent', () => {
  let component: GuaranteeAdminComponent;
  let fixture: ComponentFixture<GuaranteeAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuaranteeAdminComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuaranteeAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
