import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { StudentService } from '../../services/student.service';
//import { Student } from '../../Models/student';
//import { Console } from '@angular/core/src/console';
import { Router } from "@angular/router";


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

  constructor(private _studentService: StudentService, private _router: Router) {}

  ngOnInit() {
   
    this.form = new FormGroup({
      studentID: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(s|S)\\d{7}$')])),
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl(''),
      email: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^(s|S)\\d{7}\@student.rmit.edu.au')])),
    });
  }

  save()
  {
    if (!this.form.valid) {
      return;
    }

    if (this.title === "Add") {
      console.log(this.form.get('studentID'));
      this._studentService.saveStudent(this.form.value).subscribe((data) => {
        this._router.navigate(["/fetch-student"]);
      }, error => this.errorMessage = error);
    } 

  }

  cancel()
  {
    this._router.navigate(["/fetch-student"]);
  }

  
}
