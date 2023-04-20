using Dashboard.Models;
using OpenQA.Selenium.DevTools.V109.Profiler;
using System.Data;
using System.Security.Claims;

namespace Dashboard.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return user.Identity.IsAuthenticated;
        }

        public CurrentUser GetCurrentUser()
        {
            if (!IsAuthenticated())
                return null; // throw new UnauthorizedAccessException();

            CurrentUser u = new CurrentUser();

            var user = _httpContextAccessor.HttpContext.User;
            u.Id  = Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier));
            u.Name  = user.FindFirstValue(ClaimTypes.Email);
            u.Email = user.FindFirstValue(ClaimTypes.Email);
        
            return u;
        }
    }
}
