import { Component, OnInit } from '@angular/core';
import { RoomService } from '../../services/room.service';
import { Room } from '../../Models/room';
import { Router } from "@angular/router";

@Component({
    selector: 'fetch-room',
    templateUrl: './fetch-room.component.html',
    styleUrls: ['./fetch-room.component.css']
})
/** fetch-room component*/
export class FetchRoomComponent implements OnInit {
  roomList: Room[];

  constructor(private _roomService: RoomService, private _router: Router) {
    this.getRooms();
  }

  ngOnInit() {
    this.getRooms();
  }

  getRooms() {
    this._roomService.getRooms().subscribe(roomData => this.roomList = roomData);
  }

  deleteRoom(roomID) {

    const ans = confirm("Do you want to delete room with ID " + roomID);
    if (ans) {
      this._roomService.deleteRoom(roomID).subscribe((data) => {
        this.getRooms();
      }, error => console.error(error));
    }

  }

}
