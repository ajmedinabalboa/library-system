'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import InputAdornment from '@mui/material/InputAdornment';
import IconButton from '@mui/material/IconButton';
import {
  Visibility,
  VisibilityOff,
  AutoStories as LibraryIcon,
} from '@mui/icons-material';
import { useAuth } from '@/lib/context/AuthContext';

export default function LoginPage() {
  const router = useRouter();
  const { login } = useAuth();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login({ email, password });
      router.push('/books');
    } catch (err: any) {
      setError(
        err.response?.data?.message ||
          'Error al iniciar sesión. Verifica tus credenciales.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      sx={{
        background:
          'linear-gradient(135deg, #e3f2fd 0%, #e8eaf6 100%)',
      }}
    >
      <Card sx={{ width: '100%', maxWidth: 420, mx: 2 }} elevation={6}>
        <CardContent sx={{ p: 4 }}>
          <Box textAlign="center" mb={4}>
            <LibraryIcon color="primary" sx={{ fontSize: 52, mb: 1 }} />
            <Typography variant="h4" fontWeight={700}>
              Sistema de Biblioteca
            </Typography>
            <Typography variant="body2" color="text.secondary" mt={1}>
              Inicia sesión para gestionar tus libros
            </Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          <Box
            component="form"
            onSubmit={handleSubmit}
            display="flex"
            flexDirection="column"
            gap={2.5}
          >
            <TextField
              label="Correo electrónico"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              fullWidth
              autoComplete="email"
              autoFocus
              size="medium"
            />

            <TextField
              label="Contraseña"
              type={showPassword ? 'text' : 'password'}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              fullWidth
              autoComplete="current-password"
              size="medium"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword((v) => !v)}
                      edge="end"
                      aria-label={
                        showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'
                      }
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            <Button
              type="submit"
              variant="contained"
              size="large"
              fullWidth
              disabled={loading}
              sx={{ mt: 1 }}
            >
              {loading ? (
                <CircularProgress size={24} color="inherit" />
              ) : (
                'Iniciar sesión'
              )}
            </Button>
          </Box>

          <Box mt={3} textAlign="center">
            <Typography variant="caption" color="text.secondary">
              Demo: admin@library.com / Admin123!
            </Typography>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
