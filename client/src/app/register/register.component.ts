import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/accounts.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  
  //Input from parent to child component
  //usersFromHomeComponent = input.required<any>();
  //@Input usersFromHomeComponent;  version < 17.3
  
  
  //Output from child to parent component
  cancelRegister = output<boolean>();
  // @Output cancelRegister = new EventEmitter<boolean>(); version < 17.3
  
  
  
  model: any = {};

  register(){
    this.accountService.register(this.model).subscribe({
      next: response => {
        console.log(response);
        this.cancel();
      },
      error: error => console.error(error)
    })
  }
  
  cancel(){
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }
}
