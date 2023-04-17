using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public class Level : IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExamBoard : IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }
}
