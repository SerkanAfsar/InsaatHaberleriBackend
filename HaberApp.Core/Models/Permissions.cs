using HaberApp.Core.Domains;

namespace HaberApp.Core.Models
{
    public static class Permissions<T> where T : BaseEntity
    {
        private static string ModuleName => typeof(T).Name.ToLowerInvariant();
        public static readonly string Read = $"permission.{ModuleName}.read";
        public static readonly string Create = $"permission.{ModuleName}.create";
        public static readonly string Update = $"permission.{ModuleName}.update";
        public static readonly string Delete = $"permission.{ModuleName}.delete";
    }
}
