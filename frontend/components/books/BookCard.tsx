import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';
import Tooltip from '@mui/material/Tooltip';
import Avatar from '@mui/material/Avatar';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  CalendarMonth as CalendarIcon,
  Group as GroupIcon,
  AutoStories as BookIcon,
} from '@mui/icons-material';
import Link from 'next/link';
import type { Book } from '@/lib/types/book';
import { useAuth } from '@/lib/context/AuthContext';

interface BookCardProps {
  book: Book;
  onDelete: (book: Book) => void;
}

/** Returns a deterministic color based on the first letter of the title */
function titleColor(title: string): string {
  const colors = ['#1565C0', '#283593', '#00695C', '#4527A0', '#6A1B9A', '#558B2F', '#E65100', '#37474F'];
  return colors[title.charCodeAt(0) % colors.length];
}

export function BookCard({ book, onDelete }: BookCardProps) {
  const { isAdmin } = useAuth();
  const accentColor = titleColor(book.title);
  const initials = book.title
    .split(' ')
    .slice(0, 2)
    .map((w) => w[0])
    .join('')
    .toUpperCase();

  const publicationYear = book.publicationDate
    ? new Date(book.publicationDate).getFullYear()
    : null;

  const formattedDate = book.publicationDate
    ? new Date(book.publicationDate).toLocaleDateString('es-ES', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
      })
    : null;

  return (
    <Card
      elevation={0}
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        border: '1px solid',
        borderColor: 'divider',
        transition: 'transform 0.18s ease, box-shadow 0.18s ease',
        '&:hover': {
          transform: 'translateY(-3px)',
          boxShadow: '0 8px 30px rgba(0,0,0,0.10)',
          borderColor: accentColor,
        },
        borderRadius: 3,
        overflow: 'hidden',
      }}
    >
      {/* Colored accent header strip */}
      <Box
        sx={{
          background: `linear-gradient(135deg, ${accentColor} 0%, ${accentColor}cc 100%)`,
          px: 2.5,
          pt: 2.5,
          pb: 3,
          display: 'flex',
          alignItems: 'flex-start',
          gap: 1.5,
        }}
      >
        <Avatar
          sx={{
            bgcolor: 'rgba(255,255,255,0.18)',
            color: '#fff',
            fontWeight: 700,
            fontSize: '0.9rem',
            width: 42,
            height: 42,
            flexShrink: 0,
            border: '1.5px solid rgba(255,255,255,0.4)',
          }}
        >
          {initials}
        </Avatar>

        <Box sx={{ minWidth: 0 }}>
          <Tooltip title={book.title} placement="top-start" enterDelay={600}>
            <Typography
              variant="subtitle1"
              component="h2"
              fontWeight={700}
              sx={{
                color: '#fff',
                lineHeight: 1.3,
                display: '-webkit-box',
                WebkitLineClamp: 3,
                WebkitBoxOrient: 'vertical',
                overflow: 'hidden',
                wordBreak: 'break-word',
              }}
            >
              {book.title}
            </Typography>
          </Tooltip>
          {publicationYear && (
            <Typography variant="caption" sx={{ color: 'rgba(255,255,255,0.75)', mt: 0.3, display: 'block' }}>
              {publicationYear}
            </Typography>
          )}
        </Box>
      </Box>

      {/* Body */}
      <CardContent sx={{ flexGrow: 1, px: 2.5, py: 2 }}>
        {/* Authors */}
        <Box display="flex" alignItems="flex-start" gap={1} mb={1.5}>
          <GroupIcon fontSize="small" sx={{ color: 'text.disabled', mt: '2px', flexShrink: 0 }} />
          <Box>
            <Typography variant="caption" color="text.disabled" fontWeight={600} sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
              Autores
            </Typography>
            <Box display="flex" flexWrap="wrap" gap={0.5} mt={0.4}>
              {book.authors.length > 0 ? (
                book.authors.map((a) => (
                  <Chip
                    key={a.id}
                    label={a.name}
                    size="small"
                    sx={{
                      bgcolor: `${accentColor}18`,
                      color: accentColor,
                      fontWeight: 500,
                      fontSize: '0.72rem',
                      height: 22,
                      border: 'none',
                    }}
                  />
                ))
              ) : (
                <Typography variant="body2" color="text.secondary">
                  Autor desconocido
                </Typography>
              )}
            </Box>
          </Box>
        </Box>

        {/* Publication date */}
        {formattedDate && (
          <Box display="flex" alignItems="center" gap={1}>
            <CalendarIcon fontSize="small" sx={{ color: 'text.disabled', flexShrink: 0 }} />
            <Box>
              <Typography variant="caption" color="text.disabled" fontWeight={600} sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
                Publicación
              </Typography>
              <Typography variant="body2" color="text.primary" sx={{ mt: 0.2 }}>
                {formattedDate}
              </Typography>
            </Box>
          </Box>
        )}
      </CardContent>

      {/* Footer stats bar */}
      <Box
        sx={{
          px: 2.5,
          py: 1,
          bgcolor: 'action.hover',
          display: 'flex',
          alignItems: 'center',
          gap: 0.5,
        }}
      >
        <BookIcon sx={{ fontSize: 14, color: 'text.disabled' }} />
        <Typography variant="caption" color="text.secondary">
          {book.authorCount} {book.authorCount === 1 ? 'autor' : 'autores'}
        </Typography>
      </Box>

      {isAdmin && (
        <>
          <Divider />
          <CardActions sx={{ px: 2, py: 1.5, gap: 1 }}>
            <Button
              component={Link}
              href={`/books/${book.id}/edit`}
              variant="outlined"
              startIcon={<EditIcon />}
              size="small"
              fullWidth
              sx={{ borderColor: accentColor, color: accentColor, '&:hover': { borderColor: accentColor, bgcolor: `${accentColor}10` } }}
            >
              Editar
            </Button>
            <Button
              onClick={() => onDelete(book)}
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              size="small"
              fullWidth
            >
              Eliminar
            </Button>
          </CardActions>
        </>
      )}
    </Card>
  );
}
