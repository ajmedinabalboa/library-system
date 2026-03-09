import React from 'react';
import { render, screen } from '../../test-utils';
import { PageLoader } from '@/components/common/PageLoader';

describe('PageLoader', () => {
  it('renders the default "Cargando..." message', () => {
    render(<PageLoader />);
    expect(screen.getByText('Cargando...')).toBeInTheDocument();
  });

  it('renders a custom message', () => {
    render(<PageLoader message="Por favor espera" />);
    expect(screen.getByText('Por favor espera')).toBeInTheDocument();
  });

  it('renders a circular progress indicator', () => {
    render(<PageLoader />);
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });
});
