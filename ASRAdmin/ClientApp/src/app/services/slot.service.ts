import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Slot } from '../Models/slot';
import 'rxjs/add/operator/map';
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

@Injectable()
export class SlotService {
  private baseUrl: string = "https://localhost:44317/ASRapi/";

  constructor(private _http: Http) {}

  getSlots(): Observable<Slot[]>
  {
    return this._http.get(this.baseUrl +'Slot/GetAllSlots')
      .map((response: Response) => response.json()).catch(this.errorHandler) 
  }

  getSlotDetail(roomid: string, startTime: string): Observable<Slot>
  {
    return this._http.get(this.baseUrl + `Slot?roomid=${roomid}&startTime=${startTime}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  getStaffSlots(id: string): Observable<Slot[]>
  {
    return this._http.get(this.baseUrl + `Slot/ByStaffId?id=${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  getStudentSlots(id: string): Observable<Slot[]>
  {
    return this._http.get(this.baseUrl + `Slot/ByStudentId?id=${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  deleteSlot(roomid: string, startTime: string)
  {
    return this._http.delete(this.baseUrl + `Slot?roomid=${roomid}&startTime=${startTime}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  updateSlot(roomid: string, startTime: string, slot)
  {
    return this._http.put(this.baseUrl + `Slot?roomid=${roomid}&startTime=${startTime}`, slot)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  saveStudent(slot)
  {
    return this._http.post(this.baseUrl + 'Slot/', slot)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  errorHandler(error: Response) {
    console.log(error);
    return Observable.throw(error);
  }

}
