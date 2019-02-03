import { Component, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
    selector: 'slot-form',
    templateUrl: './slot-form.component.html',
    styleUrls: ['./slot-form.component.css']
})
/** slot-form component*/
export class SlotFormComponent {
  form;
  id: string;
  options = ["All", "Booked"];
  option: string;

  constructor(private _router: Router)
  {
    this.form = new FormGroup({
      staffID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(e|E)\\d{5}$')])),
      view: new FormControl('', Validators.required)
    });
  }

  getSlots() {

    if (!this.form.valid) {
      return;
    }

    //Check the option view for slots
    if (this.form.get('view').value == null) {
      this.option = "All";
    } else {
      this.option = this.form.get('view').value;
    }

    this.id = this.form.get('staffID').value;
    this._router.navigate(["/view-slot/", this.id+this.option]);
    
  }

  cancel() {
    this._router.navigate(["/slot-staff"]);
  }

}
