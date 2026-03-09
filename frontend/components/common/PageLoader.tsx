import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';

interface PageLoaderProps {
  message?: string;
}

export function PageLoader({ message = 'Cargando...' }: PageLoaderProps) {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      py={10}
    >
      <CircularProgress size={48} />
      <Typography variant="body2" color="text.secondary" mt={2}>
        {message}
      </Typography>
    </Box>
  );
}
