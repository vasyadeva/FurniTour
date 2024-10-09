import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterprofileComponent } from './masterprofile.component';

describe('MasterprofileComponent', () => {
  let component: MasterprofileComponent;
  let fixture: ComponentFixture<MasterprofileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterprofileComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MasterprofileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
