import { Component } from '@angular/core';
import { TPipe } from '../../_pipes/t.pipe';

@Component({
  selector: 'app-logout',
  imports: [TPipe],
  standalone: true,
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent {

}
