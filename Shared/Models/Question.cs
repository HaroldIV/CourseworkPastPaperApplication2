using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Question : DbTable, IQuestionWithoutData
{
    public string FileName { get; set; } = null!;

    public int Marks { get; set; }

    public byte[] Data { get; set; } = null!;

    public string ReadData { get; set; } = null!;

    public ExamBoard? ExamBoard { get; set; } = null!;

    public Level? Level { get; set; } = null!;
}

internal interface IQuestionWithoutData
{
    public string FileName { get; set; }

    public int Marks { get; set; }

    public string ReadData { get; set; }

    public ExamBoard? ExamBoard { get; set; }

    public Level? Level { get; set; }
}