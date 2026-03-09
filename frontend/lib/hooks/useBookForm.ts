'use client';

import { useState, useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { bookService } from '@/lib/api/bookService';

interface UseBookFormOptions {
  bookId?: string;
  onSuccess?: () => void;
}

export interface UseBookFormReturn {
  title: string;
  setTitle: (v: string) => void;
  publicationDate: string;
  setPublicationDate: (v: string) => void;
  authors: string[];
  addAuthor: (name: string) => void;
  removeAuthor: (name: string) => void;
  loading: boolean;
  saving: boolean;
  error: string;
  setError: (v: string) => void;
  handleSubmit: () => Promise<boolean>;
  isEditMode: boolean;
}

export function useBookForm({
  bookId,
  onSuccess,
}: UseBookFormOptions = {}): UseBookFormReturn {
  const router = useRouter();
  const isEditMode = !!bookId;

  const [title, setTitle] = useState('');
  const [publicationDate, setPublicationDate] = useState('');
  const [authors, setAuthors] = useState<string[]>([]);
  const [loading, setLoading] = useState(isEditMode);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!bookId) return;
    bookService
      .getBookById(bookId)
      .then((book) => {
        setTitle(book.title);
        setPublicationDate(
          book.publicationDate ? book.publicationDate.split('T')[0] : ''
        );
        setAuthors(book.authors.map((a) => a.name));
      })
      .catch(() => setError('Failed to load book'))
      .finally(() => setLoading(false));
  }, [bookId]);

  const addAuthor = useCallback(
    (name: string) => {
      const trimmed = name.trim();
      if (trimmed && !authors.includes(trimmed)) {
        setAuthors((prev) => [...prev, trimmed]);
      }
    },
    [authors]
  );

  const removeAuthor = useCallback((name: string) => {
    setAuthors((prev) => prev.filter((a) => a !== name));
  }, []);

  const handleSubmit = useCallback(async (): Promise<boolean> => {
    setError('');
    if (authors.length === 0) {
      setError('Please add at least one author');
      return false;
    }
    setSaving(true);
    try {
      const payload = {
        title,
        publicationDate: publicationDate || undefined,
        authors,
      };
      if (bookId) {
        await bookService.updateBook(bookId, payload);
      } else {
        await bookService.createBook(payload);
      }
      onSuccess?.();
      router.push('/books');
      return true;
    } catch (err: any) {
      setError(
        err.response?.data?.message ||
          `Failed to ${bookId ? 'update' : 'create'} book`
      );
      return false;
    } finally {
      setSaving(false);
    }
  }, [title, publicationDate, authors, bookId, router, onSuccess]);

  return {
    title,
    setTitle,
    publicationDate,
    setPublicationDate,
    authors,
    addAuthor,
    removeAuthor,
    loading,
    saving,
    error,
    setError,
    handleSubmit,
    isEditMode,
  };
}
