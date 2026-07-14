export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  phone: string;
  roleId: number;
}
