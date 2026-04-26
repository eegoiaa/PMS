using Core.Domain.Entities;

namespace Core.Domain.Logic;

public static class VolatilityCalculator
{
    /// <summary>
    /// Вычисляет индекс волатильности конкретной задачи (V_task)
    /// Формула: (t_fact - t_plan) / t_plan
    /// </summary>
    public static double CalculateTaskVolatility(ProjectTask task)
    {
        if (task.PlanHours <= 0)
            return 0;

        var volatility = (task.FactHours - task.PlanHours) / task.PlanHours;

        return Math.Round(volatility, 2);
    }

    /// <summary>
    /// Вычисляет суммарную прогнозируемую нагрузку сотрудника на спринт (L_total)
    /// Формула: Сумма (t_plan * (1 + V_avg))
    /// </summary>
    public static double CalculateTotalPredictedLoad(IEnumerable<ProjectTask> plannedTasks, double averageVolatility)
    {
        double totalLoad = 0;

        foreach (var task in plannedTasks)
        {
            totalLoad += task.PlanHours * (1 + averageVolatility);
        }

        return Math.Round(totalLoad, 2);
    }
}
