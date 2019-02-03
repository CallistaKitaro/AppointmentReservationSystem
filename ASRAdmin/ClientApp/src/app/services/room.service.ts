import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Room } from '../Models/room';
import 'rxjs/add/operator/map';
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

@Injectable()
export class RoomService {
  private baseUrl: string = "https://localhost:44317/ASRapi/";

  constructor(private _http: Http) {}

  getRooms(): Observable<Room[]>
  {
    return this._http.get(this.baseUrl +'Room/GetAllRooms')
      .map((response: Response) => response.json()).catch(this.errorHandler) 
  }

  getRoomById(id: string) {
    return this._http.get(this.baseUrl + `Room/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  deleteRoom(id: string)
  {
    return this._http.delete(this.baseUrl + `Room/${id}`)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  updateRoom(id: string, room)
  {
    return this._http.put(this.baseUrl + `Room/${id}`, room)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  saveRoom(room)
  {
    return this._http.post(this.baseUrl + 'Room/', room)
      .map((response: Response) => response.json()).catch(this.errorHandler)
  }

  errorHandler(error: Response) {
    console.log(error);
    return Observable.throw(error);
  }

}
