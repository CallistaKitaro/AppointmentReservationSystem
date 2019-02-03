import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SlotService } from '../../services/slot.service';
import { Slot } from '../../Models/slot';

@Component({
    selector: 'edit-slot',
    templateUrl: './edit-slot.component.html',
    styleUrls: ['./edit-slot.component.css']
})
/** edit-slot component*/
export class EditSlotComponent {
  form;
  title: string = "";
  errorMessage: any;
  id: string;
  page: string;
  roomId: string;
  startTime: string;
  slotTime: string;

  constructor(private _slotService: SlotService, private _avRoute: ActivatedRoute, private _router: Router) {

    if (this._avRoute.snapshot.paramMap.get("id")) {
      this.id = this._avRoute.snapshot.paramMap.get("id");
    }

    // Set the form
    this.form = new FormGroup({
      roomID: new FormControl(''),
      room: new FormControl(''),
      startTime: new FormControl(''),
      staffID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(e|E)\\d{5}$')])),
      staff: new FormControl(''),
      studentID: new FormControl('', Validators.pattern('^(e|E)\\d{5}$')),
      student: new FormControl(''),
    });
  }

  ngOnInit() {

    if (this.id != null) {

      this.page = this.id.substring(0, 1);
      this.roomId = this.id.substring(1,2);
      this.startTime = this.id.substring(2);

      var dateSlot = new Date(this.startTime).toLocaleDateString();
      var timeSlot = (new Date(this.startTime).toLocaleTimeString()).substring(0, 5);
      this.slotTime = dateSlot + " " + timeSlot;

      this._slotService.getSlotDetail(this.roomId, this.slotTime).subscribe(resp => this.form.setValue(resp),
        error => this.errorMessage = error);

    }
  }

  update() {
    
    // save the new data and return to each staff page
    if (this.page == "e") {
      var staffId = this.form.get('staffID').value + 'All';

      let updatedSlot: Slot = new Slot();
      updatedSlot.roomID = this.form.get('roomID').value;
      updatedSlot.startTime = this.form.get('startTime').value;
      updatedSlot.staffID = this.form.get('staffID').value;
      if (this.form.get('studentID').value == null) {
        updatedSlot.studentID = null;
      } else {
        updatedSlot.studentID = this.form.get('studentID').value;
      }

      console.log(this.roomId);
      console.log(this.slotTime);
      console.log(updatedSlot);

      this._slotService.updateSlot(this.roomId, this.slotTime, updatedSlot).subscribe((data) => {
        this._router.navigate(["/view-slot", staffId]);
      }, error => this.errorMessage = error);
    }

     // save the new data and return to each student page
    else if (this.page == "s") {
      var studentId = this.form.get('studentID').value;

      let updatedSlot: Slot = new Slot();
      updatedSlot.roomID = this.form.get('roomID').value;
      updatedSlot.startTime = this.form.get('startTime').value;
      updatedSlot.staffID = this.form.get('staffID').value;

      this._slotService.updateSlot(this.roomId, this.slotTime, updatedSlot).subscribe((data) => {
        this._router.navigate(["/view-slot", studentId]);
      }, error => this.errorMessage = error)
    }
  }


  return() {

    if (this.page == "e") {
      var staffId = this.form.get('staffID').value + 'All';
      this._router.navigate(["/view-slot", staffId]);
    }
    else if (this.page == "s") {
      var studentId = this.form.get('studentID').value;
      this._router.navigate(["/view-slot", studentId]);
    }
  
  }

}
