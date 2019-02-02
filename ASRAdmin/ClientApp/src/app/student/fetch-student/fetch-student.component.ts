import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../services/student.service';
import { Student } from '../../Models/student';
import { Router } from "@angular/router";

@Component({
  selector: 'fetch-student',
  templateUrl: './fetch-student.component.html'
})

export class FetchStudentComponent implements OnInit {
  stdList: Student[];

  constructor(private _studentService: StudentService, private _router: Router)
  {
    this.getStudents();
  }

  ngOnInit() {
    this.getStudents();
  }

  getStudents()
  {
    this._studentService.getStudents().subscribe(stdData => this.stdList = stdData);
  }

  deleteStudent(stdID)
  {
    const ans = confirm("Do you want to delete student with ID " + stdID);
    if (ans)
    {
      this._studentService.deleteStudent(stdID).subscribe((data) => {
        this.getStudents();
      }, error => console.error(error));
    }

  }

  //editStudent(stdID)
  //{
  //  this._router.navigate(['/add-student/', stdID])
  //}

}
