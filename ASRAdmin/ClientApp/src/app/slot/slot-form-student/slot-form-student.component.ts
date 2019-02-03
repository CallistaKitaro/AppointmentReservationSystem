import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
    selector: 'slot-form-student',
    templateUrl: './slot-form-student.component.html',
    styleUrls: ['./slot-form-student.component.css']
})
/** slot-form-student component*/
export class SlotFormStudentComponent {
  form;
  id: string;

  constructor(private _router: Router) {
    this.form = new FormGroup({
      studentID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(s|S)\\d{7}$')]))
    });
  }

  getSlots() {

    if (!this.form.valid) {
      return;
    }
    // return to view student slots
    this.id = this.form.get('studentID').value;
    this._router.navigate(["/view-slot/", this.id]);
  }

}
