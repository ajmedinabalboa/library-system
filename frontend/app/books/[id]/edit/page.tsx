'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import { Navbar } from '@/components/layout/Navbar';
import { BookForm } from '@/components/books/BookForm';
import { PageLoader } from '@/components/common/PageLoader';
import { useAuth } from '@/lib/context/AuthContext';
import { useSnackbar } from '@/lib/context/SnackbarContext';
import { useBookForm } from '@/lib/hooks/useBookForm';

export default function EditBookPage({ params }: { params: { id: string } }) {
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
    loading,
    saving,
    handleSubmit,
  } = useBookForm({
    bookId: params.id,
    onSuccess: () => showSuccess('¡Libro actualizado correctamente!'),
  });

  useEffect(() => {
    if (!isAuthenticated) router.push('/login');
    else if (!isAdmin) router.push('/books');
  }, [isAuthenticated, isAdmin, router]);

  if (loading) {
    return (
      <>
        <Navbar />
        <PageLoader message="Cargando libro..." />
      </>
    );
  }

  return (
    <>
      <Navbar />
      <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', py: 4 }}>
        <Container maxWidth="md">
          <Box mb={4}>
            <Typography variant="h4" fontWeight={700}>
              Editar libro
            </Typography>
            <Typography variant="body2" color="text.secondary" mt={0.5}>
              Actualiza los datos de este libro
            </Typography>
          </Box>

          <BookForm
            title={title}
            publicationDate={publicationDate}
            authors={authors}
            error={error}
            saving={saving}
            submitLabel="Guardar cambios"
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
