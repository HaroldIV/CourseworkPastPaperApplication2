using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Assignment
{
    public Guid Id { get; set; }

    public DateTime Set { get; set; }

    public DateTime Due { get; set; }

    public long? StudentPassword { get; set; }

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual Student? StudentPasswordNavigation { get; set; }
}
