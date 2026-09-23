import React from 'react';
import { useAuth } from '../features/auth/context/AuthContext';
import { LogOut, Server, User as UserIcon } from 'lucide-react';

export const Navbar: React.FC = () => {
  const { user, logout } = useAuth();

  return (
    <header className="bg-slate-900 text-white shadow-md">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <div className="flex items-center space-x-3">
          <div className="bg-sky-500 p-2 rounded-lg">
            <Server className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="font-bold text-lg leading-tight">ASISYA System</h1>
            <p className="text-xs text-slate-400">Gestión de Productos & Categorías</p>
          </div>
        </div>

        {user && (
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2 bg-slate-800 px-3 py-1.5 rounded-full text-sm">
              <UserIcon className="w-4 h-4 text-sky-400" />
              <span className="font-medium">{user.username}</span>
              <span className="bg-sky-500/20 text-sky-300 text-xs px-2 py-0.5 rounded-full uppercase">
                {user.role}
              </span>
            </div>
            <button
              onClick={logout}
              className="flex items-center space-x-1.5 text-sm bg-red-600/80 hover:bg-red-600 text-white px-3 py-1.5 rounded-lg transition"
            >
              <LogOut className="w-4 h-4" />
              <span>Salir</span>
            </button>
          </div>
        )}
      </div>
    </header>
  );
};
