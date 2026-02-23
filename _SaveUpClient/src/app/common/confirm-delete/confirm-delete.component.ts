import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TPipe } from '../../_pipes/t.pipe';

@Component({
  selector: 'app-confirm-delete',
  imports: [CommonModule, TPipe],
  standalone: true,
  templateUrl: './confirm-delete.component.html',
  styleUrl: './confirm-delete.component.css'
})
export class ConfirmDeleteComponent {
  @Input() isOpen = false;
  @Input() title = '';
  @Input() message = '';
  @Input() itemName = '';
  @Input() isLoading = false;

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }

  onBackdropClick() {
    this.onCancel();
  }
}
