'use client';

import { useState, useEffect, useCallback } from 'react';
import Autocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import { authorService } from '@/lib/api/authorService';
import type { Author } from '@/lib/types/book';

interface AuthorChipsProps {
  authors: string[];
  onAdd: (name: string) => void;
  onRemove: (name: string) => void;
}

export function AuthorChips({ authors, onAdd, onRemove }: AuthorChipsProps) {
  const [inputValue, setInputValue] = useState('');
  const [options, setOptions] = useState<Author[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchOptions = useCallback(async (query: string) => {
    setLoading(true);
    try {
      const results = await authorService.search(query);
      // Filter out authors already added
      setOptions(results.filter((a) => !authors.includes(a.name)));
    } catch {
      setOptions([]);
    } finally {
      setLoading(false);
    }
  }, [authors]);

  // Debounce input → search
  useEffect(() => {
    const timer = setTimeout(() => {
      fetchOptions(inputValue);
    }, 300);
    return () => clearTimeout(timer);
  }, [inputValue, fetchOptions]);

  const handleSelect = (_: unknown, value: Author | string | null) => {
    if (!value) return;
    const name = typeof value === 'string' ? value.trim() : value.name;
    if (name && !authors.includes(name)) {
      onAdd(name);
    }
    setInputValue('');
    setOptions([]);
  };

  return (
    <Box>
      <Autocomplete<Author, false, false, true>
        freeSolo
        options={options}
        getOptionLabel={(option) => (typeof option === 'string' ? option : option.name)}
        inputValue={inputValue}
        onInputChange={(_, value) => setInputValue(value)}
        onChange={handleSelect}
        loading={loading}
        noOptionsText="Sin resultados"
        loadingText="Buscando..."
        renderInput={(params) => (
          <TextField
            {...params}
            label="Buscar autor"
            placeholder="Escribe el nombre y selecciona o presiona Enter"
            size="small"
            InputProps={{
              ...params.InputProps,
              endAdornment: (
                <>
                  {loading && <CircularProgress color="inherit" size={16} />}
                  {params.InputProps.endAdornment}
                </>
              ),
            }}
          />
        )}
        renderOption={(props, option) => (
          <li {...props} key={typeof option === 'string' ? option : option.id}>
            {typeof option === 'string' ? option : option.name}
          </li>
        )}
      />

      {authors.length > 0 ? (
        <Stack direction="row" flexWrap="wrap" gap={1} mt={1.5}>
          {authors.map((author) => (
            <Chip
              key={author}
              label={author}
              onDelete={() => onRemove(author)}
              color="primary"
              variant="outlined"
            />
          ))}
        </Stack>
      ) : (
        <Typography variant="caption" color="text.secondary" display="block" mt={1}>
          Aún no se han agregado autores. Se requiere al menos uno.
        </Typography>
      )}
    </Box>
  );
}

