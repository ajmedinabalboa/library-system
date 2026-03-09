import React from 'react';
import { render, screen } from '../../test-utils';
import userEvent from '@testing-library/user-event';
import { BookFilters } from '@/components/books/BookFilters';
import type { BooksQuery } from '@/lib/types/book';

const defaultProps = {
  searchTitle: '',
  searchAuthors: [] as string[],
  publishedAfter: '',
  publishedBefore: '',
  sortBy: 'title' as NonNullable<BooksQuery['sortBy']>,
  sortOrder: 'asc' as NonNullable<BooksQuery['sortOrder']>,
  onSearchTitleChange: jest.fn(),
  onSearchAuthorsChange: jest.fn(),
  onPublishedAfterChange: jest.fn(),
  onPublishedBeforeChange: jest.fn(),
  onSortByChange: jest.fn(),
  onSortOrderChange: jest.fn(),
  onSearch: jest.fn(),
  onClear: jest.fn(),
};

describe('BookFilters', () => {
  it('renders the title search input', () => {
    render(<BookFilters {...defaultProps} />);
    expect(screen.getByLabelText(/buscar por título/i)).toBeInTheDocument();
  });

  it('renders the author text input', () => {
    render(<BookFilters {...defaultProps} />);
    expect(screen.getByLabelText(/agregar autor/i)).toBeInTheDocument();
  });

  it('calls onSearchAuthorsChange with the new author when Enter is pressed', async () => {
    const onSearchAuthorsChange = jest.fn();
    render(
      <BookFilters
        {...defaultProps}
        onSearchAuthorsChange={onSearchAuthorsChange}
      />,
    );
    const authorInput = screen.getByLabelText(/agregar autor/i);
    await userEvent.type(authorInput, 'Tolkien{Enter}');
    expect(onSearchAuthorsChange).toHaveBeenCalledWith(['Tolkien']);
  });

  it('displays existing author chips', () => {
    render(<BookFilters {...defaultProps} searchAuthors={['Garcia Marquez']} />);
    expect(screen.getByText('Garcia Marquez')).toBeInTheDocument();
  });

  it('"Limpiar" button is disabled when there are no active filters', () => {
    render(
      <BookFilters
        {...defaultProps}
        searchTitle=""
        searchAuthors={[]}
        publishedAfter=""
        publishedBefore=""
      />,
    );
    expect(screen.getByRole('button', { name: /limpiar/i })).toBeDisabled();
  });

  it('"Limpiar" button is enabled when a title filter is active', () => {
    render(<BookFilters {...defaultProps} searchTitle="Harry Potter" />);
    expect(screen.getByRole('button', { name: /limpiar/i })).toBeEnabled();
  });

  it('"Limpiar" button is enabled when author filters are active', () => {
    render(<BookFilters {...defaultProps} searchAuthors={['Rowling']} />);
    expect(screen.getByRole('button', { name: /limpiar/i })).toBeEnabled();
  });

  it('calls onSearch when the "Buscar" button is clicked', async () => {
    const onSearch = jest.fn();
    render(<BookFilters {...defaultProps} onSearch={onSearch} />);
    await userEvent.click(screen.getByRole('button', { name: /buscar/i }));
    expect(onSearch).toHaveBeenCalledTimes(1);
  });

  it('calls onClear when the "Limpiar" button is clicked', async () => {
    const onClear = jest.fn();
    render(
      <BookFilters {...defaultProps} searchTitle="algo" onClear={onClear} />,
    );
    await userEvent.click(screen.getByRole('button', { name: /limpiar/i }));
    expect(onClear).toHaveBeenCalledTimes(1);
  });
});
