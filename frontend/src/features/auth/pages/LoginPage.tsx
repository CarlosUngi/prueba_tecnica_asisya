import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../../../services/api';
import { Lock, User, Server, AlertCircle } from 'lucide-react';

export const LoginPage: React.FC = () => {
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('Admin123!');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const response = await api.post('/auth/login', { username, password });
      const { token, username: userResp, email, role } = response.data;
      login(token, { username: userResp, email, role });
      navigate('/products');
    } catch (err: any) {
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else {
        setError('No se pudo conectar con el servidor API.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-900 flex items-center justify-center p-4">
      <div className="max-w-md w-full bg-slate-800 rounded-2xl shadow-xl border border-slate-700 p-8">
        <div className="text-center mb-8">
          <div className="inline-flex p-3 bg-sky-500/10 text-sky-400 rounded-xl mb-3">
            <Server className="w-10 h-10" />
          </div>
          <h2 className="text-2xl font-bold text-white">ASISYA Dev II</h2>
          <p className="text-sm text-slate-400 mt-1">Inicia sesión para gestionar el catálogo de productos</p>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-500/10 border border-red-500/20 rounded-xl flex items-center space-x-3 text-red-400 text-sm">
            <AlertCircle className="w-5 h-5 flex-shrink-0" />
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-2">
              Usuario
            </label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">
                <User className="w-5 h-5" />
              </div>
              <input
                type="text"
                required
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent transition"
                placeholder="Ingresa tu usuario"
              />
            </div>
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-2">
              Contraseña
            </label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">
                <Lock className="w-5 h-5" />
              </div>
              <input
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent transition"
                placeholder="••••••••"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3.5 px-4 bg-sky-600 hover:bg-sky-500 text-white font-semibold rounded-xl shadow-lg shadow-sky-600/30 transition duration-200 flex items-center justify-center disabled:opacity-50"
          >
            {loading ? (
              <span className="inline-block animate-spin rounded-full h-5 w-5 border-b-2 border-white"></span>
            ) : (
              'Ingresar al Sistema'
            )}
          </button>
        </form>

        <div className="mt-6 text-center text-xs text-slate-500">
          Credenciales por defecto: <span className="text-slate-300">admin / Admin123!</span>
        </div>
      </div>
    </div>
  );
};
