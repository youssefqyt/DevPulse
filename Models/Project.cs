using System.ComponentModel.DataAnnotations;

namespace DevPulse.Models;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    public string Name { get; set; } = "";

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Technology stack is required.")]
    [StringLength(100, ErrorMessage = "Technology cannot exceed 100 characters.")]
    public string Technology { get; set; } = "";

    [Required]
    public string Status { get; set; } = "Active"; // Active, Completed, Paused

    [Required]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High

    [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
    public int Progress { get; set; }

    public DateOnly StartDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    public DateOnly? DueDate { get; set; }

    [Range(0, 10000, ErrorMessage = "Estimated hours must be a positive number.")]
    public decimal EstimatedHours { get; set; }

    [Range(0, 10000, ErrorMessage = "Hours spent must be a positive number.")]
    public decimal HoursSpent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskItem> Tasks { get; set; } = [];
}
