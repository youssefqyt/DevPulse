using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DevPulse.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(150, ErrorMessage = "Task title cannot exceed 150 characters.")]
    public string Title { get; set; } = "";

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = "";

    [Required]
    public string Status { get; set; } = "Todo"; // Todo, In Progress, Done

    [Required]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High

    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Please select a project.")]
    public int ProjectId { get; set; }

    [ValidateNever]
    public Project? Project { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
