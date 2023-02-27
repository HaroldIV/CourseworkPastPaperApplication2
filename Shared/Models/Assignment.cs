using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public class Assignment
{
    public Guid Id { get; set; }

    public DateTime Set { get; set; }

    public DateTime Due { get; set; }

    public Guid? StudentId
    {
        get => Student!.Id;
        set => Student!.Id = value ?? default;
    }

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual Student? Student { get; set; }
}
