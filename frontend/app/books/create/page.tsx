'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import { Navbar } from '@/components/layout/Navbar';
import { BookForm } from '@/components/books/BookForm';
import { useAuth } from '@/lib/context/AuthContext';
import { useSnackbar } from '@/lib/context/SnackbarContext';
import { useBookForm } from '@/lib/hooks/useBookForm';

export default function CreateBookPage() {
  const router = useRouter();
  const { isAuthenticated, isAdmin } = useAuth();
  const { showSuccess } = useSnackbar();
  const {
    title,
    setTitle,
    publicationDate,
    setPublicationDate,
    authors,
    addAuthor,
    removeAuthor,
    error,
    saving,
    handleSubmit,
  } = useBookForm({
    onSuccess: () => showSuccess('¡Libro creado correctamente!'),
  });

  useEffect(() => {
    if (!isAuthenticated) router.push('/login');
    else if (!isAdmin) router.push('/books');
  }, [isAuthenticated, isAdmin, router]);

  return (
    <>
      <Navbar />
      <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', py: 4 }}>
        <Container maxWidth="md">
          <Box mb={4}>
            <Typography variant="h4" fontWeight={700}>
              Agregar nuevo libro
            </Typography>
            <Typography variant="body2" color="text.secondary" mt={0.5}>
              Completa los datos para agregar un nuevo libro a la biblioteca
            </Typography>
          </Box>

          <BookForm
            title={title}
            publicationDate={publicationDate}
            authors={authors}
            error={error}
            saving={saving}
            submitLabel="Crear libro"
            onTitleChange={setTitle}
            onPublicationDateChange={setPublicationDate}
            onAddAuthor={addAuthor}
            onRemoveAuthor={removeAuthor}
            onSubmit={(e) => {
              e.preventDefault();
              handleSubmit();
            }}
          />
        </Container>
      </Box>
    </>
  );
}
