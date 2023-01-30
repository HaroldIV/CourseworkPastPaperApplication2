﻿using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Student
{
    public long Password { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Assignment> Assignments { get; } = new List<Assignment>();

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    public virtual ICollection<Class> CurrentClasses { get; } = new List<Class>();
}
