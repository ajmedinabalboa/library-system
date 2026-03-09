export interface Author {
  id: string;
  name: string;
}

export interface Book {
  id: string;
  title: string;
  publicationDate?: string;
  createdAt: string;
  updatedAt?: string;
  authors: Author[];
  authorCount: number;
}

export interface CreateBookDto {
  title: string;
  publicationDate?: string;
  authors: string[];
}

export interface UpdateBookDto {
  title: string;
  publicationDate?: string;
  authors: string[];
}

export interface BooksQuery {
  page?: number;
  pageSize?: number;
  title?: string;
  authors?: string[];
  publishedAfter?: string;
  publishedBefore?: string;
  minAuthors?: number;
  sortBy?: 'title' | 'publicationDate' | 'authorCount';
  sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
