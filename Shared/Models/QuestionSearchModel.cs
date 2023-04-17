using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public struct QuestionSearchModel
    {
        public List<string> KeywordsList { get; set; }
        public bool All { get; set; }
        
        public IEnumerable<ExamBoard> ExamBoards { get; set; }
        public IEnumerable<Level> ValidLevels { get; set; }
    }
}
