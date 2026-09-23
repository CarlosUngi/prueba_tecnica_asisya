import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../features/auth/context/AuthContext';

export const AuthGuard: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const tokenInStorage = localStorage.getItem('token');

  // Si no hay token en localStorage o en el estado del contexto, redirigir inmediatamente al Login
  if (!isAuthenticated || !tokenInStorage) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};
