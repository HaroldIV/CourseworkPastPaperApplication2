using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class PaperResult
{
    public Guid Id { get; set; }

    public int Score { get; set; }

    public Guid AssignmentId { get; set; }

    public long? StudentPassword { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Student? StudentPasswordNavigation { get; set; }
}
