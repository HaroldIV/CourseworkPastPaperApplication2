using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Class
{
    public Guid Id { get; set; }

    public long TeacherPassword { get; set; }

    public virtual Teacher TeacherPasswordNavigation { get; set; } = null!;

    public virtual ICollection<Student> StudentsPasswords { get; } = new List<Student>();
}
