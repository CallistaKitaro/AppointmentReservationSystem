import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Student } from '../Models/student';
import 'rxjs/add/operator/map';
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

@Injectable()
export class StudentService {
  private baseUrl: string = "https://localhost:44317/ASRapi/";

  constructor(private _http: Http) {}

  getStudents(): Observable<Student[]>
  {
    return this._http.get(this.baseUrl +'Student/GetAllStudents')
      .map((response: Response) => response.json()).catch(this.errorHandler) 
  }

  getStudentById(id: string)
  {
    return this._http.get(this.baseUrl + `Student/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  deleteStudent(id: string)
  {
    return this._http.delete(this.baseUrl + `Student/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  updateStudent(id: string, student)
  {
    return this._http.put(this.baseUrl + `Student/${id}`, student)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  saveStudent(student)
  {
    return this._http.post(this.baseUrl + 'Student/', student)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  errorHandler(error: Response) {
    console.log(error);
    return Observable.throw(error);
  }

}
