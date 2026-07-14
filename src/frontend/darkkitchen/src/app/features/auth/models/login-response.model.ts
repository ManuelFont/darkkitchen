import { Role } from '../../../core/auth/role.model';

export interface LoginResponse {
  token: string;
  role: Role;
}
