'use client';

import { useState, useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { bookService } from '@/lib/api/bookService';
import type { Book, BooksQuery } from '@/lib/types/book';

interface ActiveFilters {
  title: string;
  authors: string[];
  publishedAfter: string;
  publishedBefore: string;
}

export interface UseBooksReturn {
  books: Book[];
  loading: boolean;
  page: number;
  totalPages: number;
  totalCount: number;
  searchTitle: string;
  searchAuthors: string[];
  publishedAfter: string;
  publishedBefore: string;
  sortBy: BooksQuery['sortBy'];
  sortOrder: BooksQuery['sortOrder'];
  setPage: (page: number) => void;
  setSearchTitle: (title: string) => void;
  setSearchAuthors: (authors: string[]) => void;
  setPublishedAfter: (date: string) => void;
  setPublishedBefore: (date: string) => void;
  setSortBy: (sortBy: NonNullable<BooksQuery['sortBy']>) => void;
  setSortOrder: (order: NonNullable<BooksQuery['sortOrder']>) => void;
  handleSearch: () => void;
  handleClear: () => void;
  handleDelete: (id: string) => Promise<void>;
}

export function useBooks(): UseBooksReturn {
  const router = useRouter();
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const today = new Date().toISOString().split('T')[0];

  // Form state (lo que el usuario escribe)
  const [searchTitle, setSearchTitle] = useState('');
  const [searchAuthors, setSearchAuthors] = useState<string[]>([]);
  const [publishedAfter, setPublishedAfterRaw] = useState(today);
  const [publishedBefore, setPublishedBeforeRaw] = useState(today);

  // Constrained setters: enforce fecha-desde <= fecha-hasta
  const setPublishedAfter = useCallback((date: string) => {
    setPublishedAfterRaw(date);
    // If new "desde" is after the current "hasta", push "hasta" forward to match
    setPublishedBeforeRaw((prev) => (prev && date > prev ? date : prev));
  }, []);

  const setPublishedBefore = useCallback((date: string) => {
    setPublishedBeforeRaw(date);
    // If new "hasta" is before the current "desde", pull "desde" back to match
    setPublishedAfterRaw((prev) => (prev && date < prev ? date : prev));
  }, []);

  // Filtros activos (los que se envían a la API)
  // Empty dates on initial state = no date filter on first load (shows all books)
  const [activeFilters, setActiveFilters] = useState<ActiveFilters>({
    title: '',
    authors: [],
    publishedAfter: '',
    publishedBefore: '',
  });

  const [sortBy, setSortBy] = useState<NonNullable<BooksQuery['sortBy']>>('title');
  const [sortOrder, setSortOrder] = useState<NonNullable<BooksQuery['sortOrder']>>('asc');

  const fetchBooks = useCallback(
    async (
      currentPage: number,
      filters: ActiveFilters,
      sort: NonNullable<BooksQuery['sortBy']>,
      order: NonNullable<BooksQuery['sortOrder']>
    ) => {
      setLoading(true);
      try {
        const query: BooksQuery = {
          page: currentPage,
          pageSize: 9,
          sortBy: sort,
          sortOrder: order,
          ...(filters.title ? { title: filters.title } : {}),
          ...(filters.authors.length > 0 ? { authors: filters.authors } : {}),
          ...(filters.publishedAfter ? { publishedAfter: filters.publishedAfter } : {}),
          ...(filters.publishedBefore ? { publishedBefore: filters.publishedBefore } : {}),
        };
        const data = await bookService.getBooks(query);
        setBooks(data.items);
        setTotalPages(data.totalPages);
        setTotalCount(data.totalCount);
      } catch (error: any) {
        if (error.response?.status === 401) {
          router.push('/login');
        } else {
          // Clear stale results so the user doesn't see wrong data
          setBooks([]);
          setTotalPages(1);
          setTotalCount(0);
        }
      } finally {
        setLoading(false);
      }
    },
    [router]
  );

  useEffect(() => {
    fetchBooks(page, activeFilters, sortBy, sortOrder);
  }, [page, activeFilters, sortBy, sortOrder]); // eslint-disable-line react-hooks/exhaustive-deps

  const handleSearch = useCallback(() => {
    const newFilters: ActiveFilters = {
      title: searchTitle,
      authors: searchAuthors,
      publishedAfter,
      publishedBefore,
    };
    setPage(1);
    setActiveFilters(newFilters);
  }, [searchTitle, searchAuthors, publishedAfter, publishedBefore]);

  const handleClear = useCallback(() => {
    const todayVal = new Date().toISOString().split('T')[0];
    setSearchTitle('');
    setSearchAuthors([]);
    setPublishedAfterRaw(todayVal);
    setPublishedBeforeRaw(todayVal);
    setPage(1);
    setActiveFilters({ title: '', authors: [], publishedAfter: todayVal, publishedBefore: todayVal });
  }, []);

  const handleDelete = useCallback(
    async (id: string) => {
      await bookService.deleteBook(id);
      fetchBooks(page, activeFilters, sortBy, sortOrder);
    },
    [page, activeFilters, sortBy, sortOrder, fetchBooks]
  );

  return {
    books,
    loading,
    page,
    totalPages,
    totalCount,
    searchTitle,
    searchAuthors,
    publishedAfter,
    publishedBefore,
    sortBy,
    sortOrder,
    setPage,
    setSearchTitle,
    setSearchAuthors,
    setPublishedAfter,
    setPublishedBefore,
    setSortBy,
    setSortOrder,
    handleSearch,
    handleClear,
    handleDelete,
  };
}
