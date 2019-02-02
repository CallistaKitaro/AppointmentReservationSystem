import { Component } from '@angular/core';
import { SlotService } from '../../services/slot.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Slot } from '../../Models/slot';
import { Router } from "@angular/router";
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'slot-form',
    templateUrl: './slot-form.component.html',
    styleUrls: ['./slot-form.component.css']
})
/** slot-form component*/
export class SlotFormComponent {
  form;
  id: string;

  constructor(private _router: Router)
  {
    this.form = new FormGroup({
      staffID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(e|E)\\d{5}$')])),
    });
  }

  getSlots() {

    if (!this.form.valid) {
      return;
    }
    
    this.id = this.form.get('staffID').value;
    this._router.navigate(["/view-slot/", this.id]);
    
  }

  cancel() {
    this._router.navigate(["/slot-staff"]);
  }

}
