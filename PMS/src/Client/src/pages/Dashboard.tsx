import { Box, Typography, Button, Card, CardContent, LinearProgress, Chip } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { tasksApi } from "../api/tasksApi.ts";
import type { TaskDto } from "../types"; 
import { useTaskSignalR } from '../hooks/useTaskSignalR';

export default function Dashboard() {
  const navigate = useNavigate();

  const { data: tasks, isLoading, isError } = useQuery<TaskDto[]>({
    queryKey: ['tasks'],
    queryFn: tasksApi.getTasks
  });

  useTaskSignalR();

  if (isLoading) return <Box sx={{ p: 4 }}><Typography>Загрузка задач...</Typography></Box>;
  if (isError) return <Box sx={{ p: 4 }}><Typography color="error">Ошибка загрузки задач</Typography></Box>;

  return (
    <Box sx={{ p: 4, maxWidth: 1200, margin: '0 auto' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 6 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
          Мои задачи <span style={{ color: '#FFD700' }}></span>
        </Typography>
        <Button variant="outlined" color="primary" onClick={() => navigate('/login')}>
          Выйти
        </Button>
      </Box>

      <Box sx={{ 
        display: 'grid', 
        gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: 'repeat(3, 1fr)' }, 
        gap: 3 
      }}>
        {tasks?.map((task) => {
          const progressPercent = task.planHours > 0 
            ? Math.round((task.factHours / task.planHours) * 100) 
            : 0;

          return (
            <Card key={task.id} sx={{ height: '100%', position: 'relative', overflow: 'visible' }}>
              <CardContent sx={{ pt: 3 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                  <Typography variant="caption" sx={{ color: 'primary.main', fontWeight: 'bold', letterSpacing: 1 }}>
                    {task.taskKey}
                  </Typography>
                  <Chip 
                    label={task.isCompleted ? "Завершена" : "В работе"} 
                    size="small" 
                    color={task.isCompleted ? "success" : "warning"}
                    sx={{ borderRadius: '6px', fontWeight: 'bold' }}
                  />
                </Box>

                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, minHeight: '60px' }}>
                  {task.title}
                </Typography>

                <Box sx={{ mb: 1, display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="body2" color="text.secondary">Прогресс</Typography>
                  <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                    {progressPercent}%
                  </Typography>
                </Box>

                <LinearProgress 
                  variant="determinate" 
                  value={Math.min(progressPercent, 100)} 
                  sx={{ height: 8, borderRadius: 4, mb: 3, backgroundColor: '#333' }}
                />

                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Box>
                    <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>Факт</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                      {task.factHours.toFixed(2)} ч.
                    </Typography>
                  </Box>
                  <Box sx={{ textAlign: 'right' }}>
                    <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>План</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                      {task.planHours.toFixed(2)} ч.
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          );
        })}
      </Box>
    </Box>
  );
}