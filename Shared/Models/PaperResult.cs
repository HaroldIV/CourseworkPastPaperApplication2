using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class PaperResult : DbTable
{
    public int Score { get; set; }

    public Guid AssignmentId { get; set; }

    public Guid StudentId { get; set; }

    public Guid QuestionId { get; set; }

    [JsonIgnore]
    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
