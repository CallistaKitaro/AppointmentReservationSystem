import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { StudentService } from '../../services/student.service';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
    selector: 'add-student',
    templateUrl: './add-student.component.html',
    styleUrls: ['./add-student.component.css']
})
/** add-student component*/
export class AddStudentComponent implements OnInit {
  form; 
  title: string = "Add";
  errorMessage: any;
  studentID: string;

  constructor(private _studentService: StudentService, private _avRoute: ActivatedRoute, private _router: Router)
  {
    //Get the param id from url
    if (this._avRoute.snapshot.paramMap.get("id")) {
      this.studentID = this._avRoute.snapshot.paramMap.get("id");
      console.log(this.studentID);
    }
    //Initial form
    this.form = new FormGroup({
      studentID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(s|S)\\d{7}$')])),
      firstName: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^[A-Za-z]*$')])),
      lastName: new FormControl('', Validators.pattern('^[A-Za-z]*$')),
      email: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(s|S)\\d{7}\@student.rmit.edu.au')])),
    });
  }

  ngOnInit() {
    //Checking whether this component for add or edit
    if (this.studentID != null) {
      this.title = "Edit";
      this._studentService.getStudentById(this.studentID).subscribe(resp => this.form.setValue(resp),
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
    // Add new student
    if (this.title === "Add") {
      this._studentService.saveStudent(this.form.value).subscribe((data) =>
      {
        this._router.navigate(["/fetch-student"]);
      }, error => this.errorMessage = error);
    }
    //Edit student data
      else if (this.title === "Edit") {
      this._studentService.updateStudent(this.studentID, this.form.value).subscribe((data) =>
      {
        this._router.navigate(["/fetch-student"]);
      }, error => this.errorMessage = error);
    }
  }

  cancel()
  {
    this._router.navigate(["/fetch-student"]);
  }

  
}
