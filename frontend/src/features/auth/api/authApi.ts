import axios from 'axios';
import {store } from '../../../app/store';
import { LoginDto } from '../../../shared/dto/LoginDto';
import { UserDto } from '../../../shared/dto/UserDto';
import {selectToken, logoutAction, setCredentials} from '../slices/authSlice';
import {RegisterUserDto} from "../../../shared/dto/RegisterUserDto";
import {AUTH_API_URL, USER_API_URL} from '../../../shared/apiEndpoints';

// Setup axios with auth interceptor
const authAxios = axios.create();
authAxios.interceptors.request.use(config => {
  const token = selectToken(store.getState())

  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

// Auth functions
export const login = async (credentials: LoginDto): Promise<UserDto> => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/login`, credentials);
    const { token, user } = response.data;

    store.dispatch(setCredentials({ user: user, token }));

    return user;
  } catch (error) {
    store.dispatch(logoutAction());
    throw error;
  }
};

export const logout = async (): Promise<void> => {
  try {
    await authAxios.post(`${AUTH_API_URL}/logout`);
  } catch (error) {
    console.error('Logout error:', error);
  } finally {
    // Clear local storage and Redux state regardless of server response
    store.dispatch(logoutAction());
  }
};

export const register = async (user: RegisterUserDto): Promise<UserDto> => {
  try {
    const response = await axios.post(`${USER_API_URL}/user`, user);

    store.dispatch(setCredentials({user: user, token: response.data.token}))

    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getUserById = async (userId: number | undefined): Promise<UserDto> => {
  if (!userId) {
    throw new Error('User ID is required');
  }

  const response = await authAxios.get(`${USER_API_URL}/user`);
  return response.data;
};

export const deleteUser = async (userId: number | undefined): Promise<void> => {
  if (!userId) {
    throw new Error('User ID is required');
  }

  await authAxios.delete(`${USER_API_URL}/user`);

  // If deleting current user, logout
  const currentState = store.getState().auth;
  if (currentState.user && (currentState.user as UserDto).UserId === userId) {
    await logout();
  }
};
