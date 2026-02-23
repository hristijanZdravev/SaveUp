import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { logoutRouteGuard } from './logout-route.guard';

describe('logoutRouteGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => logoutRouteGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
