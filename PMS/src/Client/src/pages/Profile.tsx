import { useEffect, useState } from 'react';
import { Box, Typography, Button, Container, Card, CardContent } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { tasksApi } from '../api/tasksApi';
import { analyticsApi } from '../api/analyticsApi';

export default function Profile() {
  const navigate = useNavigate();
  const [userEmail, setUserEmail] = useState<string>("Загрузка...");
  const [developerId, setDeveloperId] = useState<string | null>(null);

  useEffect(() => {
    const savedEmail = localStorage.getItem('userEmail');
    const savedId = localStorage.getItem('developerId');

    if (savedEmail) setUserEmail(savedEmail);
    if (savedId) setDeveloperId(savedId);
  }, []);

  const { data: tasks } = useQuery({
    queryKey: ['tasks'],
    queryFn: tasksApi.getTasks
  });

  const { data: analytics } = useQuery({
    queryKey: ['analytics', developerId],
    queryFn: () => analyticsApi.getSummary(developerId!),
    enabled: !!developerId,
    retry: false 
  });

  const chartData = tasks?.map(task => ({
    name: task.taskKey,
    'План (ч)': task.planHours,
    'Факт (ч)': task.factHours
  })) || [];

  const getHealthStatus = (volatility: number | undefined) => {
    if (volatility === undefined) return 'Неизвестно';
    if (volatility > 0.5) return 'Перегруз ⚠️'; 
    if (volatility < -0.5) return 'Слишком быстро ⚡'; 
    return 'В норме 🟢'; 
  };

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
            Аналитика разработчика <span style={{ color: '#FFD700' }}></span>
          </Typography>
        </Box>

        <Box sx={{ 
          display: 'grid', 
          gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, 
          gap: 4, 
          mb: 6 
        }}>
          
          <Card sx={{ bgcolor: 'background.paper', borderRadius: 4 }}>
            <CardContent sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Разработчик</Typography>
              <Typography variant="h6" sx={{ fontWeight: 'bold', wordBreak: 'break-all' }}>
                {analytics?.fullName || userEmail}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
                Активных задач: {analytics?.activeTasksCount || 0}
              </Typography>
            </CardContent>
          </Card>
          
          <Card sx={{ 
            bgcolor: 'background.paper', 
            borderRadius: 4, 
            border: '1px solid rgba(255, 215, 0, 0.3)',
            boxShadow: '0 8px 32px rgba(255, 215, 0, 0.15)' 
          }}>
            <CardContent sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Индекс волатильности</Typography>
              <Typography variant="h4" sx={{ fontWeight: 'bold', color: '#FFD700' }}>
                {analytics?.averageVolatility !== undefined 
                  ? analytics.averageVolatility.toFixed(3) 
                  : '--'}
              </Typography>
            </CardContent>
          </Card>

          <Card sx={{ bgcolor: 'background.paper', borderRadius: 4 }}>
            <CardContent sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>Статус (Health)</Typography>
              <Typography variant="h5" sx={{ fontWeight: 'bold' }}>
                {getHealthStatus(analytics?.averageVolatility)}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
                Прогноз нагрузки: {analytics?.totalPredictedLoad?.toFixed(1) || 0} ч.
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
              <ResponsiveContainer width="99%" height="100%">
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