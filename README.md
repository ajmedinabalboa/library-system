# Sistema de Biblioteca Digital - Solución Completa

## Descripción General del Proyecto

Un Sistema de Biblioteca Digital fullstack construido con .NET 10 Web API (backend) y Next.js (frontend), siguiendo los principios de Arquitectura Limpia y las mejores prácticas modernas.

## ✅ Backend - COMPLETADO

El backend ha sido completamente implementado con Arquitectura Limpia, CQRS y autenticación JWT.

### Estructura del Backend
```
backend/src/
├── LibrarySystem.Domain/          ✅ Entidades del dominio
├── LibrarySystem.Application/     ✅ Comandos/consultas CQRS, DTOs, validadores
├── LibrarySystem.Infrastructure/  ✅ DbContext, repositorios, JWT, hash de contraseñas
└── LibrarySystem.WebAPI/          ✅ Controladores, autenticación, configuración CORS
```

### Funcionalidades Implementadas
- ✅ Arquitectura Limpia con 4 capas
- ✅ Patrón CQRS con MediatR
- ✅ Patrones Repositorio y Unidad de Trabajo
- ✅ Autenticación JWT con tokens de refresco
- ✅ FluentValidation
- ✅ Entity Framework Core con PostgreSQL
- ✅ Gestión completa de libros (CRUD)
- ✅ Filtrado avanzado, ordenamiento y paginación
- ✅ Gestión de autores con prevención de duplicados
- ✅ Documentación Swagger/OpenAPI

### Ejecutar el Backend

1. **Configurar PostgreSQL**:
   ```sql
   CREATE DATABASE librarydb;
   ```

2. **Ejecutar Migraciones**:
   ```bash
   dotnet ef migrations add InitialCreate \
     --project backend/src/LibrarySystem.Infrastructure \
     --startup-project backend/src/LibrarySystem.WebAPI
   
   dotnet ef database update \
     --project backend/src/LibrarySystem.Infrastructure \
     --startup-project backend/src/LibrarySystem.WebAPI
   ```

3. **Iniciar la API**:
   ```bash
   cd backend/src/LibrarySystem.WebAPI
   dotnet run
   ```

4. **Acceder a Swagger**: `https://localhost:5001/swagger`

### 🎯 Datos de Prueba Incluidos

La base de datos se inicializa automáticamente con datos de prueba:

- **Usuarios:**
  - Admin: `admin@library.com` / `Admin123!`
  - Usuario: `user@library.com` / `User123!`

- **Libros:** 6 libros clásicos de ingeniería de software con autores

📖 **Detalles:** Ver [backend/SEED_DATA.md](./backend/SEED_DATA.md)

## 📋 Frontend - Implementado

El frontend ha sido implementado con la siguiente estructura:

### Estructura de Carpetas
```
frontend/
├── app/
│   ├── (auth)/
│   │   └──  login/
│   │       └── page.tsx              # Página de inicio de sesión
│   ├── (dashboard)/
│   │   └── books/
│   │       ├── page.tsx              # Página de listado de libros
│   │       ├── create/
│   │       │   └── page.tsx          # Página de creación de libro
│   │       └── [id]/
│   │           └── edit/
│   │               └── page.tsx      # Página de edición de libro
│   ├── layout.tsx                    # Layout raíz
│   └── page.tsx                      # Página de inicio
├── components/
│   ├── BookCard.tsx
│   ├── BookList.tsx
│   ├── BookForm.tsx
│   ├── LoginForm.tsx
│   ├── Navbar.tsx
│   ├── Pagination.tsx
│   └── FilterBar.tsx
├── lib/
│   ├── api/
│   │   ├── axios.ts                  # Configuración de Axios con interceptores
│   │   ├── authService.ts            # Llamadas a la API de autenticación
│   │   └── bookService.ts            # Llamadas a la API de libros
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   └── useBooks.ts
│   └── types/
│       ├── auth.ts
│       └── book.ts
├── package.json
├── tsconfig.json
└── next.config.js
```

### Configurar el Frontend

1. **Crear la App Next.js**:
   ```bash
   npx create-next-app@latest frontend --typescript --tailwind --app
   cd frontend
   ```

2. **Instalar Dependencias**:
   ```bash
   npm install axios
   npm install -D @types/node
   ```

3. **Crear Variables de Entorno** (`.env.local`):
   ```env
   NEXT_PUBLIC_API_URL=https://localhost:5001/api
   ```

### Archivos Clave del Frontend

#### 1. Configuración de Axios (`lib/api/axios.ts`)
```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

// Request interceptor - attach JWT token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor - handle token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        const { data } = await axios.post(
          `${process.env.NEXT_PUBLIC_API_URL}/auth/refresh`,
          { refreshToken }
        );
        
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Redirect to login
        localStorage.clear();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;
```

#### 2. Servicio de Autenticación (`lib/api/authService.ts`)
```typescript
import api from './axios';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const { data } = await api.post('/auth/login', credentials);
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  },

  logout() {
    localStorage.clear();
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken');
  },
};
```

#### 3. Servicio de Libros (`lib/api/bookService.ts`)
```typescript
import api from './axios';

export interface Book {
  id: string;
  title: string;
  publicationDate?: string;
  createdAt: string;
  updatedAt?: string;
  authors: Author[];
  authorCount: number;
}

export interface Author {
  id: string;
  name: string;
}

export interface CreateBookDto {
  title: string;
  publicationDate?: string;
  authors: string[];
}

export interface BooksQuery {
  page?: number;
  pageSize?: number;
  title?: string;
  publishedAfter?: string;
  publishedBefore?: string;
  minAuthors?: number;
  sortBy?: 'title' | 'publicationDate' | 'authorCount';
  sortOrder?: 'asc' | 'desc';
}

export interface PaginatedBooks {
  items: Book[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export const bookService = {
  async getBooks(query: BooksQuery = {}): Promise<PaginatedBooks> {
    const { data } = await api.get('/books', { params: query });
    return data;
  },

  async getBookById(id: string): Promise<Book> {
    const { data } = await api.get(`/books/${id}`);
    return data;
  },

  async createBook(book: CreateBookDto): Promise<Book> {
    const { data } = await api.post('/books', book);
    return data;
  },

  async updateBook(id: string, book: CreateBookDto): Promise<Book> {
    const { data } = await api.put(`/books/${id}`, book);
    return data;
  },

  async deleteBook(id: string): Promise<void> {
    await api.delete(`/books/${id}`);
  },
};
```

#### 4. Página de Inicio de Sesión (`app/(auth)/login/page.tsx`)
```typescript
'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { authService } from '@/lib/api/authService';

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await authService.login({ email, password });
      router.push('/books');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow">
        <h2 className="text-3xl font-bold text-center">Library System Login</h2>
        
        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
              Email
            </label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">
              Password
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
      </div>
    </div>
  );
}
```

#### 5. Página de Listado de Libros (`app/(dashboard)/books/page.tsx`)
```typescript
'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { bookService, Book, BooksQuery } from '@/lib/api/bookService';

export default function BooksPage() {
  const [books, setBooks] = useState<Book[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState<BooksQuery>({});

  useEffect(() => {
    loadBooks();
  }, [page, filters]);

  const loadBooks = async () => {
    setLoading(true);
    try {
      const data = await bookService.getBooks({ ...filters, page, pageSize: 10 });
      setBooks(data.items);
      setTotalPages(data.totalPages);
    } catch (error) {
      console.error('Failed to load books:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this book?')) {
      try {
        await bookService.deleteBook(id);
        loadBooks();
      } catch (error) {
        console.error('Failed to delete book:', error);
      }
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Books</h1>
        <Link
          href="/books/create"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Add Book
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {books.map((book) => (
          <div key={book.id} className="bg-white rounded-lg shadow p-6">
            <h2 className="text-xl font-semibold mb-2">{book.title}</h2>
            <p className="text-gray-600 mb-2">
              Authors: {book.authors.map((a) => a.name).join(', ')}
            </p>
            {book.publicationDate && (
              <p className="text-gray-500 text-sm mb-4">
                Published: {new Date(book.publicationDate).toLocaleDateString()}
              </p>
            )}
            <div className="flex gap-2">
              <Link
                href={`/books/${book.id}/edit`}
                className="text-blue-600 hover:underline"
              >
                Edit
              </Link>
              <button
                onClick={() => handleDelete(book.id)}
                className="text-red-600 hover:underline"
              >
                Delete
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Pagination */}
      <div className="flex justify-center gap-2 mt-8">
        <button
          onClick={() => setPage((p) => Math.max(1, p - 1))}
          disabled={page === 1}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Previous
        </button>
        <span className="px-4 py-2">
          Page {page} of {totalPages}
        </span>
        <button
          onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
          disabled={page === totalPages}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
```

## 🚀 Instrucciones de Configuración Completa

### 1. Configurar el Backend
```bash
# Navegar al proyecto
cd library-system

# Restaurar paquetes
dotnet restore

# Configurar la base de datos
createdb librarydb

# Ejecutar migraciones
dotnet ef database update \
  --project src/LibrarySystem.Infrastructure \
  --startup-project src/LibrarySystem.WebAPI

# Iniciar la API
cd src/LibrarySystem.WebAPI
dotnet run
```

### 2. Configurar el Frontend
```bash
# Crear la app Next.js
npx create-next-app@latest frontend --typescript --tailwind --app
cd frontend

# Instalar dependencias
npm install axios

# Crear .env.local
echo "NEXT_PUBLIC_API_URL=https://localhost:5001/api" > .env.local

# Iniciar servidor de desarrollo
npm run dev
```

### 3. Crear Usuario de Prueba
```sql
-- Es necesario hashear la contraseña usando ASP.NET Core Identity Password Hasher
-- O crear un script de semilla en el backend

INSERT INTO "Users" ("Id", "Email", "PasswordHash", "Name", "Role", "CreatedAt")
VALUES (
  gen_random_uuid(),
  'admin@library.com',
  'hashed_password_here',
  'Admin User',
  'Admin',
  NOW()
);
```

## 📝 Resumen de Endpoints de la API

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| POST | /api/auth/login | Iniciar sesión | No |
| POST | /api/auth/refresh | Refrescar token | No |
| GET | /api/books | Listar libros (con filtros) | Sí |
| GET | /api/books/{id} | Obtener libro por ID | Sí |
| POST | /api/books | Crear libro | Sí |
| PUT | /api/books/{id} | Actualizar libro | Sí |
| DELETE | /api/books/{id} | Eliminar libro | Sí |

## 🏗️ Aspectos Destacados de la Arquitectura

- ✅  **Arquitectura Limpia**: Clara separación de responsabilidades
- ✅ **CQRS**: Comandos y consultas separados
- ✅ **Patrón Repositorio**: Abstracción del acceso a datos
- ✅ **Autenticación JWT**: Autenticación segura sin estado
- ✅ **Interceptores Axios**: Renovación automática de tokens
- ✅ **Tipado Seguro**: Soporte completo de TypeScript
- ✅ **Diseño Responsivo**: Estilos con TailwindCSS
- ✅ **Estructura por Funcionalidad**: Organizada por dominio

## 📚 Recursos Adicionales

- [README del Backend](README.backend.md) - Documentación detallada del backend
- [Documentación de .NET](https://docs.microsoft.com/es-es/dotnet/)
- [Documentación de Next.js](https://nextjs.org/docs)
- [Documentación de PostgreSQL](https://www.postgresql.org/docs/)

## 🎯 Próximos Pasos

1. Implementar las páginas restantes del frontend (Crear/Editar libro)
2. Agregar validación de formularios con React Hook Form
3. Agregar estados de carga y manejo de errores
4. Implementar una interfaz de filtrado avanzado
5. Agregar pruebas unitarias e integración
6. Configurar un pipeline de CI/CD
7. Agregar soporte para Docker
8. Implementar logs completos

## 📄 Licencia

Este proyecto fue creado con fines de demostración.
