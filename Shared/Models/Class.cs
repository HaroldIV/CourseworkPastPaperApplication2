using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Class
{
    public Guid Id { get; set; }

    public long TeacherPassword { get; set; }

    public virtual Teacher TeacherNavigation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; init; } = new List<Student>();
}
