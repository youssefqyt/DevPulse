namespace DevPulse.Models;

public class DashboardViewModel
{
    public int ProjectCount { get; set; }
    public int ActiveProjectCount { get; set; }
    public int CompletedProjectCount { get; set; }
    public decimal LearningHours { get; set; }
    public decimal EstimatedHours { get; set; }
    public int AiToolsCount { get; set; }

    public int TotalTasksCount { get; set; }
    public int CompletedTasksCount { get; set; }
    public int InProgressTasksCount { get; set; }
    public int TodoTasksCount { get; set; }
    public int OverallProgress { get; set; }

    public List<Project> Projects { get; set; } = [];
    public List<TaskItem> UpcomingTasks { get; set; } = [];
    public List<AiTool> AiTools { get; set; } = [];
}
