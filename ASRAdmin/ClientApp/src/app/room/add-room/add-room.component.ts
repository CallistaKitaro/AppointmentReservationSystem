import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { RoomService } from '../../services/room.service';
import { Router, ActivatedRoute } from "@angular/router";
import { Room } from '../../Models/room';

@Component({
    selector: 'add-room',
    templateUrl: './add-room.component.html',
    styleUrls: ['./add-room.component.css']
})
/** add-room component*/
export class AddRoomComponent {
  form;
  totalRoom: Room[];
  title: string = "Add";
  errorMessage: any;
  roomID: string;

  constructor(private _roomService: RoomService, private _avRoute: ActivatedRoute, private _router: Router) {

    if (this._avRoute.snapshot.paramMap.get("id")) {
      this.roomID = this._avRoute.snapshot.paramMap.get("id");
    }

    // Initial room form
    this.form = new FormGroup({
      roomID: new FormControl(''),
      roomName: new FormControl('', Validators.compose([
        Validators.required, Validators.pattern('^[A-Za-z]*$')])),  
    });
  }

  ngOnInit() {

    if (this.roomID != null) {
      this.title = "Edit";
      this._roomService.getRoomById(this.roomID).subscribe(resp => this.form.setValue(resp),
        error => this.errorMessage = error);
    } else {
      this.title = "Add";
    }
  }

  save() {
    if (!this.form.valid) {
      return;
    }

    //Check if this Add room or Edit page
    if (this.title === "Add") {

      this._roomService.getRooms().subscribe(roomData => this.totalRoom = roomData);
      var sum = this.totalRoom.length;

      let newRoom = new Room();
      newRoom.roomID = String(sum + 1);
      newRoom.roomName = (this.form.get('roomName').value).toUpperCase();

      this._roomService.saveRoom(newRoom).subscribe((data) => {
        this._router.navigate(["/fetch-room"]);
      }, error => this.errorMessage = error);
      
    } else if (this.title === "Edit") {
      this._roomService.updateRoom(this.roomID, this.form.value).subscribe((data) => {
        this._router.navigate(["/fetch-room"]);
      }, error => this.errorMessage = error);
    }
  }

  cancel() {
    this._router.navigate(["/fetch-room"]);
  }
 
}
