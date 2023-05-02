using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Assignment : DbTable
{
    public DateOnly Set { get; set; }

    public DateOnly Due { get; set; }

    public string Name { get; set; } = null!;

    [JsonInclude]
    public int TotalMarks => Questions.Sum(question => question.Marks);

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual Class Class { get; set; } = null!;
}