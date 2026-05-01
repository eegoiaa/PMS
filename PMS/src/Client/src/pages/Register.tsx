import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Box, Button, Card, CardContent, TextField, Typography, Alert, Link as MuiLink } from '@mui/material';
import { axiosClient } from '../api/axiosClient';

const registerSchema = z.object({
  fullName: z.string().min(2, 'Минимум 2 символа'),
  email: z.string().min(1, 'Email обязателен').email('Неверный формат email'),
  password: z.string().min(6, 'Пароль должен быть от 6 символов')
});

type RegisterFormInputs = z.infer<typeof registerSchema>;

export default function Register() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RegisterFormInputs>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormInputs) => {
    try {
      await axiosClient.post('/auth/register', data);
      navigate('/login');
    } catch (error: any) {
      setServerError(error.response?.data?.message || 'Ошибка регистрации');
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 3 }}>
      <Card sx={{ maxWidth: 400, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 'bold', textAlign: 'center', mb: 3 }}>
            Регистрация
          </Typography>

          {serverError && <Alert severity="error" sx={{ mb: 3 }}>{serverError}</Alert>}

          <form onSubmit={handleSubmit(onSubmit)}>
            <TextField fullWidth label="ФИО" sx={{ mb: 2 }} {...register('fullName')} error={!!errors.fullName} helperText={errors.fullName?.message} />
            <TextField fullWidth label="Email" sx={{ mb: 2 }} {...register('email')} error={!!errors.email} helperText={errors.email?.message} />
            <TextField fullWidth type="password" label="Пароль" sx={{ mb: 3 }} {...register('password')} error={!!errors.password} helperText={errors.password?.message} />

            <Button type="submit" variant="contained" color="primary" fullWidth size="large" disabled={isSubmitting}>
              Создать аккаунт
            </Button>

            <Typography sx={{ mt: 3, textAlign: 'center', color: 'text.secondary' }}>
              Уже есть аккаунт? <MuiLink component={Link} to="/login" color="primary">Войти</MuiLink>
            </Typography>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}