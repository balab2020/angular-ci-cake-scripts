import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeoowJestComponent } from './heoow-jest.component';

describe('HeoowJestComponent', () => {
  let component: HeoowJestComponent;
  let fixture: ComponentFixture<HeoowJestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeoowJestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeoowJestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
