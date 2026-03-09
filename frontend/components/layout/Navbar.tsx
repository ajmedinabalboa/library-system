'use client';

import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import { AutoStories as LibraryIcon, Logout as LogoutIcon } from '@mui/icons-material';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/lib/context/AuthContext';

export function Navbar() {
  const router = useRouter();
  const { logout } = useAuth();

  const handleLogout = () => {
    logout();
    router.push('/login');
  };

  return (
    <AppBar position="sticky" elevation={2}>
      <Container maxWidth="xl">
        <Toolbar disableGutters>
          <LibraryIcon sx={{ mr: 1 }} />
          <Typography
            variant="h6"
            component="div"
            fontWeight={700}
            sx={{ flexGrow: 1, cursor: 'pointer', userSelect: 'none' }}
            onClick={() => router.push('/books')}
          >
            Sistema de Biblioteca
          </Typography>
          <Box>
            <Button
              color="inherit"
              startIcon={<LogoutIcon />}
              onClick={handleLogout}
            >
              Cerrar sesión
            </Button>
          </Box>
        </Toolbar>
      </Container>
    </AppBar>
  );
}
