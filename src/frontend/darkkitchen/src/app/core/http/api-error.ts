import { HttpErrorResponse } from '@angular/common/http';

const cannotConnectToApiMessage = 'Could not connect to the API';

export function getApiErrorMessage(response: HttpErrorResponse, fallbackMessage: string): string {
  if (response.status === 0) {
    return cannotConnectToApiMessage;
  }

  if (typeof response.error === 'string' && response.error.trim()) {
    return response.error;
  }

  if (typeof response.error?.message === 'string' && response.error.message.trim()) {
    return response.error.message;
  }

  return fallbackMessage;
}
