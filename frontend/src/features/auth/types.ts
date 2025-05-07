export interface LoginRequest {
  userEmail: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface LogoutResponse {
  message: string;
}

export interface ReduxUser {
  UserId: number,
  UserName: string;
  UserEmail: string;
  UserTypeId: number;
}
