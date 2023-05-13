using CourseworkPastPaperApplication2.Shared;
using Microsoft.AspNetCore.Components;

namespace CourseworkPastPaperApplication2.Client.Shared
{
    // Restricts TUser used in the LoginComponent to be an instance of User and have a blank constructor. 
    public partial class LoginComponent<TUser> : ComponentBase where TUser : User, new()
    {
    }
}
