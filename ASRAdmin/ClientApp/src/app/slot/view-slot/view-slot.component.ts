import { Component, OnInit } from '@angular/core';
import { Pipe } from '@Angular/core';
import { SlotService } from '../../services/slot.service';
import { Slot } from '../../Models/slot';
import { Router, ActivatedRoute } from "@angular/router";
import { DatePipe } from '@angular/common';

@Component({
    selector: 'view-slot',
    templateUrl: './view-slot.component.html',
    styleUrls: ['./view-slot.component.css']
})
/** view-slot component*/
export class ViewSlotComponent implements OnInit {
  title: string = "";
  slotList: Slot[];
  id: string;
  option: string;

  constructor(private _slotService: SlotService, private _router: Router, private _avRoute: ActivatedRoute)
  {
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
        this.title = "Staff's Slots"
        if (this.option == "All") {
          this.getStaffSlot(this.id.substring(0,6));
        } else if (this.option == "Booked") {
          this.getBookedStaffSlot(this.id.substring(0,6));
        }
        
      } else if (this.id.substring(0, 1) === 's')
      {
        this.title = "Student's Slots"
        this._slotService.getStudentSlots(this.id).subscribe(slotData => this.slotList = slotData);
      }
    }
  }

  private getStaffSlot(id: string) {
    this._slotService.getStaffSlots(id).subscribe(slotData => this.slotList = slotData);
  }

  private getBookedStaffSlot(id: string) {
    this._slotService.getBookedStaffSlots(id).subscribe(slotData => this.slotList = slotData);
  }

  deleteSlot(roomID, startTime) {
    var dateSlot = new Date(startTime).toLocaleDateString();
    var timeSlot = (new Date(startTime).toLocaleTimeString()).substring(0,5);
    var slotTime = dateSlot + " " + timeSlot;

    const ans = confirm(`Do you want to delete slot: ${roomID}, ${slotTime}`);
    if (ans) {
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
