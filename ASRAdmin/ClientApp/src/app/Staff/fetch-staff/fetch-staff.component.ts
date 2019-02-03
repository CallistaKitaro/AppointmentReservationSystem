import { Component, OnInit } from '@angular/core';
import { StaffService } from '../../services/staff.service';
import { Staff } from '../../Models/staff';
import { Router } from "@angular/router";

@Component({
  selector: 'fetch-staff',
  templateUrl: './fetch-staff.component.html'
})

export class FetchStaffComponent implements OnInit {
  staffList: Staff[];

  constructor(private _staffService: StaffService, private _router: Router)
  {
    this.getStaffs();
  }

  ngOnInit() {
    this.getStaffs();
  }

  getStaffs()
  {
    this._staffService.getStaffs().subscribe(staffData => this.staffList = staffData);
  }

  deleteStaff(staffID)
  {
    const ans = confirm("Do you want to delete staff with ID " + staffID);
    if (ans)
    {
      this._staffService.deleteStaff(staffID).subscribe((data) => {
        this.getStaffs();
      }, error => console.error(error));
    }
  }

  editStaff(staffID)
  {
    this._router.navigate(['/add-staff/', staffID])
  }

}
