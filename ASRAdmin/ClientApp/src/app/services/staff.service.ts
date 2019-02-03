import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Staff } from '../Models/staff';
import 'rxjs/add/operator/map';
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

@Injectable()
export class StaffService {
  private baseUrl: string = "https://localhost:44317/ASRapi/";

  constructor(private _http: Http) {}

  getStaffs(): Observable<Staff[]>
  {
    return this._http.get(this.baseUrl +'Staff/GetAllStaffs')
      .map((response: Response) => response.json()).catch(this.errorHandler) 
  }

  getStaffById(id: string)
  {
    return this._http.get(this.baseUrl + `Staff/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  deleteStaff(id: string)
  {
    return this._http.delete(this.baseUrl + `Staff/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  updateStaff(id: string, staff)
  {
    return this._http.put(this.baseUrl + `Staff/${id}`, staff)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  saveStaff(staff)
  {
    return this._http.post(this.baseUrl + 'Staff/', staff)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  errorHandler(error: Response) {
    console.log(error);
    return Observable.throw(error);
  }

}
