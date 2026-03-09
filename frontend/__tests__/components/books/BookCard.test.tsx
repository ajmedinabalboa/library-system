import React from 'react';
import { render, screen } from '../../test-utils';
import userEvent from '@testing-library/user-event';
import { BookCard } from '@/components/books/BookCard';
import type { Book } from '@/lib/types/book';

jest.mock('next/link', () => ({
  __esModule: true,
  default: React.forwardRef<HTMLAnchorElement, React.ComponentPropsWithoutRef<'a'>>(
    function MockLink({ href, children, ...rest }, ref) {
      return (
        <a href={href} ref={ref} {...rest}>
          {children}
        </a>
      );
    },
  ),
}));

jest.mock('@/lib/context/AuthContext', () => ({
  useAuth: jest.fn(),
}));

import { useAuth } from '@/lib/context/AuthContext';

const mockBook: Book = {
  id: '1',
  title: 'Cien Años de Soledad',
  publicationDate: '1967-05-30',
  createdAt: '2024-01-01',
  authors: [{ id: 'a1', name: 'Gabriel Garcia Marquez' }],
  authorCount: 1,
};

describe('BookCard', () => {
  beforeEach(() => {
    (useAuth as jest.Mock).mockReturnValue({ isAdmin: false });
  });

  it('renders the book title', () => {
    render(<BookCard book={mockBook} onDelete={jest.fn()} />);
    expect(screen.getByText('Cien Años de Soledad')).toBeInTheDocument();
  });

  it('renders the author name', () => {
    render(<BookCard book={mockBook} onDelete={jest.fn()} />);
    expect(screen.getByText('Gabriel Garcia Marquez')).toBeInTheDocument();
  });

  it('renders the author count', () => {
    render(<BookCard book={mockBook} onDelete={jest.fn()} />);
    expect(screen.getByText(/1 autor/)).toBeInTheDocument();
  });

  it('shows "Autor desconocido" when the book has no authors', () => {
    const bookNoAuthors: Book = { ...mockBook, authors: [], authorCount: 0 };
    render(<BookCard book={bookNoAuthors} onDelete={jest.fn()} />);
    expect(screen.getByText('Autor desconocido')).toBeInTheDocument();
  });

  it('hides edit and delete buttons for non-admin users', () => {
    render(<BookCard book={mockBook} onDelete={jest.fn()} />);
    expect(screen.queryByRole('link', { name: /editar/i })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /eliminar/i })).not.toBeInTheDocument();
  });

  it('shows edit and delete buttons for admin users', () => {
    (useAuth as jest.Mock).mockReturnValue({ isAdmin: true });
    render(<BookCard book={mockBook} onDelete={jest.fn()} />);
    expect(screen.getByRole('link', { name: /editar/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /eliminar/i })).toBeInTheDocument();
  });

  it('calls onDelete with the book when the Eliminar button is clicked', async () => {
    const onDelete = jest.fn();
    (useAuth as jest.Mock).mockReturnValue({ isAdmin: true });
    render(<BookCard book={mockBook} onDelete={onDelete} />);
    await userEvent.click(screen.getByRole('button', { name: /eliminar/i }));
    expect(onDelete).toHaveBeenCalledWith(mockBook);
  });
});
