# Digital Library System - Frontend

Modern web interface built with Next.js 14, TypeScript, and Tailwind CSS for managing books and authors.

## 🚀 Technologies

- **Next.js 14** - App Router with React Server Components
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **Axios** - HTTP client with JWT interceptors
- **React Hooks** - Modern state management

## 📁 Project Structure

```
frontend/
├── app/                      # Next.js App Router pages
│   ├── layout.tsx           # Root layout with global styles
│   ├── page.tsx             # Home page (auto-redirects)
│   ├── login/               # Authentication
│   │   └── page.tsx         # Login form
│   └── books/               # Book management
│       ├── page.tsx         # List all books (with search/filter)
│       ├── create/          # Create new book
│       │   └── page.tsx
│       └── [id]/edit/       # Edit existing book
│           └── page.tsx
├── components/              # Reusable React components
│   └── Navbar.tsx          # Navigation bar
├── lib/                    # Core utilities
│   ├── types/              # TypeScript interfaces
│   │   ├── auth.ts         # Auth-related types
│   │   └── book.ts         # Book-related types
│   └── api/                # API client services
│       ├── axios.ts        # Configured Axios with interceptors
│       ├── authService.ts  # Authentication operations
│       └── bookService.ts  # Book CRUD operations
└── styles/                 # Global styles
    └── globals.css         # Tailwind imports & custom CSS
```

## 🔧 Configuration

### Environment Variables

Create `.env.local`:

```env
NEXT_PUBLIC_API_URL=https://localhost:5001/api
```

### API Integration

The frontend uses Axios with automatic JWT token management:

- **Request Interceptor**: Automatically adds Bearer token to all requests
- **Response Interceptor**: Handles 401 errors by refreshing tokens
- **Auto-redirect**: Redirects to login if refresh token expires

## 🎯 Features

### Authentication
- **Login Page**: Email/password authentication
- **Token Management**: Access token (15 min) + Refresh token (7 days)
- **Auto Refresh**: Seamless token refresh on expiration
- **Protected Routes**: Automatic redirect to login for unauthorized access

### Book Management
- **List Books**: Paginated view with 9 books per page
- **Search**: Filter books by title
- **Sort**: By title, publication date, or author count (ascending/descending)
- **Create Book**: Add new books with multiple authors
- **Edit Book**: Update book details and authors
- **Delete Book**: Remove books with confirmation

### User Experience
- **Responsive Design**: Mobile-friendly layouts
- **Loading States**: Spinner indicators during operations
- **Error Handling**: Clear error messages with validation
- **Form Validation**: Client-side validation before submission

## 🏃 Running the Application

### Development Mode

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### Production Build

```bash
npm run build
npm start
```

## 📝 Usage

### 1. Login
- Navigate to `/login`
- Use credentials created via the backend API
- Default test user: `admin@library.com` / `Admin123!`

### 2. View Books
- After login, you'll see the books list
- Use search bar to filter by title
- Use sort dropdown to order results
- Click page numbers for pagination

### 3. Create Book
- Click "Add Book" in the navigation
- Enter book title (required)
- Optionally add publication date
- Add at least one author (type name and click "Add")
- Remove authors by clicking the X on their tags
- Click "Create Book" to save

### 4. Edit Book
- Click "Edit" button on any book card
- Modify title, date, or authors
- Click "Save Changes" to update

### 5. Delete Book
- Click "Delete" button on any book card
- Confirm the deletion in the prompt

## 🔐 Authentication Flow

```
1. User enters credentials → POST /auth/login
2. Receives access token (15 min) and refresh token (7 days)
3. Access token stored in localStorage
4. All API requests include: Authorization: Bearer <token>
5. On 401 error → POST /auth/refresh with refresh token
6. New access token received and stored
7. Original request retried automatically
8. If refresh fails → Redirect to login
```

## 🎨 Styling

- **Tailwind CSS**: Utility-first approach
- **Color Scheme**: Blue primary, gray neutrals
- **Components**: Cards, buttons, forms, navigation
- **Responsive**: Mobile-first with breakpoints

## 🐛 Troubleshooting

### Backend Connection Issues
- Ensure backend is running on `https://localhost:5001`
- Check .env.local has correct API URL
- Verify CORS is enabled in backend

### Authentication Problems
- Clear localStorage: `localStorage.clear()`
- Check token expiration in browser DevTools
- Verify user credentials in database

### Build Errors
- Delete `.next/` folder: `rm -rf .next`
- Clear node_modules: `rm -rf node_modules && npm install`
- Check TypeScript errors: `npm run build`

## 📦 Dependencies

### Production
- `next`: ^14.2.3
- `react`: ^18.3.1
- `react-dom`: ^18.3.1
- `axios`: ^1.6.8

### Development
- `typescript`: ^5
- `@types/node`: ^20
- `@types/react`: ^18
- `@types/react-dom`: ^18
- `tailwindcss`: ^3.4.1
- `postcss`: ^8
- `autoprefixer`: ^10.0.1

## 🚀 Next Steps

- Add user registration page
- Implement author search/autocomplete
- Add book cover image upload
- Create user profile management
- Add favorites/bookmarks feature
- Implement advanced search filters
