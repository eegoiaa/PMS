export interface TaskDto {
  id: string;
  taskKey: string;
  title: string;
  planHours: number;
  factHours: number;
  isCompleted: boolean;
  developerId: string;
}

export interface DeveloperAnalyticsDto {
  fullName: string;
  averageVolatility: number;
  totalPredictedLoad: number;
  activeTasksCount: number;
}