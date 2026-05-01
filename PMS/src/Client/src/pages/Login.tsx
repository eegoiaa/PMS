import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Box, Button, Card, CardContent, TextField, Typography, Alert } from '@mui/material';
import { axiosClient } from '../api/axiosClient';

const loginSchema = z.object({
  email: z.string().min(1, 'Email обязателен').email('Неверный формат email'),
  password: z.string().min(1, 'Пароль обязателен')
});

type LoginFormInputs = z.infer<typeof loginSchema>;

export default function Login() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormInputs>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormInputs) => {
    setServerError(null);
    try {
      await axiosClient.post('/auth/login', {
        email: data.email,
        password: data.password
      });
      
      navigate('/dashboard');
    } catch (error: any) {
      console.error('Login error:', error);
      if (error.response?.data?.message) {
        setServerError(error.response.data.message);
      } else {
        setServerError('Ошибка соединения с сервером. Проверьте Gateway.');
      }
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 3 }}>
      <Card sx={{ maxWidth: 400, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 'bold', textAlign: 'center', mb: 3 }}>
            Вход в систему
          </Typography>

          {serverError && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {serverError}
            </Alert>
          )}

          <form onSubmit={handleSubmit(onSubmit)}>
            <TextField
              fullWidth
              label="Email"
              variant="outlined"
              sx={{ mb: 3 }}
              {...register('email')} 
              error={!!errors.email}
              helperText={errors.email?.message} 
            />

            <TextField
              fullWidth
              type="password"
              label="Пароль"
              variant="outlined"
              sx={{ mb: 4 }}
              {...register('password')}
              error={!!errors.password}
              helperText={errors.password?.message}
            />

            <Button 
              type="submit" 
              variant="contained" 
              color="primary" 
              fullWidth 
              size="large"
              disabled={isSubmitting} 
            >
              {isSubmitting ? 'Вход...' : 'Войти'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}