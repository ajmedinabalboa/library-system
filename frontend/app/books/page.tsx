'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Pagination from '@mui/material/Pagination';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { Add as AddIcon } from '@mui/icons-material';
import Link from 'next/link';
import { Navbar } from '@/components/layout/Navbar';
import { BookCard } from '@/components/books/BookCard';
import { BookFilters } from '@/components/books/BookFilters';
import { DeleteBookDialog } from '@/components/books/DeleteBookDialog';
import { PageLoader } from '@/components/common/PageLoader';
import { EmptyState } from '@/components/common/EmptyState';
import { useAuth } from '@/lib/context/AuthContext';
import { useSnackbar } from '@/lib/context/SnackbarContext';
import { useBooks } from '@/lib/hooks/useBooks';
import type { Book } from '@/lib/types/book';

export default function BooksPage() {
  const router = useRouter();
  const { isAuthenticated, isAdmin } = useAuth();
  const { showSuccess, showError } = useSnackbar();
  const {
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
  } = useBooks();

  const [bookToDelete, setBookToDelete] = useState<Book | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    if (!isAuthenticated) router.push('/login');
  }, [isAuthenticated, router]);

  const confirmDelete = async () => {
    if (!bookToDelete) return;
    setDeleting(true);
    try {
      await handleDelete(bookToDelete.id);
      showSuccess(`"${bookToDelete.title}" eliminado correctamente`);
    } catch {
      showError('Error al eliminar el libro. Por favor intenta de nuevo.');
    } finally {
      setDeleting(false);
      setBookToDelete(null);
    }
  };

  return (
    <>
      <Navbar />
      <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', py: 4 }}>
        <Container maxWidth="xl">
          {/* Header */}
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={4}
          >
            <Box>
              <Typography variant="h4" fontWeight={700}>
                Biblioteca de libros
              </Typography>
              <Typography variant="body2" color="text.secondary" mt={0.5}>
                {totalCount} {totalCount === 1 ? 'libro' : 'libros'} en total
              </Typography>
            </Box>
            {isAdmin && (
              <Button
                component={Link}
                href="/books/create"
                variant="contained"
                startIcon={<AddIcon />}
                size="large"
              >
                Agregar nuevo libro
              </Button>
            )}
          </Box>

          {/* Filters */}
          <Paper elevation={1} sx={{ p: 3, mb: 4 }}>
            <BookFilters
              searchTitle={searchTitle}
              searchAuthors={searchAuthors}
              publishedAfter={publishedAfter}
              publishedBefore={publishedBefore}
              sortBy={sortBy ?? 'title'}
              sortOrder={sortOrder ?? 'asc'}
              onSearchTitleChange={setSearchTitle}
              onSearchAuthorsChange={setSearchAuthors}
              onPublishedAfterChange={setPublishedAfter}
              onPublishedBeforeChange={setPublishedBefore}
              onSortByChange={setSortBy}
              onSortOrderChange={setSortOrder}
              onSearch={handleSearch}
              onClear={handleClear}
            />
          </Paper>

          {/* Content */}
          {loading ? (
            <PageLoader message="Cargando libros..." />
          ) : books.length === 0 ? (
            <EmptyState />
          ) : (
            <>
              <Grid container spacing={3}>
                {books.map((book) => (
                  <Grid size={{ xs: 12, sm: 6, lg: 4 }} key={book.id}>
                    <BookCard book={book} onDelete={setBookToDelete} />
                  </Grid>
                ))}
              </Grid>

              {totalPages > 1 && (
                <Box display="flex" justifyContent="center" mt={5}>
                  <Pagination
                    count={totalPages}
                    page={page}
                    onChange={(_, value) => setPage(value)}
                    color="primary"
                    size="large"
                    showFirstButton
                    showLastButton
                  />
                </Box>
              )}
            </>
          )}
        </Container>
      </Box>

      <DeleteBookDialog
        open={!!bookToDelete}
        bookTitle={bookToDelete?.title ?? ''}
        onConfirm={confirmDelete}
        onCancel={() => !deleting && setBookToDelete(null)}
      />
    </>
  );
}
