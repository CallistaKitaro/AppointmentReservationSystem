import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { StaffService } from '../../services/staff.service';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
    selector: 'add-staff',
    templateUrl: './add-staff.component.html',
    styleUrls: ['./add-staff.component.css']
})
/** add-staff component*/
export class AddStaffComponent implements OnInit {
  form; 
  title: string = "Add";
  errorMessage: any;
  staffID: string;

  constructor(private _staffService: StaffService, private _avRoute: ActivatedRoute, private _router: Router)
  {
    //Get the params id that pas to url
    if (this._avRoute.snapshot.paramMap.get("id"))
    {
      this.staffID = this._avRoute.snapshot.paramMap.get("id");
    }
    //Initial form
    this.form = new FormGroup({
      staffID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(e|E)\\d{5}$')])),
      firstName: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^[A-Za-z]*$')])),
      lastName: new FormControl('', Validators.pattern('^[A-Za-z]*$')),
      email: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(e|E)\\d{5}\@rmit.edu.au')])),
    });
  }

  ngOnInit() {
    //Checking whether this page to add or edit staff
    if (this.staffID != null) {
      this.title = "Edit";
      this._staffService.getStaffById(this.staffID).subscribe(resp => this.form.setValue(resp),
        error => this.errorMessage = error);
    } else {
      this.title = "Add";  
    }   
  }

  save()
  {
    if (!this.form.valid) {
      return;
    }
    // Save new staff
    if (this.title === "Add") {
      this._staffService.saveStaff(this.form.value).subscribe((data) =>
      {
        this._router.navigate(["/fetch-staff"]);
      }, error => this.errorMessage = error);
    }
    // Or edit old staff details
    else if (this.title === "Edit") {
      this._staffService.updateStaff(this.staffID, this.form.value).subscribe((data) =>
      {
        this._router.navigate(["/fetch-staff"]);
      }, error => this.errorMessage = error);
    }
  }

  cancel()
  {
    this._router.navigate(["/fetch-staff"]);
  }

  
}
