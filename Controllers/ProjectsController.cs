using DevPulse.Data;
using DevPulse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevPulse.Controllers;

public class ProjectsController : Controller
{
    private readonly DevPulseDbContext _context;

    public ProjectsController(DevPulseDbContext context)
    {
        _context = context;
    }

    // GET: /Projects
    public async Task<IActionResult> Index(
        string? search,
        string? status,
        string? priority,
        string? sortBy)
    {
        var query = _context.Projects
            .Include(p => p.Tasks)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                p.Technology.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => p.Status.ToLower() == status.ToLower());
        }

        // Priority filter
        if (!string.IsNullOrWhiteSpace(priority) && !priority.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => p.Priority.ToLower() == priority.ToLower());
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "oldest" => query.OrderBy(p => p.CreatedAt),
            "name" => query.OrderBy(p => p.Name),
            "name-desc" => query.OrderByDescending(p => p.Name),
            "progress" => query.OrderByDescending(p => p.Progress),
            "deadline" => query.OrderBy(p => p.DueDate.HasValue ? 0 : 1).ThenBy(p => p.DueDate),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var projects = await query.ToListAsync();

        ViewBag.CurrentSearch = search;
        ViewBag.CurrentStatus = status ?? "All";
        ViewBag.CurrentPriority = priority ?? "All";
        ViewBag.CurrentSort = sortBy ?? "newest";

        return View(projects);
    }

    // GET: /Projects/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects
            .Include(p => p.Tasks.OrderBy(t => t.Status == "Done").ThenBy(t => t.DueDate))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    // GET: /Projects/Create
    public IActionResult Create()
    {
        return View(new Project());
    }

    // POST: /Projects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (!ModelState.IsValid)
            return View(project);

        project.CreatedAt = DateTime.UtcNow;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Project \"{project.Name}\" was created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Projects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    // POST: /Projects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(project);

        var existing = await _context.Projects.FindAsync(id);
        if (existing == null)
            return NotFound();

        try
        {
            existing.Name = project.Name;
            existing.Technology = project.Technology;
            existing.Description = project.Description;
            existing.Status = project.Status;
            existing.Priority = project.Priority;
            existing.Progress = project.Progress;
            existing.EstimatedHours = project.EstimatedHours;
            existing.HoursSpent = project.HoursSpent;
            existing.StartDate = project.StartDate;
            existing.DueDate = project.DueDate;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Project \"{existing.Name}\" was updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(project.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Projects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    // POST: /Projects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project != null)
        {
            var projectName = project.Name;
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Project \"{projectName}\" was deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST / GET: /Projects/SyncProgress/5 (Calculates progress from tasks)
    public async Task<IActionResult> SyncProgress(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        if (project.Tasks.Count > 0)
        {
            var completedCount = project.Tasks.Count(t => t.Status == "Done");
            project.Progress = (int)Math.Round((double)completedCount / project.Tasks.Count * 100);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Project progress synced to {project.Progress}% ({completedCount} of {project.Tasks.Count} tasks completed).";
        }
        else
        {
            TempData["ErrorMessage"] = "No tasks found for this project. Add tasks to auto-calculate progress.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(p => p.Id == id);
    }
}
