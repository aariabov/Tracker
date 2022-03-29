using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tracker.Web.RequestModels;

public class InstructionRm
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string ExecutorId { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}