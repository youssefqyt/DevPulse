using DevPulse.Data;
using DevPulse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevPulse.Controllers;

public class DashboardController : Controller
{
    private readonly DevPulseDbContext _context;

    public DashboardController(DevPulseDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var allTasks = projects.SelectMany(p => p.Tasks).ToList();
        var completedTasks = allTasks.Count(t => t.Status == "Done");
        var inProgressTasks = allTasks.Count(t => t.Status == "In Progress");
        var todoTasks = allTasks.Count(t => t.Status == "Todo");

        int overallProgress = allTasks.Count > 0
            ? (int)Math.Round((double)completedTasks / allTasks.Count * 100)
            : (projects.Count > 0 ? (int)Math.Round(projects.Average(p => p.Progress)) : 0);

        var upcomingTasks = allTasks
            .Where(t => t.Status != "Done" && t.DueDate.HasValue)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .ToList();

        var model = new DashboardViewModel
        {
            ProjectCount = projects.Count,
            ActiveProjectCount = projects.Count(p => p.Status == "Active"),
            CompletedProjectCount = projects.Count(p => p.Status == "Completed"),
            LearningHours = projects.Sum(p => p.HoursSpent),
            EstimatedHours = projects.Sum(p => p.EstimatedHours),
            AiToolsCount = 4,

            TotalTasksCount = allTasks.Count,
            CompletedTasksCount = completedTasks,
            InProgressTasksCount = inProgressTasks,
            TodoTasksCount = todoTasks,
            OverallProgress = overallProgress,

            Projects = projects,
            UpcomingTasks = upcomingTasks,

            AiTools =
            [
                new AiTool
                {
                    Name = "ChatGPT",
                    Description = "AI reasoning and code explanation assistant",
                    Icon = "🤖",
                    Category = "General AI",
                    Url = "https://chatgpt.com"
                },
                new AiTool
                {
                    Name = "GitHub Copilot",
                    Description = "AI pair programmer directly in your code editor",
                    Icon = "⚡",
                    Category = "Code Completion",
                    Url = "https://github.com/features/copilot"
                },
                new AiTool
                {
                    Name = "Claude",
                    Description = "Anthropic AI for complex code refactoring and architecture",
                    Icon = "🧠",
                    Category = "Architecture & Logic",
                    Url = "https://claude.ai"
                },
                new AiTool
                {
                    Name = "v0 / Bolt",
                    Description = "AI for rapid UI design and frontend scaffolding",
                    Icon = "🎨",
                    Category = "Frontend & UI",
                    Url = "https://v0.dev"
                }
            ]
        };

        return View(model);
    }
}
