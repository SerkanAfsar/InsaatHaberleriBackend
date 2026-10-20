using HaberApp.Core.Domains;
using HaberApp.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace HaberApp.Core.Helpers
{
    public static class RoleHelpers
    {
        public static async Task<string?> FindRootAdminRoleId()
        {
            var roleManager = ServiceProviderHelper.GetRequiredService<RoleManager<AppRole>>();

            var rootAdminRole = await roleManager.FindByNameAsync(RoleConstants.RootAdmin);

            if (rootAdminRole is not null)
            {
                return rootAdminRole.Id;
            }
            return null;
        }
    }
}
