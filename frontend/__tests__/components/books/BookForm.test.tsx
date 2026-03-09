import React from 'react';
import { render, screen } from '../../test-utils';
import userEvent from '@testing-library/user-event';
import { BookForm } from '@/components/books/BookForm';

jest.mock('next/navigation', () => ({
  useRouter: jest.fn(() => ({ push: jest.fn() })),
}));

// Mock AuthorChips to avoid its async/debounce complexity
jest.mock('@/components/books/AuthorChips', () => ({
  AuthorChips: ({
    authors,
    onRemove,
  }: {
    authors: string[];
    onAdd: (n: string) => void;
    onRemove: (n: string) => void;
  }) => (
    <div data-testid="author-chips">
      {authors.map((a) => (
        <div key={a}>
          <span>{a}</span>
          <button type="button" onClick={() => onRemove(a)}>
            quitar {a}
          </button>
        </div>
      ))}
    </div>
  ),
}));

const defaultProps = {
  title: '',
  publicationDate: '',
  authors: [] as string[],
  error: '',
  saving: false,
  submitLabel: 'Guardar',
  onTitleChange: jest.fn(),
  onPublicationDateChange: jest.fn(),
  onAddAuthor: jest.fn(),
  onRemoveAuthor: jest.fn(),
  onSubmit: jest.fn((e: React.FormEvent) => e.preventDefault()),
};

describe('BookForm', () => {
  it('renders the title input', () => {
    render(<BookForm {...defaultProps} />);
    expect(screen.getByLabelText(/título del libro/i)).toBeInTheDocument();
  });

  it('submit button is disabled when the authors list is empty', () => {
    render(<BookForm {...defaultProps} authors={[]} />);
    expect(screen.getByRole('button', { name: /guardar/i })).toBeDisabled();
  });

  it('submit button is disabled when saving is true', () => {
    render(<BookForm {...defaultProps} authors={['Autor']} saving={true} />);
    expect(screen.getByRole('button', { name: /guardando/i })).toBeDisabled();
  });

  it('submit button is enabled when there are authors and saving is false', () => {
    render(<BookForm {...defaultProps} authors={['Autor']} saving={false} />);
    expect(screen.getByRole('button', { name: /guardar/i })).toBeEnabled();
  });

  it('renders an error alert when the error prop is provided', () => {
    render(<BookForm {...defaultProps} error="Algo salió mal" />);
    expect(screen.getByRole('alert')).toHaveTextContent('Algo salió mal');
  });

  it('does not render an error alert when the error prop is empty', () => {
    render(<BookForm {...defaultProps} error="" />);
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('calls onTitleChange when the title input value changes', async () => {
    const onTitleChange = jest.fn();
    render(<BookForm {...defaultProps} onTitleChange={onTitleChange} />);
    await userEvent.type(screen.getByLabelText(/título del libro/i), 'Mi Libro');
    expect(onTitleChange).toHaveBeenCalled();
  });

  it('renders existing authors from the authors prop', () => {
    render(<BookForm {...defaultProps} authors={['Garcia Marquez', 'Tolkien']} />);
    expect(screen.getByText('Garcia Marquez')).toBeInTheDocument();
    expect(screen.getByText('Tolkien')).toBeInTheDocument();
  });
});
