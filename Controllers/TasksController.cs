using DevPulse.Data;
using DevPulse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevPulse.Controllers;

public class TasksController : Controller
{
    private readonly DevPulseDbContext _context;

    public TasksController(DevPulseDbContext context)
    {
        _context = context;
    }

    // GET: /Tasks or /Tasks?projectId=1
    public async Task<IActionResult> Index(
        int? projectId,
        string? status,
        string? priority,
        string? search)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .AsQueryable();

        Project? currentProject = null;

        if (projectId.HasValue)
        {
            currentProject = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId.Value);

            if (currentProject == null)
                return NotFound();

            query = query.Where(t => t.ProjectId == projectId.Value);
            ViewBag.Project = currentProject;
            ViewBag.IsAllTasks = false;
        }
        else
        {
            ViewBag.IsAllTasks = true;
            ViewBag.AllProjects = await _context.Projects
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(searchLower) ||
                t.Description.ToLower().Contains(searchLower));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(t => t.Status.ToLower() == status.ToLower());
        }

        // Priority filter
        if (!string.IsNullOrWhiteSpace(priority) && !priority.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
        }

        var tasks = await query
            .OrderBy(t => t.Status == "Done")
            .ThenBy(t => t.DueDate.HasValue ? 0 : 1)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

        ViewBag.CurrentStatus = status ?? "All";
        ViewBag.CurrentPriority = priority ?? "All";
        ViewBag.CurrentSearch = search;

        return View(tasks);
    }

    // GET: /Tasks/Create or /Tasks/Create?projectId=1
    public async Task<IActionResult> Create(int? projectId)
    {
        var task = new TaskItem();

        if (projectId.HasValue)
        {
            var project = await _context.Projects.FindAsync(projectId.Value);
            if (project == null)
                return NotFound();

            task.ProjectId = projectId.Value;
            ViewBag.Project = project;
        }

        ViewBag.Projects = await _context.Projects
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(task);
    }

    // POST: /Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem task)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Project = await _context.Projects.FindAsync(task.ProjectId);
            ViewBag.Projects = await _context.Projects.OrderBy(p => p.Name).ToListAsync();
            return View(task);
        }

        if (task.DueDate.HasValue)
        {
            task.DueDate = DateTime.SpecifyKind(task.DueDate.Value, DateTimeKind.Utc);
        }

        task.CreatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Task \"{task.Title}\" was created successfully!";

        return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
    }

    // GET: /Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound();

        ViewBag.Project = task.Project;
        ViewBag.Projects = await _context.Projects.OrderBy(p => p.Name).ToListAsync();

        return View(task);
    }

    // POST: /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItem task)
    {
        if (id != task.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Project = await _context.Projects.FindAsync(task.ProjectId);
            ViewBag.Projects = await _context.Projects.OrderBy(p => p.Name).ToListAsync();
            return View(task);
        }

        var existing = await _context.Tasks.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.Priority = task.Priority;
        existing.ProjectId = task.ProjectId;

        if (task.DueDate.HasValue)
        {
            existing.DueDate = DateTime.SpecifyKind(task.DueDate.Value, DateTimeKind.Utc);
        }
        else
        {
            existing.DueDate = null;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Task \"{existing.Title}\" was updated successfully!";

        return RedirectToAction(nameof(Index), new { projectId = existing.ProjectId });
    }

    // POST: /Tasks/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? returnUrl)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        var allowedStatuses = new[] { "Todo", "In Progress", "Done" };
        if (allowedStatuses.Contains(newStatus))
        {
            task.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Task status updated to \"{newStatus}\"!";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
    }

    // POST: /Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        var projectId = task.ProjectId;
        var taskTitle = task.Title;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Task \"{taskTitle}\" was deleted.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), new { projectId });
    }

    // GET: /Tasks/Delete/5 (Fallback with safe redirect)
    [HttpGet]
    public async Task<IActionResult> Delete(int id, string? returnUrl)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        var projectId = task.ProjectId;
        var taskTitle = task.Title;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Task \"{taskTitle}\" was deleted.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), new { projectId });
    }
}
