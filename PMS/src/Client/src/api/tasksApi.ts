import { axiosClient } from "./axiosClient";
import type { TaskDto } from "../types";

export const tasksApi = {
  getTasks: async (): Promise<TaskDto[]> => {
    const response = await axiosClient.get<TaskDto[]>('/tasks/my'); 
    return response.data;
  }
};