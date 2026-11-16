using System.Collections.ObjectModel;

namespace HaberApp.Core.Utils
{
    public static class RoleConstants
    {
        public static string RootAdmin { get; set; } = nameof(RootAdmin);
        public static string NormalUser { get; set; } = nameof(NormalUser);
        public static IReadOnlyCollection<string> RoleList => new ReadOnlyCollection<string>(new[] { RootAdmin, NormalUser });
        public static bool IsInRole(string roleName) => RoleList.Contains(roleName);

    }
}
