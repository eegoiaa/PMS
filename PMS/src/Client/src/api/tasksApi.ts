import { axiosClient } from "./axiosClient";
import type { TaskDto } from "../types";

export const tasksApi = {
  getTasks: async (): Promise<TaskDto[]> => {
    const response = await axiosClient.get<TaskDto[]>('/tasks/my'); 
    return response.data;
  },

  createTask: async (task: { title: string; planHours: number }): Promise<string> => {
    const response = await axiosClient.post<string>('/tasks', task);
    return response.data;
  }
};