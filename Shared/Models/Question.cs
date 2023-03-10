using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Question
{
    public Guid Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public string ReadData { get; set; } = null!;

    public Guid AssignmentId { get => Assignment.Id; set => Assignment.Id = value; }

    public virtual Assignment Assignment { get; set; } = null!;
}
