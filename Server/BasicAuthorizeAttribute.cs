using Microsoft.AspNetCore.Authorization;

namespace Server
{
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        public BasicAuthorizeAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
