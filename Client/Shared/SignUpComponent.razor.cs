using CourseworkPastPaperApplication2.Shared;
using Microsoft.AspNetCore.Components;

namespace CourseworkPastPaperApplication2.Client.Shared
{
    // Restricts TUser used in the SignUpComponent to be an instance of User and have a blank constructor. 
    public partial class SignUpComponent<TUser> : ComponentBase where TUser : User, new()
    {
    }
}
