import axios from 'axios';
import {store } from '../../../app/store';
import { LoginDto } from '../../../shared/dto/LoginDto';
import { UserDto } from '../../../shared/dto/UserDto';
import {selectToken, logoutAction, setCredentials} from '../slices/authSlice';
import {RegisterUserDto} from "../../../shared/dto/RegisterUserDto";

const AUTH_API_URL = `${process.env.REACT_APP_BASE_URL}/api/Auth`;
const USER_API_URL = `${process.env.REACT_APP_BASE_URL}/api/UserApp`;

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
    const { token } = response.data;

    // Get user data
    const userData = await getUserByEmail(credentials.UserEmail);

    // Update Redux store
    store.dispatch(setCredentials({ user: userData, token }));

    return userData;
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
    const response = await axios.post(`${USER_API_URL}/SaveUser`, user);

    store.dispatch(setCredentials({user: user, token: response.data.token}))

    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getUserByEmail = async (email: string | undefined): Promise<UserDto> => {
  if (!email) {
    throw new Error('Email is required');
  }

  try {
    const response = await authAxios.post(`${USER_API_URL}/GetUserByEmailId`, { UserEmail: email });

    return response.data;
  } catch (error) {
    console.error('Error fetching user by email:', error);
    throw error;
  }
};

export const getUserById = async (userId: number | undefined): Promise<UserDto> => {
  if (!userId) {
    throw new Error('User ID is required');
  }

  const response = await authAxios.post(`${USER_API_URL}/GetUserById`, { UserId: userId });
  return response.data;
};

export const updateUser = async (user: UserDto): Promise<UserDto> => {
  const response = await authAxios.post(`${USER_API_URL}/SaveUser`, user);

  // If updating the current user, update Redux store
  const currentState = store.getState().auth;
  if (currentState.user && (currentState.user as UserDto).UserId === user.UserId) {
    store.dispatch(setCredentials({
      user: response.data,
      token: currentState.token
    }));
  }

  return response.data;
};

export const deleteUser = async (userId: number | undefined): Promise<void> => {
  if (!userId) {
    throw new Error('User ID is required');
  }

  await authAxios.post(`${USER_API_URL}/DeleteUser`, { UserId: userId });

  // If deleting current user, logout
  const currentState = store.getState().auth;
  if (currentState.user && (currentState.user as UserDto).UserId === userId) {
    await logout();
  }
};
