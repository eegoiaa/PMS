import { useEffect, useState } from 'react';
import { Box, Typography, Button, Container, Card, CardContent } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { jwtDecode } from 'jwt-decode';
import { tasksApi } from '../api/tasksApi';
import { analyticsApi } from '../api/analyticsApi';

export default function Profile() {
  const navigate = useNavigate();
  // Убрали дефолтные значения, чтобы видеть реальную картину
  const [userEmail, setUserEmail] = useState<string>("Загрузка...");
  const [developerId, setDeveloperId] = useState<string | null>(null);

  // ПРАВИЛЬНАЯ РАСШИФРОВКА ТОКЕНА ASP.NET
  useEffect(() => {
    const tokenCookie = document.cookie.split('; ').find(row => row.startsWith('jwt='));
    if (tokenCookie) {
      const token = tokenCookie.split('=')[1];
      try {
        const decoded: any = jwtDecode(token);
        console.log("Расшифрованный токен (посмотри в консоли F12!):", decoded);
        
        // Динамически ищем ключ для Email
        const emailKey = Object.keys(decoded).find(k => k.toLowerCase().includes('email'));
        // Динамически ищем ключ для ID
        const idKey = Object.keys(decoded).find(k => k.toLowerCase().includes('nameidentifier') || k.toLowerCase() === 'sub');

        if (emailKey && decoded[emailKey]) {
          setUserEmail(decoded[emailKey]);
        } else {
          setUserEmail("Email не найден");
        }

        if (idKey && decoded[idKey]) {
          setDeveloperId(decoded[idKey]);
        } else {
          console.error("Критическая ошибка: ID разработчика не найден в токене!");
        }

      } catch (e) {
        console.error("Ошибка расшифровки токена", e);
      }
    }
  }, []);

  const { data: tasks } = useQuery({
    queryKey: ['tasks'],
    queryFn: tasksApi.getTasks
  });

  const { data: analytics } = useQuery({
    queryKey: ['analytics', developerId],
    queryFn: () => analyticsApi.getSummary(developerId!),
    enabled: !!developerId, // Запрос улетит только когда мы точно знаем ID
    retry: false 
  });

  const chartData = tasks?.map(task => ({
    name: task.taskKey,
    'План (ч)': task.planHours,
    'Факт (ч)': task.factHours
  })) || [];

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default', pt: 4, pb: 10 }}>
      <Container maxWidth="lg">
        
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 5 }}>
          <Button 
            variant="outlined" 
            color="inherit" 
            onClick={() => navigate('/dashboard')}
            sx={{ mr: 4, borderColor: 'rgba(255,255,255,0.2)' }}
          >
            &larr; Назад
          </Button>
          <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
            Аналитика разработчика <span style={{ color: '#FFD700' }}>.</span>
          </Typography>
        </Box>

        <Box sx={{ 
          display: 'grid', 
          gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, 
          gap: 4, 
          mb: 6 
        }}>
          <Card sx={{ bgcolor: 'background.paper', borderRadius: 4, height: '100%' }}>
            <CardContent sx={{ p: 4 }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Пользователь</Typography>
              <Typography variant="h6" sx={{ fontWeight: 'bold', wordBreak: 'break-all' }}>
                {userEmail}
              </Typography>
            </CardContent>
          </Card>
          
          {/* ИСПРАВЛЕНИЕ: Заменили borderTop на красивое свечение (boxShadow) */}
          <Card sx={{ 
            bgcolor: 'background.paper', 
            borderRadius: 4, 
            height: '100%', 
            border: '1px solid rgba(255, 215, 0, 0.3)',
            boxShadow: '0 8px 32px rgba(255, 215, 0, 0.15)' 
          }}>
            <CardContent sx={{ p: 4 }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Индекс волатильности</Typography>
              <Typography variant="h3" sx={{ fontWeight: 'bold', color: '#FFD700' }}>
                {analytics?.volatilityScore !== undefined ? analytics.volatilityScore.toFixed(2) : '--'}
              </Typography>
            </CardContent>
          </Card>

          <Card sx={{ bgcolor: 'background.paper', borderRadius: 4, height: '100%' }}>
            <CardContent sx={{ p: 4 }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Статус (Health)</Typography>
              <Typography variant="h5" sx={{ fontWeight: 'bold', textTransform: 'capitalize' }}>
                {analytics?.health || 'Неизвестно'}
              </Typography>
            </CardContent>
          </Card>
        </Box>

        <Card sx={{ borderRadius: 4, bgcolor: 'background.paper', p: 4 }}>
          <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 4 }}>
            Соотношение Плана и Факта по задачам
          </Typography>
          <Box sx={{ height: 400, width: '100%' }}>
            {chartData.length > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData} margin={{ top: 20, right: 30, left: 0, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                  <XAxis dataKey="name" stroke="#8884d8" />
                  <YAxis stroke="#8884d8" />
                  <Tooltip 
                    contentStyle={{ backgroundColor: '#1e1e1e', borderColor: '#333', borderRadius: '8px' }}
                    itemStyle={{ color: '#fff' }}
                  />
                  <Legend wrapperStyle={{ paddingTop: '20px' }} />
                  <Bar dataKey="План (ч)" fill="#8884d8" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="Факт (ч)" fill="#FFD700" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <Box sx={{ display: 'flex', height: '100%', alignItems: 'center', justifyContent: 'center' }}>
                <Typography color="text.secondary">Нет данных для отображения графика</Typography>
              </Box>
            )}
          </Box>
        </Card>

      </Container>
    </Box>
  );
}