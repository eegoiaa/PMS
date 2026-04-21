namespace Core.Application.Queries.GetAnalytics;

public record GetDeveloperAnalyticsDto(
    string FullName,
    double AverageVolatility,
    double TotalPredictedLoad,
    int ActiveTasksCount
    );
