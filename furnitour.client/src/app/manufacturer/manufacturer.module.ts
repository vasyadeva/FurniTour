import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateManufacturerComponent } from './update-manufacturer/update-manufacturer.component';
import { InfoManufacturerComponent } from './info-manufacturer/info-manufacturer.component';
import { ManufacturersComponent } from './manufacturers/manufacturers.component';
import { AddManufacturerComponent } from './add-manufacturer/add-manufacturer.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class ManufacturerModule { }
