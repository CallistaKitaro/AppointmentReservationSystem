import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { HttpModule } from "@angular/http";
import { RouterModule } from '@angular/router';

import { StudentService } from './services/student.service';
import { StaffService } from './services/staff.service';
import { SlotService } from './services/slot.service';
import { RoomService } from './services/room.service';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FetchRoomComponent } from './room/fetch-room/fetch-room.component';
import { AddRoomComponent } from './room/add-room/add-room.component';
import { FetchStudentComponent } from './student/fetch-student/fetch-student.component';
import { AddStudentComponent } from './student/add-student/add-student.component';
import { FetchStaffComponent } from './staff/fetch-staff/fetch-staff.component';
import { AddStaffComponent } from './staff/add-staff/add-staff.component';
import { SlotFormComponent } from './slot/slot-form/slot-form.component';
import { ViewSlotComponent } from './slot/view-slot/view-slot.component';
import { EditSlotComponent } from './slot/edit-slot/edit-slot.component';
import { SlotFormStudentComponent } from './slot/slot-form-student/slot-form-student.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FetchRoomComponent,
    AddRoomComponent,
    FetchStudentComponent,
    AddStudentComponent,
    FetchStaffComponent,
    AddStaffComponent,
    SlotFormComponent,
    SlotFormStudentComponent,
    ViewSlotComponent,
    EditSlotComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'fetch-room', component: FetchRoomComponent },
      { path: 'add-room', component: AddRoomComponent },
      { path: 'add-room/:id', component: AddRoomComponent },
      { path: 'fetch-student', component: FetchStudentComponent },
      { path: 'add-student', component: AddStudentComponent },
      { path: 'add-student/:id', component: AddStudentComponent },
      { path: 'fetch-staff', component: FetchStaffComponent },
      { path: 'add-staff', component: AddStaffComponent },
      { path: 'add-staff/:id', component: AddStaffComponent },
      { path: 'slot-form', component: SlotFormComponent },
      { path: 'slot-form-student', component: SlotFormStudentComponent},
      { path: 'view-slot/:id', component: ViewSlotComponent },
      { path: 'edit-slot/:id', component: EditSlotComponent },
    ])
  ],
  providers: [StudentService, StaffService, SlotService, RoomService],
  bootstrap: [AppComponent]
})
export class AppModule { }
