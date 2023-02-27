﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Student : User
{
    public virtual ICollection<Assignment> Assignments { get; } = new List<Assignment>();

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    [JsonIgnore]
    public virtual ICollection<Class> CurrentClasses { get; } = new List<Class>();
}
