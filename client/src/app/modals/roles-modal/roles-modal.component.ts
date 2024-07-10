import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  standalone: true,
  imports: [],
  templateUrl: './roles-modal.component.html',
  styleUrl: './roles-modal.component.css',
})
export class RolesModalComponent {
  bsModalRef = inject(BsModalRef);
  title = '';
  availableRoles: string[] = [];
  selectedRoles: string[] = [];
  username: string = '';
  rolesUpdated: boolean = false;

  updateChecked(checkedVallue: string) {
    if (this.selectedRoles.includes(checkedVallue)) {
      this.selectedRoles = this.selectedRoles.filter(
        (r) => r !== checkedVallue
      );
    } else {
      this.selectedRoles.push(checkedVallue);
    }
  }

  onSelectRoles(): void {
    this.rolesUpdated = true;
    this.bsModalRef.hide();
  }
}
