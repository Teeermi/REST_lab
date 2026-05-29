import { createContext, useContext, useState } from 'react';
import type { User } from '../types';

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (token: string, user: User) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

interface AuthState {
  user: User | null;
  token: string | null;
}

export const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

const emptyAuthState: AuthState = {
  user: null,
  token: null,
};

const getStoredAuthState = (): AuthState => {
  try {
    const savedToken = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');

    if (!savedToken || !savedUser) {
      return emptyAuthState;
    }

    return {
      token: savedToken,
      user: JSON.parse(savedUser) as User,
    };
  } catch {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    return emptyAuthState;
  }
};

export const useAuthState = () => {
  const [{ user, token }, setAuthState] = useState<AuthState>(getStoredAuthState);

  const login = (newToken: string, newUser: User) => {
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify(newUser));
    setAuthState({ token: newToken, user: newUser });
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setAuthState(emptyAuthState);
  };

  return {
    user,
    token,
    login,
    logout,
    isAuthenticated: !!token,
  };
};
