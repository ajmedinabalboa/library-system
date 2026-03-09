import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import { AutoStories as BookIcon } from '@mui/icons-material';
import Link from 'next/link';

interface EmptyStateProps {
  title?: string;
  description?: string;
  actionLabel?: string;
  actionHref?: string;
}

export function EmptyState({
  title = 'No se encontraron libros',
  description = 'Comienza agregando tu primer libro a la biblioteca',
  actionLabel = 'Agregar libro',
  actionHref = '/books/create',
}: EmptyStateProps) {
  return (
    <Box
      textAlign="center"
      py={10}
      display="flex"
      flexDirection="column"
      alignItems="center"
    >
      <BookIcon sx={{ fontSize: 72, color: 'text.disabled', mb: 2 }} />
      <Typography variant="h5" fontWeight={600} gutterBottom>
        {title}
      </Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        {description}
      </Typography>
      {actionHref && (
        <Button
          component={Link}
          href={actionHref}
          variant="contained"
          size="large"
        >
          {actionLabel}
        </Button>
      )}
    </Box>
  );
}
