import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';
import type { TaskDto } from '../types';

export const useTaskSignalR = () => {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7019/task-hub", {
        withCredentials: true 
      })
      .withAutomaticReconnect() 
      .build();

    connection.on("TaskUpdated", (taskId: string, newFactHours: number) => {
      console.log(`[SignalR] Сигнал! Задача ${taskId} обновилась. Новые часы: ${newFactHours}`);

      queryClient.setQueryData<TaskDto[]>(['tasks'], (oldTasks) => {
        if (!oldTasks) return [];
        return oldTasks.map(task => 
          task.id === taskId 
            ? { ...task, factHours: newFactHours } 
            : task
        );
      });
    });

    connection.start()
      .then(() => console.log("[SignalR] Успешно подключено к TaskHub!"))
      .catch(err => console.error("[SignalR] Ошибка подключения: ", err));

    return () => {
      connection.stop();
    };
  }, [queryClient]);
};