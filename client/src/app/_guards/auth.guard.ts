import { ActivatedRouteSnapshot, CanActivateFn, UrlTree } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { RouterStateSnapshot } from '@angular/router';
import {ToastrService } from 'ngx-toastr';
import { Observable, map } from 'rxjs';

export const AuthGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot): 
  Observable<boolean | UrlTree> | 
  Promise<boolean | UrlTree> | boolean | UrlTree => {
   const accountService: AccountService = inject(AccountService);
   const toaster: ToastrService = inject(ToastrService);
   return accountService.currentUser$.pipe(
      map(user => {
        if(user) return true;
        else{
          toaster.error('You not logged in');
          return false;
        }
      })
    )
}

