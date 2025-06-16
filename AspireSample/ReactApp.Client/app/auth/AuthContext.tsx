import { createContext, useCallback, useMemo, useState } from "react";
import type { User } from "~/types/auth";
import { GUEST_USER } from "./contants";



const AuthContext = createContext<{
  user: User;
  isAuthenticated: boolean;
  hasRole: (roles: string[]) => boolean;
  login: () => void;
  logout: () => void;
}>({
  user: GUEST_USER,
  isAuthenticated: false,
  hasRole: (roles: string[]) => false,
  login: () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: React.ReactElement }) {
  const [user, setUser] = useState<User>(GUEST_USER);

  const isAuthenticated = useMemo(
    () => user.id != GUEST_USER.id,
    [user.id]
  );

  const hasRole = useCallback(
    (roles: string[]) => {
      return roles.some((role) => user.roles.includes(role));
    },
    [user.roles]
  );

  const login = useCallback(() => {
  }, []);

  const logout = useCallback(() => {
  }, []);

  const contextValue = useMemo(
    () => ({
      isAuthenticated,
      user,
      hasRole,
      login,
      logout,
    }),
    [user, login, hasRole, logout, isAuthenticated]
  );

  return <AuthContext value={contextValue}>{children}</AuthContext>;
}

export default AuthContext;
