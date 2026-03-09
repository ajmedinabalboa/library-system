'use client';

import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import { authService } from '@/lib/api/authService';
import type { LoginRequest } from '@/lib/types/auth';

interface AuthContextValue {
  isAuthenticated: boolean;
  role: string | null;
  isAdmin: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  // Start with false/null so server and client initial render match (no hydration error).
  // Sync the real values from localStorage after mount (client-only).
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [role, setRole] = useState<string | null>(null);

  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
    setRole(authService.getRole());
  }, []);

  const login = useCallback(async (credentials: LoginRequest) => {
    await authService.login(credentials);
    setIsAuthenticated(true);
    setRole(authService.getRole());
  }, []);

  const logout = useCallback(() => {
    authService.logout();
    setIsAuthenticated(false);
    setRole(null);
  }, []);

  return (
    <AuthContext.Provider value={{ isAuthenticated, role, isAdmin: role === 'Admin', login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
