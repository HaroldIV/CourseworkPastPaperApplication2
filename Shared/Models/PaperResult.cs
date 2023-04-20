using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public class PaperResult : DbTable
{
    public int Score { get; set; }

    public Guid AssignmentId { get => Assignment.Id; set => Assignment.Id = value; }

    public Guid? StudentId { get => Student!.Id; set => Student!.Id = value ?? default; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
