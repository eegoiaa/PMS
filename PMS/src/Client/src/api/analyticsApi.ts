import { axiosClient } from "./axiosClient";

export interface AnalyticsSummary {
  volatilityScore: number;
  health: string;
}

export const analyticsApi = {
  getSummary: async (developerId: string): Promise<AnalyticsSummary> => {
    const response = await axiosClient.get<AnalyticsSummary>(`/analytics/developer/${developerId}`);
    return response.data;
  }
};