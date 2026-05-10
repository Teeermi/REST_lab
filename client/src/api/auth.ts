import api from './client';
import type { AuthResponse } from '../types';

export const login = async (email: string, password: string): Promise<AuthResponse> => {
  const { data } = await api.post<AuthResponse>('/users/login', { email, password });
  return data;
};

export const register = async (email: string, username: string, password: string): Promise<AuthResponse> => {
  const { data } = await api.post<AuthResponse>('/users', { email, username, password });
  return data;
};
