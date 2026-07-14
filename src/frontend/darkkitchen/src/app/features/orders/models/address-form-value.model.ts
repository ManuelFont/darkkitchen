export interface AddressFormValue {
  street: string;
  doorNumber: number | null;
  apartment: string;
}

export interface AddressFormChange {
  valid: boolean;
  value: AddressFormValue;
}
