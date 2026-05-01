import { Box, Button, Container, Typography, Card, CardContent, Stack } from '@mui/material';
import { Link } from 'react-router-dom';
import { Speed, AutoGraph, ElectricBolt } from '@mui/icons-material';

export default function Home() {
  return (
    <Box sx={{ bgcolor: 'background.default', color: 'text.primary', minHeight: '100vh' }}>
      
      {/* --- NAVBAR --- */}
      <Container maxWidth="lg">
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', py: 3 }}>
          <Typography variant="h5" sx={{ fontWeight: 'bold', letterSpacing: 1 }}>
            PMS<span style={{ color: '#FFD700' }}>.</span>PORTAL
          </Typography>
          <Stack direction="row" spacing={2}>
            <Button component={Link} to="/login" color="inherit" sx={{ fontWeight: 600 }}>
              Войти
            </Button>
            <Button component={Link} to="/register" variant="contained" color="primary">
              Начать работу
            </Button>
          </Stack>
        </Box>
      </Container>

      {/* --- HERO SECTION --- */}
      <Container maxWidth="lg" sx={{ mt: { xs: 8, md: 15 }, mb: 10 }}>
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, alignItems: 'center', gap: 4 }}>
          
          {/* Левая колонка */}
          <Box sx={{ flex: { xs: '1 1 100%', md: '0 0 58%' } }}>
            <Typography variant="h1" sx={{ 
              fontSize: { xs: '2.5rem', md: '4rem' }, 
              fontWeight: 800, 
              lineHeight: 1.1,
              mb: 3 
            }}>
              Управляйте временем <br />
              <span style={{ color: '#FFD700' }}>на новом уровне</span>
            </Typography>
            <Typography variant="h6" sx={{ color: 'text.secondary', mb: 5, maxWidth: '500px', fontWeight: 400 }}>
              Первая система управления проектами с глубокой интеграцией WakaTime и предиктивной аналитикой волатильности разработчиков.
            </Typography>
            <Stack direction="row" spacing={2}>
              <Button 
                component={Link} 
                to="/register" 
                variant="contained" 
                color="primary" 
                size="large" 
                sx={{ px: 4, py: 1.5, fontSize: '1.1rem' }}
              >
                Попробовать бесплатно
              </Button>
              <Button 
                variant="outlined" 
                color="primary" 
                size="large" 
                sx={{ px: 4, py: 1.5, fontSize: '1.1rem' }}
              >
                Узнать больше
              </Button>
            </Stack>
          </Box>
          
          {/* Декоративный элемент / Абстракция (Правая колонка) */}
          <Box sx={{ 
            flex: { xs: '1 1 100%', md: '0 0 41%' }, 
            display: { xs: 'none', md: 'flex' }, 
            justifyContent: 'center', 
            position: 'relative' 
          }}>
            {/* Желтое свечение на фоне */}
            <Box sx={{
              position: 'absolute',
              width: '100%',
              height: '100%',
              background: 'rgba(255, 215, 0, 0.12)',
              filter: 'blur(80px)',
              borderRadius: '50%',
              top: '50%',
              left: '50%',
              transform: 'translate(-50%, -50%)',
              zIndex: 0
            }} />
            
            {/* Карточка с кодом */}
            <Box sx={{ 
              py: 6, // Отступы сверху и снизу
              px: 4, 
              width: '100%', // Растягиваем карточку по ширине
              display: 'flex',
              justifyContent: 'center', // Центрируем блок кода внутри карточки
              border: '1px solid rgba(255,255,255,0.1)', 
              borderRadius: '32px', // Фиксированное красивое скругление (вместо овала)
              bgcolor: 'background.paper',
              boxShadow: '0 20px 40px rgba(0,0,0,0.4)',
              zIndex: 1
            }}>
              <pre style={{ margin: 0, color: '#FFD700', fontSize: '15px', fontFamily: 'Consolas, monospace', lineHeight: 1.6 }}>
{`{
  "project": "PMS.Portal",
  "status": "In Progress",
  "realtime": true,
  "wakatime": "connected",
  "analytics": {
    "volatility": "low",
    "health": "excellent"
  }
}`}
              </pre>
            </Box>
          </Box>
        </Box>
      </Container>

      {/* --- FEATURES SECTION --- */}
      <Container maxWidth="lg" sx={{ py: 10 }}>
        <Typography variant="h4" sx={{ textAlign: 'center', fontWeight: 'bold', mb: 8 }}>
          Почему выбирают <span style={{ color: '#FFD700' }}>PMS</span>
        </Typography>
        
        <Box sx={{ 
          display: 'grid', 
          gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, 
          gap: 4 
        }}>
          <FeatureCard 
            icon={<AutoGraph sx={{ fontSize: 40, color: '#FFD700' }} />}
            title="Авто-трекинг"
            description="Никаких ручных логов. Интеграция с WakaTime сама посчитает время, проведенное в IDE."
          />
          <FeatureCard 
            icon={<ElectricBolt sx={{ fontSize: 40, color: '#FFD700' }} />}
            title="Real-time магия"
            description="Благодаря SignalR данные на дашборде обновляются мгновенно. Вы видите прогресс вживую."
          />
          <FeatureCard 
            icon={<Speed sx={{ fontSize: 40, color: '#FFD700' }} />}
            title="Умная аналитика"
            description="Система анализирует волатильность и предсказывает сроки завершения задач."
          />
        </Box>
      </Container>

      {/* --- FOOTER --- */}
      <Box sx={{ borderTop: '1px solid rgba(255,255,255,0.05)', py: 6, mt: 10 }}>
        <Container maxWidth="lg">
          <Typography variant="body2" color="text.secondary" align="center">
            © {new Date().getFullYear()} PMS Portal. Разработано для эффективных команд.
          </Typography>
        </Container>
      </Box>
    </Box>
  );
}

function FeatureCard({ icon, title, description }: { icon: any, title: string, description: string }) {
  return (
    <Card sx={{ height: '100%', transition: 'transform 0.3s', '&:hover': { transform: 'translateY(-10px)' } }}>
      <CardContent sx={{ p: 4 }}>
        <Box sx={{ mb: 2 }}>{icon}</Box>
        <Typography variant="h5" sx={{ fontWeight: 'bold', mb: 2 }}>{title}</Typography>
        <Typography variant="body1" color="text.secondary">{description}</Typography>
      </CardContent>
    </Card>
  );
}