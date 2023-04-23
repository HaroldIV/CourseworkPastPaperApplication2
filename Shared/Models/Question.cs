using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public class Question : DbTable
{
    public byte[] Data { get; set; } = null!;

    public string ReadData { get; set; } = null!;

    public ExamBoard? ExamBoard { get; set; } = null!;

    public Level? Level { get; set; } = null!;
}
