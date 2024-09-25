import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { ItemService } from '../../services/item/item.service';
import { itemGet } from '../../models/item.get.model';
import { ActivatedRoute, Router,RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PopupService } from '../../services/popup/popup.service';
@Component({
  selector: 'app-item-info',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './item-info.component.html',
  styleUrl: './item-info.component.css'
})
export class ItemInfoComponent implements OnInit {
  itemId: string | null = null;
  id! : number;
  item: itemGet = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    image: ''
  };
  constructor(private itemService: ItemService, private route: ActivatedRoute, private router: Router, private popupService: PopupService) {}
  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.route.paramMap.subscribe(params => {
      this.itemId = params.get('id');
      this.id = parseInt(this.itemId!);
      this.itemService.details(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          this.item = response;
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(error?.error || 'Error fetching item details');
          console.error('Error fetching item details:', error);
        }
      );
    });
  }

}
