import { axiosClient } from "./axiosClient";

export interface AnalyticsSummary {
  fullName: string;
  averageVolatility: number;
  totalPredictedLoad: number;
  activeTasksCount: number;
}

export const analyticsApi = {
  getSummary: async (developerId: string): Promise<AnalyticsSummary> => {
    const response = await axiosClient.get<AnalyticsSummary>(`/analytics/developer/${developerId}`);
    return response.data;
  }
};