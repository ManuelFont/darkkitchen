import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-nav-item',
  imports: [MatIconModule],
  templateUrl: './nav-item.component.html',
})
export class NavItemComponent {
  readonly icon = input.required<string>();
  readonly label = input.required<string>();
  readonly active = input(false);

  readonly selected = output<void>();
}
