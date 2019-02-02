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
  slotList: Slot[];
  id: string;

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
        this.title ="Staff's Slots"
        this._slotService.getStaffSlots(this.id).subscribe(slotData => this.slotList = slotData);
      } else if (this.id.substring(0, 1) === 's')
      {
        this.title = "Staff's Slots"
        this._slotService.getStudentSlots(this.id).subscribe(slotData => this.slotList = slotData);
      }
    }
  }
}
