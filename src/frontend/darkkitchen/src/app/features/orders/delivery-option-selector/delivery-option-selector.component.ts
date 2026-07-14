import { Component, computed, input, output } from '@angular/core';
import { formatMoney } from '../../../shared/utils/format-money';
import { DeliveryType } from '../../delivery-types/models/delivery-type.model';

interface DeliveryOptionCard {
  id: string;
  label: string;
  description: string;
  selected: boolean;
  cardClass: string;
  radioClass: string;
}

const SELECTED_CARD = 'border-accent bg-accent-500/10';
const UNSELECTED_CARD = 'border-white/10 hover:bg-white/5';
const SELECTED_RADIO = 'border-accent';
const UNSELECTED_RADIO = 'border-white/20';

@Component({
  selector: 'app-delivery-option-selector',
  imports: [],
  templateUrl: './delivery-option-selector.component.html',
})
export class DeliveryOptionSelectorComponent {
  readonly deliveryTypes = input.required<DeliveryType[]>();
  readonly selectedId = input.required<string>();
  readonly selectId = output<string>();

  protected readonly cards = computed<DeliveryOptionCard[]>(() =>
    this.deliveryTypes().map((option) => {
      const selected = option.id === this.selectedId();

      return {
        id: option.id,
        label: option.name,
        description: `${formatMoney(option.cost)} delivery fee`,
        selected,
        cardClass: selected ? SELECTED_CARD : UNSELECTED_CARD,
        radioClass: selected ? SELECTED_RADIO : UNSELECTED_RADIO,
      };
    }),
  );
}
