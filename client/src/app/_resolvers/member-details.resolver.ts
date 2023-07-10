import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';

export const MemberDetailsResolver: ResolveFn<Member> = 
( route: ActivatedRouteSnapshot, 
  state:RouterStateSnapshot,
   memberService: MembersService = inject(MembersService)): 
   Observable<Member> => { 
    return  memberService.getMember(route.paramMap.get('username')!)
    }
