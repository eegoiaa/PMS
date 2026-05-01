import { useEffect, useState, useRef } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Box, Card, CardContent, Typography, Button, CircularProgress } from '@mui/material';
import { CheckCircleOutlined, ErrorOutlined } from '@mui/icons-material';
import { axiosClient } from '../api/axiosClient';

export default function ConfirmEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [errorMessage, setErrorMessage] = useState('');
  
  const hasRequested = useRef(false);

  useEffect(() => {
    const userId = searchParams.get('userId');
    const token = searchParams.get('token');

    if (!userId || !token) {
      setStatus('error');
      setErrorMessage('Некорректная или устаревшая ссылка подтверждения.');
      return;
    }

    if (hasRequested.current) return;
    hasRequested.current = true;

    axiosClient.post('/auth/confirm-email', { userId, token })
      .then(() => {
        setStatus('success');
      })
      .catch((error) => {
        setStatus('error');
        setErrorMessage(error.response?.data?.message || 'Произошла ошибка при подтверждении почты.');
      });
  }, [searchParams]);

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 3 }}>
      <Card sx={{ maxWidth: 450, width: '100%', textAlign: 'center' }}>
        <CardContent sx={{ p: 5 }}>
          
          {status === 'loading' && (
            <>
              <CircularProgress size={60} sx={{ color: 'primary.main', mb: 3 }} />
              <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                Подтверждаем почту...
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                Пожалуйста, подождите пару секунд.
              </Typography>
            </>
          )}

          {status === 'success' && (
            <>
              <CheckCircleOutlined sx={{ fontSize: 80, color: '#4caf50', mb: 2 }} />
              <Typography variant="h5" sx={{ fontWeight: 'bold', mb: 1 }}>
                Регистрация завершена!
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
                Ваша почта успешно подтверждена. Теперь вы можете войти в систему, используя свои данные.
              </Typography>
              <Button 
                variant="contained" 
                color="primary" 
                size="large" 
                fullWidth 
                onClick={() => navigate('/login')}
              >
                Перейти ко входу
              </Button>
            </>
          )}

          {status === 'error' && (
            <>
              <ErrorOutlined sx={{ fontSize: 80, color: '#f44336', mb: 2 }} />
              <Typography variant="h5" sx={{ fontWeight: 'bold', mb: 1 }}>
                Упс, что-то пошло не так
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
                {errorMessage}
              </Typography>
              <Button 
                variant="outlined" 
                color="primary" 
                size="large" 
                fullWidth 
                onClick={() => navigate('/login')}
              >
                Вернуться на страницу входа
              </Button>
            </>
          )}

        </CardContent>
      </Card>
    </Box>
  );
}