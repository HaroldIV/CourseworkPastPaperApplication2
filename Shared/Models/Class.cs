using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Class
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public virtual Teacher TeacherNavigation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; init; } = new List<Student>();
}
