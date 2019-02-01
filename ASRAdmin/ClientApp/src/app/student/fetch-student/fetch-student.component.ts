import { Component, Inject } from '@angular/core';
//import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'fetch-student',
  templateUrl: './fetch-student.component.html'
})
export class FetchStudentComponent {


}

interface Student {
  StudentID: string;
  FirstName: number;
  LastName: number;
  Email: string;
}
