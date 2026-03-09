import React from 'react';
import { render, screen } from '../../test-utils';
import userEvent from '@testing-library/user-event';
import { DeleteBookDialog } from '@/components/books/DeleteBookDialog';

describe('DeleteBookDialog', () => {
  it('renders the book title in the dialog when open', () => {
    render(
      <DeleteBookDialog
        open
        bookTitle="Harry Potter"
        onConfirm={jest.fn()}
        onCancel={jest.fn()}
      />,
    );
    expect(screen.getByText(/Harry Potter/)).toBeInTheDocument();
  });

  it('does not render dialog content when closed', () => {
    render(
      <DeleteBookDialog
        open={false}
        bookTitle="Harry Potter"
        onConfirm={jest.fn()}
        onCancel={jest.fn()}
      />,
    );
    expect(screen.queryByText(/Harry Potter/)).not.toBeInTheDocument();
  });

  it('calls onConfirm when the Eliminar button is clicked', async () => {
    const onConfirm = jest.fn();
    render(
      <DeleteBookDialog
        open
        bookTitle="Mi libro"
        onConfirm={onConfirm}
        onCancel={jest.fn()}
      />,
    );
    await userEvent.click(screen.getByRole('button', { name: /eliminar/i }));
    expect(onConfirm).toHaveBeenCalledTimes(1);
  });

  it('calls onCancel when the Cancelar button is clicked', async () => {
    const onCancel = jest.fn();
    render(
      <DeleteBookDialog
        open
        bookTitle="Mi libro"
        onConfirm={jest.fn()}
        onCancel={onCancel}
      />,
    );
    await userEvent.click(screen.getByRole('button', { name: /cancelar/i }));
    expect(onCancel).toHaveBeenCalledTimes(1);
  });
});
