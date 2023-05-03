using core.Dtos;
using Dashboard.Models;
using Dashboard.Repository;
using Dashboard.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Security.Claims;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork u;

        public HomeController(UserService userService)
        {
            u = new UnitOfWork(new Context(), userService.GetCurrentUser()); // skipping DI
		}
  
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
				return Redirect("/home/Index");

			return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginCheck(string Email, string Password)
        {
            var loginResponse = u.accountRepository.login(Email, Password);

            if (!loginResponse.Result)
            {
                TempData["error"] = loginResponse.Msg;
                return Redirect("/home/login");
            }

            // Add user info for who Authenticated
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResponse.Payload.Id.ToString()),
                new Claim(ClaimTypes.Name, loginResponse.Payload.Name),
                new Claim(ClaimTypes.Email, loginResponse.Payload.Email),
            };
            var useridentity = new ClaimsIdentity(claims, "claims");
            ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

            // Authentication
            await HttpContext.SignInAsync(principal);
            return Redirect("/home/Index");
        }

        public async Task<IActionResult> Logout()
        {
            //mRemove Authentication
            await HttpContext.SignOutAsync();
            return Redirect("/home/login");
        }

        public IActionResult Index()
        {
            var stories = u.storyRepository.GetAll();
            var listViewDto = new ListViewDto();
            listViewDto.NrOfRecs = stories.Count();
            Context db = new Context();

            listViewDto.PageData = stories.Select(e => new ListViewItemDto
            {
                Id = e.Id,
                Title = e.Title,
                CoverPhotoURL = e.CoverPhotoURL.Contains("http") ? e.CoverPhotoURL : AppConstants.AWS.ImgRoot + S3.Folder_Dir_StoryImage + e.CoverPhotoURL,
                AspectRatio = e.CoverPhotoAspectRatio is null ? 0 : (double)e.CoverPhotoAspectRatio,
                CreatedAt = DateHumanReadable(e.CreatedAt),
                LocationInfo = e.Address,
                Keywords = string.Join(',', e.Keywords.ToList())
            }).ToList();

            return View(listViewDto);
        }

        string DateHumanReadable(DateTime d)
        {
            if (d.Date == DateTime.UtcNow.Date)
            {
                return "today";
            }
            else if (d.Date == DateTime.UtcNow.AddDays(-1))
            {
                return "yesterday";
            }
            else if (d.Date == DateTime.UtcNow.AddDays(1))
            {
                return "tomorrow";
            }
            else
            {
                string day = d.Day.ToString();
                string month = d.ToString("MMM");
                string year = d.Year.ToString();

                return $@"
                    <p class='date-part1'>{day}</p> 
                    <p class='date-part2'>{month}</p> 
                    <p class='date-part3'>{year}</p>";
            }
        }
    }
}
