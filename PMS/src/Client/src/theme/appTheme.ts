import { createTheme } from '@mui/material/styles';

export const appTheme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#FFD700',
      contrastText: '#1A1A1A', 
    },
    background: {
      default: '#1A1A1A',
      paper: '#242424', 
    },
    text: {
      primary: '#F3F3F3', 
      secondary: '#A0A0A0', 
    },
  },
  shape: {
    borderRadius: 16,
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    button: {
      textTransform: 'none', 
      fontWeight: 600,
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          padding: '10px 24px',
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: 'none', 
          boxShadow: '0 8px 24px rgba(0,0,0,0.2)', 
        },
      },
    },
  },
});