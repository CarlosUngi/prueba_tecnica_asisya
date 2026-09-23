import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from '../features/auth/pages/LoginPage';
import { ProductsPage } from '../features/products/pages/ProductsPage';
import { AuthGuard } from './AuthGuard';
import { Navbar } from '../components/Navbar';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      {/* Rutas protegidas por AuthGuard */}
      <Route element={<AuthGuard />}>
        <Route
          path="/products"
          element={
            <>
              <Navbar />
              <ProductsPage />
            </>
          }
        />
      </Route>

      {/* Redirección por defecto: si no hay token, AuthGuard lo enviará a /login */}
      <Route path="/" element={<Navigate to="/products" replace />} />
      <Route path="*" element={<Navigate to="/products" replace />} />
    </Routes>
  );
};
