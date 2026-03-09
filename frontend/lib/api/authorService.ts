import api from './axios';
import type { Author } from '@/lib/types/book';

export const authorService = {
  async search(query: string): Promise<Author[]> {
    const { data } = await api.get<Author[]>('/authors', {
      params: query.trim() ? { search: query.trim() } : undefined,
    });
    return data;
  },
};
