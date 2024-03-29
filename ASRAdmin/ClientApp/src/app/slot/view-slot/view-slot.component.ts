import { Component, OnInit } from '@angular/core';
import { SlotService } from '../../services/slot.service';
import { Slot } from '../../Models/slot';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
    selector: 'view-slot',
    templateUrl: './view-slot.component.html',
    styleUrls: ['./view-slot.component.css']
})
/** view-slot component*/
export class ViewSlotComponent implements OnInit {
  title: string = ""; 
  page: string = ""; // to navigate with page the edit
  slotList: Slot[];
  id: string;
  option: string;

  constructor(private _slotService: SlotService, private _router: Router, private _avRoute: ActivatedRoute)
  {
    this.title = "";
    if (this._avRoute.snapshot.paramMap.get("id"))
    {
      this.id = this._avRoute.snapshot.paramMap.get("id");
    }
  }

  ngOnInit()
  {
    if (this.id != null)
    {
      if (this.id.substring(0, 1) === 'e') {
        this.option = this.id.substring(6);
        this.title = "Staff's Slots";
        this.page = "e";
        if (this.option == "All") {
          this.getStaffSlot(this.id.substring(0,6));
        } else if (this.option == "Booked") {
          this.getBookedStaffSlot(this.id.substring(0,6));
        }
        
      } else if (this.id.substring(0, 1) == 's')
      {
        this.title = "Student's Slots";
        this.page = "s";
        this._slotService.getStudentSlots(this.id).subscribe(slotData => this.slotList = slotData);
      }
    }
  }

  // Get all staff's slot
  private getStaffSlot(id: string) {
    this._slotService.getStaffSlots(id).subscribe(slotData => this.slotList = slotData);
  }

  // Get all booked student's slot
  private getBookedStaffSlot(id: string) {
    this._slotService.getBookedStaffSlots(id).subscribe(slotData => this.slotList = slotData);
  }


  deleteSlot(roomID, startTime) {
    // Set date and time to proper format 
    var dateSlot = new Date(startTime).toLocaleDateString();
    var timeSlot = (new Date(startTime).toLocaleTimeString()).substring(0,5);
    var slotTime = dateSlot + " " + timeSlot;

    const ans = confirm(`Do you want to delete slot: ${roomID}, ${slotTime}`);
    if (ans) {

      //Checking the option to return to all staff's slot or booked slot 
      if (this.option == "All") {
        this._slotService.deleteSlot(roomID, slotTime).subscribe((data) => {
          this.getStaffSlot(this.id);
        }, error => console.error(error));
      } else {
        this._slotService.deleteSlot(roomID, slotTime).subscribe((data) => {
          this.getBookedStaffSlot(this.id);
        }, error => console.error(error));
      }       
    }
  }

}
