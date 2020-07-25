import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { HeoowJestComponent } from './heoow-jest/heoow-jest.component';
describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent, HeoowJestComponent
      ],
    }).compileComponents();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(fixture.nativeElement).toBeTruthy();
  });

  it('should match snapshot', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    //@ts-ignore
    expect(fixture.nativeElement).toMatchSnapshot();
  });

  it(`should have as title 'angular-hello'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('angular-hello');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.content span').textContent).toContain('angular-hello app is running!');
  });
});
