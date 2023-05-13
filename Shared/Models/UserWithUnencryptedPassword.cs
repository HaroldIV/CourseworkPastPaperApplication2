using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // This class represents the user login/sign-up details that a user may enter on the client-side and supply to the server-side. 
    public struct UserWithUnencryptedPassword
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}