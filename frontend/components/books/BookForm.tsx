'use client';

import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Alert from '@mui/material/Alert';
import Stack from '@mui/material/Stack';
import CircularProgress from '@mui/material/CircularProgress';
import {
  Save as SaveIcon,
  ArrowBack as BackIcon,
} from '@mui/icons-material';
import { useRouter } from 'next/navigation';
import { AuthorChips } from './AuthorChips';

interface BookFormProps {
  title: string;
  publicationDate: string;
  authors: string[];
  error: string;
  saving: boolean;
  submitLabel: string;
  onTitleChange: (value: string) => void;
  onPublicationDateChange: (value: string) => void;
  onAddAuthor: (name: string) => void;
  onRemoveAuthor: (name: string) => void;
  onSubmit: (e: React.FormEvent) => void;
}

export function BookForm({
  title,
  publicationDate,
  authors,
  error,
  saving,
  submitLabel,
  onTitleChange,
  onPublicationDateChange,
  onAddAuthor,
  onRemoveAuthor,
  onSubmit,
}: BookFormProps) {
  const router = useRouter();

  return (
    <Paper elevation={2} sx={{ p: 4 }}>
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      <Box
        component="form"
        onSubmit={onSubmit}
        onKeyDown={(e: React.KeyboardEvent<HTMLFormElement>) => {
          if (e.key === 'Enter' && (e.target as HTMLElement).tagName !== 'BUTTON') {
            e.preventDefault();
          }
        }}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        <TextField
          label="Título del libro"
          value={title}
          onChange={(e) => onTitleChange(e.target.value)}
          required
          fullWidth
          placeholder="Ingresa el título del libro"
          size="medium"
        />

        <TextField
          label="Fecha de publicación (opcional)"
          type="date"
          value={publicationDate}
          onChange={(e) => onPublicationDateChange(e.target.value)}
          fullWidth
          InputLabelProps={{ shrink: true }}
          size="medium"
        />

        <Box>
          <Typography variant="subtitle2" fontWeight={600} mb={1.5}>
            Autores *
          </Typography>
          <AuthorChips
            authors={authors}
            onAdd={onAddAuthor}
            onRemove={onRemoveAuthor}
          />
        </Box>

        <Stack direction={{ xs: 'column', sm: 'row' }} gap={2} pt={1}>
          <Button
            type="submit"
            variant="contained"
            size="large"
            startIcon={
              saving ? (
                <CircularProgress size={18} color="inherit" />
              ) : (
                <SaveIcon />
              )
            }
            disabled={saving || authors.length === 0}
            fullWidth
          >
            {saving ? 'Guardando...' : submitLabel}
          </Button>
          <Button
            type="button"
            variant="outlined"
            size="large"
            startIcon={<BackIcon />}
            onClick={() => router.push('/books')}
            fullWidth
          >
            Cancelar
          </Button>
        </Stack>
      </Box>
    </Paper>
  );
}
