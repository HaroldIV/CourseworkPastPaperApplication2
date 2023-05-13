using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // The question search model used to search through questions in associated queries and requests from the client-side. 
    public struct QuestionSearchModel
    {
        // Hash set of keywords used. 
        // It is a hash set to prevent duplicate keywords. 
        public HashSet<string> KeywordsList { get; set; }
        
        // Boolean flag indicated whether or not to retrieve only questions that match all keywords or to maximise the number of matches instead. 
        public bool All { get; set; }
        
        // All exam boards that the question can be of. 
        public IEnumerable<ExamBoard> ExamBoards { get; set; }

        // All levels that the question can be of. 
        public IEnumerable<Level> ValidLevels { get; set; }
    }
}
