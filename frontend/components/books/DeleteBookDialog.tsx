import Dialog from '@mui/material/Dialog';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { WarningAmber as WarningIcon } from '@mui/icons-material';

interface DeleteBookDialogProps {
  open: boolean;
  bookTitle: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export function DeleteBookDialog({
  open,
  bookTitle,
  onConfirm,
  onCancel,
}: DeleteBookDialogProps) {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <WarningIcon color="warning" />
        Eliminar libro
      </DialogTitle>

      <DialogContent>
        <Typography>
          ¿Estás seguro de que deseas eliminar{' '}
          <strong>&ldquo;{bookTitle}&rdquo;</strong>? Esta acción no se puede
          deshacer.
        </Typography>
      </DialogContent>

      <DialogActions sx={{ px: 3, pb: 2, gap: 1 }}>
        <Button onClick={onCancel} variant="outlined" fullWidth>
          Cancelar
        </Button>
        <Button onClick={onConfirm} variant="contained" color="error" fullWidth autoFocus>
          Eliminar
        </Button>
      </DialogActions>
    </Dialog>
  );
}
