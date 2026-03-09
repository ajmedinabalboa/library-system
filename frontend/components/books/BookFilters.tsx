import { useState } from 'react';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Button from '@mui/material/Button';
import InputAdornment from '@mui/material/InputAdornment';
import Grid from '@mui/material/Grid';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';
import Chip from '@mui/material/Chip';
import {
  Search as SearchIcon,
  FilterAlt as FilterIcon,
  ClearAll as ClearIcon,
  Person as PersonIcon,
  CalendarMonth as CalendarIcon,
  Sort as SortIcon,
} from '@mui/icons-material';
import type { BooksQuery } from '@/lib/types/book';

interface BookFiltersProps {
  searchTitle: string;
  searchAuthors: string[];
  publishedAfter: string;
  publishedBefore: string;
  sortBy: NonNullable<BooksQuery['sortBy']>;
  sortOrder: NonNullable<BooksQuery['sortOrder']>;
  onSearchTitleChange: (value: string) => void;
  onSearchAuthorsChange: (authors: string[]) => void;
  onPublishedAfterChange: (value: string) => void;
  onPublishedBeforeChange: (value: string) => void;
  onSortByChange: (value: NonNullable<BooksQuery['sortBy']>) => void;
  onSortOrderChange: (value: NonNullable<BooksQuery['sortOrder']>) => void;
  onSearch: () => void;
  onClear: () => void;
}

export function BookFilters({
  searchTitle,
  searchAuthors,
  publishedAfter,
  publishedBefore,
  sortBy,
  sortOrder,
  onSearchTitleChange,
  onSearchAuthorsChange,
  onPublishedAfterChange,
  onPublishedBeforeChange,
  onSortByChange,
  onSortOrderChange,
  onSearch,
  onClear,
}: BookFiltersProps) {
  const [authorInput, setAuthorInput] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch();
  };

  const handleAuthorKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if ((e.key === 'Enter' || e.key === ',') && authorInput.trim()) {
      e.preventDefault();
      const newAuthor = authorInput.trim().replace(/,+$/, '');
      if (newAuthor && !searchAuthors.includes(newAuthor)) {
        onSearchAuthorsChange([...searchAuthors, newAuthor]);
      }
      setAuthorInput('');
    }
  };

  const handleRemoveAuthor = (author: string) => {
    onSearchAuthorsChange(searchAuthors.filter((a) => a !== author));
  };

  const handleClearInternal = () => {
    setAuthorInput('');
    onClear();
  };

  const hasActiveFilters =
    !!searchTitle || searchAuthors.length > 0 || !!publishedAfter || !!publishedBefore;

  return (
    <Box component="form" onSubmit={handleSubmit}>
      {/* Sección: Búsqueda */}
      <Box display="flex" alignItems="center" gap={1} mb={2}>
        <FilterIcon fontSize="small" color="action" />
        <Typography variant="subtitle2" color="text.secondary" fontWeight={600}>
          Filtros de búsqueda
        </Typography>
      </Box>

      <Grid container spacing={2}>
        {/* Título */}
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <TextField
            label="Buscar por título"
            value={searchTitle}
            onChange={(e) => onSearchTitleChange(e.target.value)}
            fullWidth
            size="small"
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        {/* Autores (chip input) */}
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <TextField
            label="Agregar autor"
            value={authorInput}
            onChange={(e) => setAuthorInput(e.target.value)}
            onKeyDown={handleAuthorKeyDown}
            fullWidth
            size="small"
            placeholder="Escribe y presiona Enter o ,"
            helperText="Busca por múltiples autores"
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <PersonIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
          {searchAuthors.length > 0 && (
            <Box display="flex" flexWrap="wrap" gap={0.5} mt={0.5}>
              {searchAuthors.map((author) => (
                <Chip
                  key={author}
                  label={author}
                  size="small"
                  onDelete={() => handleRemoveAuthor(author)}
                  color="primary"
                  variant="outlined"
                />
              ))}
            </Box>
          )}
        </Grid>

        {/* Fecha desde */}
        <Grid size={{ xs: 6, md: 2 }}>
          <TextField
            label="Publicado desde"
            type="date"
            value={publishedAfter}
            onChange={(e) => onPublishedAfterChange(e.target.value)}
            fullWidth
            size="small"
            InputLabelProps={{ shrink: true }}
            inputProps={{ max: publishedBefore || undefined }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <CalendarIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        {/* Fecha hasta */}
        <Grid size={{ xs: 6, md: 2 }}>
          <TextField
            label="Publicado hasta"
            type="date"
            value={publishedBefore}
            onChange={(e) => onPublishedBeforeChange(e.target.value)}
            fullWidth
            size="small"
            InputLabelProps={{ shrink: true }}
            inputProps={{ min: publishedAfter || undefined }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <CalendarIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
        </Grid>
      </Grid>

      <Divider sx={{ my: 2 }} />

      {/* Sección: Ordenamiento y acciones */}
      <Grid container spacing={2} alignItems="flex-end">
        <Grid size={{ xs: 6, sm: 4, md: 3 }}>
          <FormControl fullWidth size="small">
            <InputLabel>Ordenar por</InputLabel>
            <Select
              value={sortBy}
              label="Ordenar por"
              startAdornment={
                <InputAdornment position="start">
                  <SortIcon fontSize="small" />
                </InputAdornment>
              }
              onChange={(e) =>
                onSortByChange(e.target.value as NonNullable<BooksQuery['sortBy']>)
              }
            >
              <MenuItem value="title">Título</MenuItem>
              <MenuItem value="publicationDate">Publicación</MenuItem>
              <MenuItem value="authorCount">Autores</MenuItem>
            </Select>
          </FormControl>
        </Grid>

        <Grid size={{ xs: 6, sm: 4, md: 3 }}>
          <FormControl fullWidth size="small">
            <InputLabel>Orden</InputLabel>
            <Select
              value={sortOrder}
              label="Orden"
              onChange={(e) =>
                onSortOrderChange(
                  e.target.value as NonNullable<BooksQuery['sortOrder']>
                )
              }
            >
              <MenuItem value="asc">Ascendente</MenuItem>
              <MenuItem value="desc">Descendente</MenuItem>
            </Select>
          </FormControl>
        </Grid>

        <Grid size={{ xs: 12, sm: 4, md: 3 }} sx={{ ml: { md: 'auto' } }}>
          <Box display="flex" gap={1}>
            <Button
              type="submit"
              variant="contained"
              fullWidth
              startIcon={<SearchIcon />}
            >
              Buscar
            </Button>
            <Button
              type="button"
              variant="outlined"
              fullWidth
              startIcon={<ClearIcon />}
              onClick={handleClearInternal}
              disabled={!hasActiveFilters}
              color="inherit"
            >
              Limpiar
            </Button>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
}
