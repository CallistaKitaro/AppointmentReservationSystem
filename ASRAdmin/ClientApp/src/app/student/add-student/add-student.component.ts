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
  
    if (this._avRoute.snapshot.paramMap.get("id")) {
      this.studentID = this._avRoute.snapshot.paramMap.get("id");
      console.log(this.studentID);
    }
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

    if (this.title === "Add") {
      this._studentService.saveStudent(this.form.value).subscribe((data) =>
      {
        this._router.navigate(["/fetch-student"]);
      }, error => this.errorMessage = error);
    } else if (this.title === "Edit") {
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
