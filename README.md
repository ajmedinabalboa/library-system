# Digital Library System - Complete Solution

## Project Overview

A fullstack Digital Library System built with .NET 10 Web API (backend) and Next.js (frontend), following Clean Architecture principles and modern best practices.

## ✅ Backend - COMPLETED

The backend has been fully implemented with Clean Architecture, CQRS, and JWT authentication.

### Backend Structure
```
backend/src/
├── LibrarySystem.Domain/          ✅ Domain entities
├── LibrarySystem.Application/     ✅ CQRS commands/queries, DTOs, validators
├── LibrarySystem.Infrastructure/  ✅ DbContext, repositories, JWT, password hashing
└── LibrarySystem.WebAPI/          ✅ Controllers, auth, CORS configuration
```

### Features Implemented
- ✅ Clean Architecture with 4 layers
- ✅ CQRS pattern with MediatR
- ✅ Repository and Unit of Work patterns
- ✅ JWT authentication with refresh tokens
- ✅ FluentValidation
- ✅ Entity Framework Core with PostgreSQL
- ✅ Comprehensive book management (CRUD)
- ✅ Advanced filtering, sorting, and pagination
- ✅ Author management with duplicate prevention
- ✅ Swagger/OpenAPI documentation

### Running the Backend

1. **Setup PostgreSQL**:
   ```sql
   CREATE DATABASE librarydb;
   ```

2. **Run Migrations**:
   ```bash
   dotnet ef migrations add InitialCreate \
     --project backend/src/LibrarySystem.Infrastructure \
     --startup-project backend/src/LibrarySystem.WebAPI
   
   dotnet ef database update \
     --project backend/src/LibrarySystem.Infrastructure \
     --startup-project backend/src/LibrarySystem.WebAPI
   ```

3. **Run the API**:
   ```bash
   cd backend/src/LibrarySystem.WebAPI
   dotnet run
   ```

4. **Access Swagger**: `https://localhost:5001/swagger`

### 🎯 Test Data Included

The database is automatically seeded with test data:

- **Users:**
  - Admin: `admin@library.com` / `Admin123!`
  - User: `user@library.com` / `User123!`

- **Books:** 6 classic software engineering books with authors

📖 **Details:** See [backend/SEED_DATA.md](./backend/SEED_DATA.md)

## 📋 Frontend - Next Steps

The frontend should be implemented with the following structure:

### Recommended Folder Structure
```
frontend/
├── app/
│   ├── (auth)/
│   │   └──  login/
│   │       └── page.tsx              # Login page
│   ├── (dashboard)/
│   │   └── books/
│   │       ├── page.tsx              # Books list page
│   │       ├── create/
│   │       │   └── page.tsx          # Create book page
│   │       └── [id]/
│   │           └── edit/
│   │               └── page.tsx      # Edit book page
│   ├── layout.tsx                    # Root layout
│   └── page.tsx                      # Home page
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
│   │   ├── axios.ts                  # Axios configuration with interceptors
│   │   ├── authService.ts            # Auth API calls
│   │   └── bookService.ts            # Book API calls
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

### Setup Frontend

1. **Create Next.js App**:
   ```bash
   npx create-next-app@latest frontend --typescript --tailwind --app
   cd frontend
   ```

2. **Install Dependencies**:
   ```bash
   npm install axios
   npm install -D @types/node
   ```

3. **Create Environment Variables** (`.env.local`):
   ```env
   NEXT_PUBLIC_API_URL=https://localhost:5001/api
   ```

### Key Frontend Files to Implement

#### 1. Axios Configuration (`lib/api/axios.ts`)
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

#### 2. Auth Service (`lib/api/authService.ts`)
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

#### 3. Book Service (`lib/api/bookService.ts`)
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

#### 4. Login Page (`app/(auth)/login/page.tsx`)
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

#### 5. Books List Page (`app/(dashboard)/books/page.tsx`)
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

## 🚀 Complete Setup Instructions

### 1. Backend Setup
```bash
# Clone/navigate to project
cd library-system

# Restore packages
dotnet restore

# Setup database
createdb librarydb

# Run migrations
dotnet ef database update \
  --project src/LibrarySystem.Infrastructure \
  --startup-project src/LibrarySystem.WebAPI

# Run API
cd src/LibrarySystem.WebAPI
dotnet run
```

### 2. Frontend Setup
```bash
# Create Next.js app
npx create-next-app@latest frontend --typescript --tailwind --app
cd frontend

# Install dependencies
npm install axios

# Create .env.local
echo "NEXT_PUBLIC_API_URL=https://localhost:5001/api" > .env.local

# Run development server
npm run dev
```

### 3. Create Test User
```sql
-- You'll need to hash the password using ASP.NET Core Identity Password Hasher
-- Or create a seed script in the backend

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

## 📝 API Endpoints Summary

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /api/auth/login | Login | No |
| POST | /api/auth/refresh | Refresh token | No |
| GET | /api/books | List books (with filters) | Yes |
| GET | /api/books/{id} | Get book by ID | Yes |
| POST | /api/books | Create book | Yes |
| PUT | /api/books/{id} | Update book | Yes |
| DELETE | /api/books/{id} | Delete book | Yes |

## 🏗️ Architecture Highlights

- ✅  **Clean Architecture**: Clear separation of concerns
- ✅ **CQRS**: Commands and queries separated
- ✅ **Repository Pattern**: Data access abstraction
- ✅ **JWT Authentication**: Secure stateless auth
- ✅ **Axios Interceptors**: Automatic token refresh
- ✅ **Type Safety**: Full TypeScript support
- ✅ **Responsive Design**: Tail windCSS styling
- ✅ **Feature-based Structure**: Organized by domain

## 📚 Additional Resources

- [Backend README](README.backend.md) - Detailed backend documentation
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Next.js Documentation](https://nextjs.org/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

## 🎯 Next Steps

1. Implement remaining frontend pages (Create/Edit book)
2. Add form validation with React Hook Form
3. Add loading states and error handling
4. Implement advanced filtering UI
5. Add unit and integration tests
6. Set up CI/CD pipeline
7. Add Docker support
8. Implement comprehensive logging

## 📄 License

This project was created for demonstration purposes.
