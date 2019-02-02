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

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { FetchStudentComponent } from './student/fetch-student/fetch-student.component';
import { AddStudentComponent } from './student/add-student/add-student.component';
import { FetchStaffComponent } from './staff/fetch-staff/fetch-staff.component';
import { AddStaffComponent } from './staff/add-staff/add-staff.component';
import { SlotFormComponent } from './slot/slot-form/slot-form.component';
import { ViewSlotComponent } from './slot/view-slot/view-slot.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    FetchStudentComponent,
    AddStudentComponent,
    FetchStaffComponent,
    AddStaffComponent,
    SlotFormComponent,
    ViewSlotComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'fetch-student', component: FetchStudentComponent },
      { path: 'add-student', component: AddStudentComponent },
      { path: "add-student/:id", component: AddStudentComponent },
      { path: 'fetch-staff', component: FetchStaffComponent },
      { path: 'add-staff', component: AddStaffComponent },
      { path: "add-staff/:id", component: AddStaffComponent },
      { path: "slot-form", component: SlotFormComponent },
      { path: "view-slot/:id", component: ViewSlotComponent },
    ])
  ],
  providers: [StudentService, StaffService, SlotService],
  bootstrap: [AppComponent]
})
export class AppModule { }
