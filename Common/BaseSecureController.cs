using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASM_NhomSugar_SD19311.Common
{
    [ApiController]
    public class BaseSecureController : ControllerBase
    {
        protected CurrentUser CurrentUser
        {
            get
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                return new CurrentUser
                {
                    Id = userId,
                    UserName = userName,
                    Email = email
                };
            }
        }
    }

    public class CurrentUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
