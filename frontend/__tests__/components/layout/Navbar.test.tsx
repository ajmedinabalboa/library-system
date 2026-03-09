import React from 'react';
import { render, screen } from '../../test-utils';
import userEvent from '@testing-library/user-event';
import { Navbar } from '@/components/layout/Navbar';

const mockPush = jest.fn();

jest.mock('next/navigation', () => ({
  useRouter: jest.fn(() => ({ push: mockPush })),
}));

jest.mock('@/lib/context/AuthContext', () => ({
  useAuth: jest.fn(),
}));

import { useAuth } from '@/lib/context/AuthContext';

describe('Navbar', () => {
  beforeEach(() => {
    (useAuth as jest.Mock).mockReturnValue({ logout: jest.fn() });
  });

  it('renders the library title', () => {
    render(<Navbar />);
    expect(screen.getByText('Sistema de Biblioteca')).toBeInTheDocument();
  });

  it('renders the logout button', () => {
    render(<Navbar />);
    expect(
      screen.getByRole('button', { name: /cerrar sesión/i }),
    ).toBeInTheDocument();
  });

  it('calls logout and navigates to /login when the logout button is clicked', async () => {
    const mockLogout = jest.fn();
    (useAuth as jest.Mock).mockReturnValue({ logout: mockLogout });
    render(<Navbar />);
    await userEvent.click(screen.getByRole('button', { name: /cerrar sesión/i }));
    expect(mockLogout).toHaveBeenCalledTimes(1);
    expect(mockPush).toHaveBeenCalledWith('/login');
  });

  it('navigates to /books when the title text is clicked', async () => {
    render(<Navbar />);
    await userEvent.click(screen.getByText('Sistema de Biblioteca'));
    expect(mockPush).toHaveBeenCalledWith('/books');
  });
});
