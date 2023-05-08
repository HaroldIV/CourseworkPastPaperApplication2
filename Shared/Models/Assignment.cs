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

    [JsonInclude]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [JsonIgnore]
    public virtual Class Class { get; set; } = null!;
}