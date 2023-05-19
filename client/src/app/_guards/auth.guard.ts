import { Call } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';

@Injectable({
  providedIn:'root'
})

export class AuthGuard implements CanActivate{

  constructor(private accountService: AccountService, private toastr:ToastrService){

  }
  canActivate():Observable<boolean> {
   return this.accountService.currentUser$.pipe(
    map(user => {
      if(user) return true;
      else{
        this.toastr.error('You not logged in');
        return false;
      }
    })
   )
  }

}