import { CanActivateFn, UrlTree } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ActivatedRouteSnapshot } from '@angular/router';
import {ToastrService } from 'ngx-toastr';
import { Observable, map } from 'rxjs';

export const AdminGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot  ): 
  Observable<boolean | UrlTree> | 
  Promise<boolean | UrlTree> | boolean | UrlTree => {
   const accountService:AccountService = inject(AccountService);
   const toaster: ToastrService = inject(ToastrService);
    return accountService.currentUser$.pipe(
      map(user => {
        if(!user) return false;
        if (user.roles.includes('Admin') || user.roles.includes('Moderator')){
          return true;
        }else{
          toaster.error('You cannot enter this area');
          return false;
        }
      })
    )
  };


