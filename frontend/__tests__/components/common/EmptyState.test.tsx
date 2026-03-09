import React from 'react';
import { render, screen } from '../../test-utils';
import { EmptyState } from '@/components/common/EmptyState';

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

describe('EmptyState', () => {
  it('renders the default title and description', () => {
    render(<EmptyState />);
    expect(screen.getByText('No se encontraron libros')).toBeInTheDocument();
    expect(
      screen.getByText('Comienza agregando tu primer libro a la biblioteca'),
    ).toBeInTheDocument();
  });

  it('renders the default action link pointing to /books/create', () => {
    render(<EmptyState />);
    const link = screen.getByRole('link', { name: /agregar libro/i });
    expect(link).toHaveAttribute('href', '/books/create');
  });

  it('renders custom title and description', () => {
    render(<EmptyState title="Sin resultados" description="No hay nada aquí" />);
    expect(screen.getByText('Sin resultados')).toBeInTheDocument();
    expect(screen.getByText('No hay nada aquí')).toBeInTheDocument();
  });

  it('does not render the action button when actionHref is empty', () => {
    render(<EmptyState actionHref="" />);
    expect(screen.queryByRole('link')).not.toBeInTheDocument();
  });

  it('renders a custom action label with a custom href', () => {
    render(<EmptyState actionLabel="Crear nuevo" actionHref="/create" />);
    const link = screen.getByRole('link', { name: /crear nuevo/i });
    expect(link).toHaveAttribute('href', '/create');
  });
});
