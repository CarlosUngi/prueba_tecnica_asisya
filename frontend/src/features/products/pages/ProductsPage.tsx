import React, { useState, useEffect } from 'react';
import api from '../../../services/api';
import { 
  Search, Plus, Zap, RefreshCw, ChevronLeft, ChevronRight, 
  Tag, Image as ImageIcon, Edit3, Trash2, X, AlertCircle, CheckCircle2 
} from 'lucide-react';

interface Category {
  categoryId: number;
  categoryName: string;
  description?: string;
  picture?: string;
}

interface Product {
  productId: number;
  productName: string;
  supplierId?: number;
  supplierName?: string;
  categoryId: number;
  categoryName: string;
  categoryPicture?: string;
  quantityPerUnit?: string;
  unitPrice: number;
  unitsInStock: number;
  unitsOnOrder: number;
  reorderLevel: number;
  discontinued: boolean;
  createdAt: string;
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const ProductsPage: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [bulkLoading, setBulkLoading] = useState(false);
  const [notification, setNotification] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  // Filtros y Paginación
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | ''>('');
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  // Modales
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  // Form State
  const [formData, setFormData] = useState({
    productName: '',
    categoryId: 1,
    quantityPerUnit: '1 unidad',
    unitPrice: 100,
    unitsInStock: 50,
    discontinued: false,
  });

  const fetchCategories = async () => {
    try {
      const res = await api.get('/categories');
      setCategories(res.data);
      if (res.data.length > 0 && !formData.categoryId) {
        setFormData((prev) => ({ ...prev, categoryId: res.data[0].categoryId }));
      }
    } catch (err) {
      console.error('Error al cargar categorías', err);
    }
  };

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const params: any = {
        pageNumber,
        pageSize,
      };
      if (searchTerm.trim()) params.searchTerm = searchTerm.trim();
      if (selectedCategoryId) params.categoryId = selectedCategoryId;

      const res = await api.get<PagedResult<Product>>('/products', { params });
      setProducts(res.data.items);
      setTotalCount(res.data.totalCount);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      showNotification('error', 'Error al cargar el catálogo de productos.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  useEffect(() => {
    fetchProducts();
  }, [pageNumber, pageSize, selectedCategoryId]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    fetchProducts();
  };

  const showNotification = (type: 'success' | 'error', message: string) => {
    setNotification({ type, message });
    setTimeout(() => setNotification(null), 5000);
  };

  // Carga Masiva de 100,000 registros
  const handleBulkInsert = async () => {
    if (!window.confirm('¿Deseas generar e insertar masivamente 100,000 productos aleatorios en la base de datos?')) {
      return;
    }

    setBulkLoading(true);
    const startTime = performance.now();

    try {
      const res = await api.post('/products/bulk?count=100000');
      const endTime = performance.now();
      const seconds = ((endTime - startTime) / 1000).toFixed(2);

      showNotification('success', `⚡ ¡Carga Masiva Exitosa! Se insertaron 100,000 productos en ${seconds} segundos.`);
      setPageNumber(1);
      fetchProducts();
    } catch (err) {
      showNotification('error', 'Ocurrió un error al ejecutar la carga masiva.');
    } finally {
      setBulkLoading(false);
    }
  };

  // Guardar Nuevo Producto
  const handleCreateProduct = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/products', formData);
      showNotification('success', 'Producto creado exitosamente.');
      setShowCreateModal(false);
      resetForm();
      fetchProducts();
    } catch (err) {
      showNotification('error', 'Error al crear el producto.');
    }
  };

  // Editar Producto
  const handleUpdateProduct = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingProduct) return;

    try {
      await api.put(`/products/${editingProduct.productId}`, formData);
      showNotification('success', 'Producto actualizado correctamente.');
      setEditingProduct(null);
      resetForm();
      fetchProducts();
    } catch (err) {
      showNotification('error', 'Error al actualizar el producto.');
    }
  };

  // Borrar Producto
  const handleDeleteProduct = async (id: number) => {
    if (!window.confirm('¿Estás seguro de eliminar este producto?')) return;

    try {
      await api.delete(`/products/${id}`);
      showNotification('success', 'Producto eliminado.');
      fetchProducts();
    } catch (err) {
      showNotification('error', 'Error al eliminar el producto.');
    }
  };

  const openEditModal = (p: Product) => {
    setEditingProduct(p);
    setFormData({
      productName: p.productName,
      categoryId: p.categoryId,
      quantityPerUnit: p.quantityPerUnit || '',
      unitPrice: p.unitPrice,
      unitsInStock: p.unitsInStock,
      discontinued: p.discontinued,
    });
  };

  const resetForm = () => {
    setFormData({
      productName: '',
      categoryId: categories[0]?.categoryId || 1,
      quantityPerUnit: '1 unidad',
      unitPrice: 100,
      unitsInStock: 50,
      discontinued: false,
    });
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Notificación Toast */}
      {notification && (
        <div className={`mb-6 p-4 rounded-xl flex items-center justify-between shadow-lg text-sm font-medium ${
          notification.type === 'success' ? 'bg-emerald-500/10 border border-emerald-500/20 text-emerald-400' : 'bg-red-500/10 border border-red-500/20 text-red-400'
        }`}>
          <div className="flex items-center space-x-3">
            {notification.type === 'success' ? <CheckCircle2 className="w-5 h-5" /> : <AlertCircle className="w-5 h-5" />}
            <span>{notification.message}</span>
          </div>
          <button onClick={() => setNotification(null)}><X className="w-4 h-4" /></button>
        </div>
      )}

      {/* Header y Acciones Principales */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Catálogo de Productos</h1>
          <p className="text-sm text-slate-500">
            Total de registros en base de datos: <span className="font-semibold text-slate-700">{totalCount.toLocaleString()}</span>
          </p>
        </div>

        <div className="flex items-center space-x-3">
          <button
            onClick={handleBulkInsert}
            disabled={bulkLoading}
            className="flex items-center space-x-2 px-4 py-2.5 bg-amber-500 hover:bg-amber-600 text-white text-sm font-semibold rounded-xl shadow-sm transition disabled:opacity-50"
          >
            <Zap className={`w-4 h-4 ${bulkLoading ? 'animate-bounce' : ''}`} />
            <span>{bulkLoading ? 'Insertando 100k...' : 'Carga Masiva (100,000)'}</span>
          </button>

          <button
            onClick={() => { resetForm(); setShowCreateModal(true); }}
            className="flex items-center space-x-2 px-4 py-2.5 bg-sky-600 hover:bg-sky-700 text-white text-sm font-semibold rounded-xl shadow-sm transition"
          >
            <Plus className="w-4 h-4" />
            <span>Nuevo Producto</span>
          </button>
        </div>
      </div>

      {/* Barra de Filtros y Búsqueda */}
      <div className="bg-white rounded-2xl p-4 shadow-sm border border-slate-200 mb-6 flex flex-col sm:flex-row gap-4 items-center justify-between">
        <form onSubmit={handleSearchSubmit} className="relative flex-1 w-full">
          <Search className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Buscar producto por nombre..."
            className="w-full pl-10 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-sky-500"
          />
        </form>

        <div className="flex items-center space-x-3 w-full sm:w-auto">
          <select
            value={selectedCategoryId}
            onChange={(e) => {
              setSelectedCategoryId(e.target.value ? Number(e.target.value) : '');
              setPageNumber(1);
            }}
            className="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-sky-500 text-slate-700"
          >
            <option value="">Todas las Categorías</option>
            {categories.map((c) => (
              <option key={c.categoryId} value={c.categoryId}>
                {c.categoryName}
              </option>
            ))}
          </select>

          <button
            onClick={fetchProducts}
            className="p-2 text-slate-600 hover:bg-slate-100 rounded-xl transition"
            title="Recargar"
          >
            <RefreshCw className={`w-5 h-5 ${loading ? 'animate-spin' : ''}`} />
          </button>
        </div>
      </div>

      {/* Tabla de Productos */}
      <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse text-sm">
            <thead>
              <tr className="bg-slate-50 border-b border-slate-200 text-slate-500 font-semibold uppercase text-xs">
                <th className="py-3.5 px-4">ID</th>
                <th className="py-3.5 px-4">Producto</th>
                <th className="py-3.5 px-4">Categoría & Foto</th>
                <th className="py-3.5 px-4">Precio Unitario</th>
                <th className="py-3.5 px-4">Stock</th>
                <th className="py-3.5 px-4 text-center">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-12 text-slate-400">
                    <RefreshCw className="w-6 h-6 animate-spin mx-auto mb-2" />
                    Cargando catálogo...
                  </td>
                </tr>
              ) : products.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center py-12 text-slate-400">
                    No se encontraron productos registrados.
                  </td>
                </tr>
              ) : (
                products.map((p) => (
                  <tr key={p.productId} className="hover:bg-slate-50/80 transition">
                    <td className="py-3.5 px-4 font-mono text-xs text-slate-500">#{p.productId}</td>
                    <td className="py-3.5 px-4 font-medium text-slate-900">{p.productName}</td>
                    <td className="py-3.5 px-4">
                      <div className="flex items-center space-x-2.5">
                        {p.categoryPicture ? (
                          <img src={p.categoryPicture} alt={p.categoryName} className="w-8 h-8 rounded-lg object-cover border" />
                        ) : (
                          <div className="w-8 h-8 rounded-lg bg-slate-100 flex items-center justify-center text-slate-400">
                            <ImageIcon className="w-4 h-4" />
                          </div>
                        )}
                        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-sky-50 text-sky-700">
                          {p.categoryName}
                        </span>
                      </div>
                    </td>
                    <td className="py-3.5 px-4 font-semibold text-slate-900">${p.unitPrice.toFixed(2)}</td>
                    <td className="py-3.5 px-4">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                        p.unitsInStock > 10 ? 'bg-emerald-50 text-emerald-700' : 'bg-red-50 text-red-700'
                      }`}>
                        {p.unitsInStock} un.
                      </span>
                    </td>
                    <td className="py-3.5 px-4 text-center">
                      <div className="flex items-center justify-center space-x-2">
                        <button
                          onClick={() => openEditModal(p)}
                          className="p-1.5 text-slate-500 hover:text-sky-600 hover:bg-sky-50 rounded-lg transition"
                          title="Editar"
                        >
                          <Edit3 className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDeleteProduct(p.productId)}
                          className="p-1.5 text-slate-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                          title="Eliminar"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Paginador */}
        <div className="bg-slate-50 px-4 py-3 border-t border-slate-200 flex items-center justify-between text-xs text-slate-500">
          <div>
            Página <span className="font-semibold text-slate-700">{pageNumber}</span> de <span className="font-semibold text-slate-700">{totalPages}</span>
          </div>

          <div className="flex items-center space-x-2">
            <button
              onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
              disabled={pageNumber === 1}
              className="p-1.5 rounded-lg border border-slate-200 bg-white hover:bg-slate-100 disabled:opacity-40"
            >
              <ChevronLeft className="w-4 h-4" />
            </button>
            <button
              onClick={() => setPageNumber((p) => Math.min(totalPages, p + 1))}
              disabled={pageNumber >= totalPages}
              className="p-1.5 rounded-lg border border-slate-200 bg-white hover:bg-slate-100 disabled:opacity-40"
            >
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Modal Crear / Editar */}
      {(showCreateModal || editingProduct) && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-xl border border-slate-200">
            <div className="flex items-center justify-between mb-4">
              <h3 className="font-bold text-lg text-slate-900">
                {editingProduct ? 'Editar Producto' : 'Nuevo Producto'}
              </h3>
              <button onClick={() => { setShowCreateModal(false); setEditingProduct(null); }}>
                <X className="w-5 h-5 text-slate-400 hover:text-slate-600" />
              </button>
            </div>

            <form onSubmit={editingProduct ? handleUpdateProduct : handleCreateProduct} className="space-y-4">
              <div>
                <label className="block text-xs font-semibold text-slate-600 uppercase mb-1">Nombre</label>
                <input
                  type="text"
                  required
                  value={formData.productName}
                  onChange={(e) => setFormData({ ...formData, productName: e.target.value })}
                  className="w-full px-3 py-2 border rounded-xl text-sm focus:ring-2 focus:ring-sky-500 outline-none"
                />
              </div>

              <div>
                <label className="block text-xs font-semibold text-slate-600 uppercase mb-1">Categoría</label>
                <select
                  value={formData.categoryId}
                  onChange={(e) => setFormData({ ...formData, categoryId: Number(e.target.value) })}
                  className="w-full px-3 py-2 border rounded-xl text-sm focus:ring-2 focus:ring-sky-500 outline-none"
                >
                  {categories.map((c) => (
                    <option key={c.categoryId} value={c.categoryId}>{c.categoryName}</option>
                  ))}
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-xs font-semibold text-slate-600 uppercase mb-1">Precio ($)</label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={formData.unitPrice}
                    onChange={(e) => setFormData({ ...formData, unitPrice: Number(e.target.value) })}
                    className="w-full px-3 py-2 border rounded-xl text-sm focus:ring-2 focus:ring-sky-500 outline-none"
                  />
                </div>
                <div>
                  <label className="block text-xs font-semibold text-slate-600 uppercase mb-1">Stock</label>
                  <input
                    type="number"
                    required
                    value={formData.unitsInStock}
                    onChange={(e) => setFormData({ ...formData, unitsInStock: Number(e.target.value) })}
                    className="w-full px-3 py-2 border rounded-xl text-sm focus:ring-2 focus:ring-sky-500 outline-none"
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-3 pt-4">
                <button
                  type="button"
                  onClick={() => { setShowCreateModal(false); setEditingProduct(null); }}
                  className="px-4 py-2 border rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-50"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-sky-600 hover:bg-sky-700 text-white rounded-xl text-sm font-medium shadow-sm"
                >
                  {editingProduct ? 'Guardar Cambios' : 'Crear Producto'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
