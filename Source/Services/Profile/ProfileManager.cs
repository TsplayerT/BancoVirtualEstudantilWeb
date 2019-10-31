using BancoVirtualEstudantilWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BancoVirtualEstudantilWeb.Services.Profile
{
    public class ProfileManager
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private ApplicationUser CurrentUserPrivate { get; set; }

        public ProfileManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            UserManager = userManager;
            HttpContextAccessor = httpContextAccessor;
        }

        public ApplicationUser CurrentUser => CurrentUserPrivate ??= UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User).Result;

        public bool IsHasPassword(ApplicationUser user)
        {
            return UserManager.HasPasswordAsync(user).Result;
        }

        public bool IsEmailConfirmed(ApplicationUser user)
        {
            return UserManager.IsEmailConfirmedAsync(user).Result;
        }
    }
}
