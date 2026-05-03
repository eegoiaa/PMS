import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Box, Button, Card, CardContent, TextField, Typography, Alert, Link as MuiLink, Snackbar } from '@mui/material';
import { axiosClient } from '../api/axiosClient';

const registerSchema = z.object({
  fullName: z.string().min(2, 'Минимум 2 символа'),
  email: z.string().min(1, 'Email обязателен').email('Неверный формат email'),
  password: z.string().min(6, 'Пароль должен быть от 6 символов'),
  confirmPassword: z.string().min(1, 'Подтверждение пароля обязательно')
}).refine((data) => data.password === data.confirmPassword, {
  message: "Пароли не совпадают",
  path: ["confirmPassword"],
});

type RegisterFormInputs = z.infer<typeof registerSchema>;

export default function Register() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);
  
  const [showSuccess, setShowSuccess] = useState(false);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RegisterFormInputs>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormInputs) => {
    setServerError(null);
    try {
      await axiosClient.post('/auth/sign-up', {
        fullName: data.fullName,
        email: data.email,
        password: data.password,
        confirmPassword: data.confirmPassword
      });
      
      setShowSuccess(true);
      
      setTimeout(() => {
        navigate('/login');
      }, 3500);

    } catch (error: any) {
      setServerError(error.response?.data?.message || 'Ошибка регистрации');
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 3 }}>
      <Button 
          onClick={() => navigate('/')} 
          sx={{ 
            position: 'absolute', 
            top: 24, 
            left: 24, 
            color: 'text.secondary',
            textTransform: 'none',
            fontSize: '1rem'
          }}
        >
          &larr; На главную
        </Button>
      <Card sx={{ maxWidth: 400, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 'bold', textAlign: 'center', mb: 3 }}>
            Регистрация
          </Typography>

          {serverError && <Alert severity="error" sx={{ mb: 3 }}>{serverError}</Alert>}

          <form onSubmit={handleSubmit(onSubmit)}>
            <TextField fullWidth label="ФИО" sx={{ mb: 2 }} {...register('fullName')} error={!!errors.fullName} helperText={errors.fullName?.message} />
            <TextField fullWidth label="Email" sx={{ mb: 2 }} {...register('email')} error={!!errors.email} helperText={errors.email?.message} />
            <TextField fullWidth type="password" label="Пароль" sx={{ mb: 2 }} {...register('password')} error={!!errors.password} helperText={errors.password?.message} />
            <TextField fullWidth type="password" label="Подтвердите пароль" sx={{ mb: 3 }} {...register('confirmPassword')} error={!!errors.confirmPassword} helperText={errors.confirmPassword?.message} />

            <Button type="submit" variant="contained" color="primary" fullWidth size="large" disabled={isSubmitting || showSuccess}>
              {showSuccess ? 'Готово!' : 'Создать аккаунт'}
            </Button>

            <Typography sx={{ mt: 3, textAlign: 'center', color: 'text.secondary' }}>
              Уже есть аккаунт? <MuiLink component={Link} to="/login" color="primary">Войти</MuiLink>
            </Typography>
          </form>
        </CardContent>
      </Card>

      <Snackbar 
        open={showSuccess} 
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert severity="success" variant="filled" sx={{ width: '100%', borderRadius: 2 }}>
          Регистрация прошла успешно! Проверьте почту для подтверждения. Перенаправляем на вход...
        </Alert>
      </Snackbar>
    </Box>
  );
}