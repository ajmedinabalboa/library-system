import api from './axios';
import {
  Book,
  CreateBookDto,
  UpdateBookDto,
  BooksQuery,
  PaginatedResult,
} from '@/lib/types/book';

export const bookService = {
  async getBooks(query: BooksQuery = {}): Promise<PaginatedResult<Book>> {
    const { data } = await api.get<PaginatedResult<Book>>('/books', {
      params: query,
    });
    return data;
  },

  async getBookById(id: string): Promise<Book> {
    const { data } = await api.get<Book>(`/books/${id}`);
    return data;
  },

  async createBook(book: CreateBookDto): Promise<Book> {
    const { data } = await api.post<Book>('/books', book);
    return data;
  },

  async updateBook(id: string, book: UpdateBookDto): Promise<Book> {
    const { data } = await api.put<Book>(`/books/${id}`, book);
    return data;
  },

  async deleteBook(id: string): Promise<void> {
    await api.delete(`/books/${id}`);
  },
};
