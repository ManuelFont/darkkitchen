import { Component, input, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { startWith } from 'rxjs';
import { AddressFormChange } from '../models/address-form-value.model';

const ALPHANUMERIC_SPACES = /^[\p{L}\p{N}\s]+$/u;
const POSITIVE_INTEGER = /^[1-9]\d*$/;

function optionalAlphanumeric(control: AbstractControl): ValidationErrors | null {
  const value = (control.value ?? '').trim();
  if (!value) {
    return null;
  }

  return ALPHANUMERIC_SPACES.test(value) ? null : { alphanumeric: true };
}

@Component({
  selector: 'app-address-form',
  imports: [ReactiveFormsModule],
  templateUrl: './address-form.component.html',
})
export class AddressFormComponent {
  readonly showErrors = input(false);
  readonly formChange = output<AddressFormChange>();

  protected readonly form = new FormGroup({
    street: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(ALPHANUMERIC_SPACES)],
    }),
    doorNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(POSITIVE_INTEGER)],
    }),
    apartment: new FormControl('', {
      nonNullable: true,
      validators: [optionalAlphanumeric],
    }),
  });

  protected get street(): FormControl {
    return this.form.controls.street;
  }

  protected get doorNumber(): FormControl {
    return this.form.controls.doorNumber;
  }

  protected get apartment(): FormControl {
    return this.form.controls.apartment;
  }

  constructor() {
    this.form.valueChanges
      .pipe(startWith(this.form.getRawValue()), takeUntilDestroyed())
      .subscribe(() => {
        const { street, doorNumber, apartment } = this.form.getRawValue();
        const parsed = Number.parseInt(doorNumber, 10);

        this.formChange.emit({
          valid: this.form.valid,
          value: {
            street: street.trim(),
            doorNumber: Number.isNaN(parsed) ? null : parsed,
            apartment: apartment.trim(),
          },
        });
      });
  }
}
