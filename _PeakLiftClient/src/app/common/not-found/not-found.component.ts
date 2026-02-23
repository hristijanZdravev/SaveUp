import { Component } from '@angular/core';
import { TPipe } from '../../_pipes/t.pipe';

@Component({
  selector: 'app-not-found',
  imports: [TPipe],
  standalone: true,
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.css'
})
export class NotFoundComponent {

}
