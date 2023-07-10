import { CanActivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';

export const PreventUnsavedChangesGuard: CanActivateFn = 
(route,state) => {
    const component : MemberEditComponent = inject(MemberEditComponent);
    if(component.editForm?.dirty){
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
  return true;
};
