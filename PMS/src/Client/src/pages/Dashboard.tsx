import { useState, useEffect } from 'react';
import { 
  Box, Typography, Button, Card, CardContent, LinearProgress, 
  Chip, Dialog, DialogTitle, DialogContent, DialogActions, TextField,
  Stack, Avatar, Menu, MenuItem} from "@mui/material";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { tasksApi } from "../api/tasksApi";
import { useTaskSignalR } from '../hooks/useTaskSignalR';
import type { TaskDto } from "../types";

// Схема валидации для новой задачи
const taskSchema = z.object({
  title: z.string().min(3, 'Название слишком короткое'),
  planHours: z.number().min(0.1, 'Минимум 0.1 часа'), 
});

type TaskFormInputs = z.infer<typeof taskSchema>;

export default function Dashboard() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  
  // Состояния для модалки и меню
  const [open, setOpen] = useState(false); 
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [userEmail, setUserEmail] = useState<string>("user@pms.com");

  // Извлекаем email из JWT токена в куках
  useEffect(() => {
    const savedEmail = localStorage.getItem('userEmail');
    if (savedEmail) {
      setUserEmail(savedEmail);
    }
  }, []);

  // Получаем задачи
  const { data: tasks, isLoading } = useQuery<TaskDto[]>({
    queryKey: ['tasks'],
    queryFn: tasksApi.getTasks
  });

  // Включаем SignalR для обновлений в реальном времени
  useTaskSignalR();

  // Мутация для создания задачи
  const createTaskMutation = useMutation({
    mutationFn: tasksApi.createTask,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      setOpen(false);
      reset();
    }
  });

  // Мутация для завершения задачи
  const completeTaskMutation = useMutation({
    mutationFn: tasksApi.completeTask,
    onSuccess: () => {
      // Обновляем список задач, чтобы чип мгновенно стал зеленым
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    }
  });

  // Настройка формы
  const { register, handleSubmit, reset, formState: { errors } } = useForm<TaskFormInputs>({
    resolver: zodResolver(taskSchema),
  });

  const onSubmit = (data: TaskFormInputs) => {
    createTaskMutation.mutate(data);
  };

  const getEmailInitial = (email: string) => email.charAt(0).toUpperCase();

  if (isLoading) return <Box sx={{ p: 4 }}><Typography>Загрузка задач...</Typography></Box>;

  return (
    <Box sx={{ p: 4, maxWidth: 1200, margin: '0 auto' }}>
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
      {/* --- HEADER --- */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 6 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
          Мои задачи <span style={{ color: '#FFD700' }}></span>
        </Typography>
        
        <Stack direction="row" spacing={3} sx={{ alignItems: 'center' }}>
          <Button variant="contained" color="primary" onClick={() => setOpen(true)}>
            Новая задача
          </Button>

          <Box 
            onClick={(e) => setAnchorEl(e.currentTarget)} 
            sx={{ display: 'flex', alignItems: 'center', gap: 1.5, cursor: 'pointer' }}
          >
            <Avatar sx={{ 
              bgcolor: 'primary.main', 
              color: 'background.default', 
              fontWeight: 'bold',
              width: 35,
              height: 35,
              fontSize: '0.9rem'
            }}>
              {getEmailInitial(userEmail)}
            </Avatar>
            <Typography variant="body2" sx={{ fontWeight: 500, color: 'text.secondary', display: { xs: 'none', sm: 'block' } }}>
              {userEmail}
            </Typography>
          </Box>

          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={() => setAnchorEl(null)}
            slotProps={{
              paper: { sx: { mt: 1, minWidth: 180, borderRadius: 2 } }
            }}
          >
            <MenuItem onClick={() => { setAnchorEl(null); navigate('/profile'); }}>
              👤 Мой профиль
            </MenuItem>
            <MenuItem onClick={() => { setAnchorEl(null); navigate('/login'); }} sx={{ color: 'error.main' }}>
              🚪 Выйти
            </MenuItem>
          </Menu>
        </Stack>
      </Box>

      {/* --- GRID OF TASKS --- */}
      <Box sx={{ 
        display: 'grid', 
        gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: 'repeat(3, 1fr)' }, 
        gap: 3 
      }}>
        {tasks?.map((task) => {
          const progressPercent = task.planHours > 0 ? Math.round((task.factHours / task.planHours) * 100) : 0;
          return (
            <Card key={task.id} sx={{ height: '100%' }}>
              <CardContent sx={{ pt: 3 }}>
                
                {/* --- ВОТ ЗДЕСЬ ДОБАВЛЕНА КНОПКА ЗАВЕРШЕНИЯ --- */}
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                  <Typography variant="caption" sx={{ color: 'primary.main', fontWeight: 'bold' }}>
                    {task.taskKey}
                  </Typography>
                  
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                    <Chip 
                      label={task.isCompleted ? "Завершена" : "В работе"} 
                      size="small" 
                      color={task.isCompleted ? "success" : "warning"}
                    />
                    
                    {/* Кнопка показывается ТОЛЬКО если задача еще не завершена */}
                    {!task.isCompleted && (
                      <Button 
                        size="small" 
                        variant="contained" 
                        color="success"
                        sx={{ fontSize: '0.7rem', minWidth: 'auto', px: 1.5, py: 0.3 }}
                        onClick={() => completeTaskMutation.mutate(task.id)}
                        disabled={completeTaskMutation.isPending}
                      >
                        ✔
                      </Button>
                    )}
                  </Stack>
                </Box>
                {/* ------------------------------------------------ */}

                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, minHeight: '60px' }}>
                  {task.title}
                </Typography>
                <LinearProgress variant="determinate" value={Math.min(progressPercent, 100)} sx={{ height: 8, borderRadius: 4, mb: 2 }} />
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                    {task.factHours.toFixed(2)} ч.
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    План: {task.planHours.toFixed(1)} ч.
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          );
        })}
      </Box>

      {/* --- MODAL: CREATE TASK --- */}
      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="xs">
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogTitle sx={{ fontWeight: 'bold' }}>Создание задачи</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              label="Название задачи"
              fullWidth
              variant="outlined"
              sx={{ mt: 1, mb: 2 }}
              {...register('title')}
              error={!!errors.title}
              helperText={errors.title?.message}
            />
            <TextField
              margin="dense"
              label="Планируемые часы"
              type="number"
              fullWidth
              variant="outlined"
              {...register('planHours', { valueAsNumber: true })}
              error={!!errors.planHours}
              helperText={errors.planHours?.message}
              slotProps={{
                htmlInput: { step: "0.1" }
              }}
            />
          </DialogContent>
          <DialogActions sx={{ p: 3 }}>
            <Button onClick={() => setOpen(false)} color="inherit">Отмена</Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={createTaskMutation.isPending}
            >
              {createTaskMutation.isPending ? 'Создание...' : 'Создать'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
}