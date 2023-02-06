using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class PaperResult
{
    public Guid Id { get; set; }

    public int Score { get; set; }

    public Guid AssignmentId { get => Assignment.Id; set => Assignment.Id = value; }

    public long? StudentPassword { get => StudentPasswordNavigation!.Password; set => StudentPasswordNavigation!.Password = value ?? default; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Student? StudentPasswordNavigation { get; set; }
}
