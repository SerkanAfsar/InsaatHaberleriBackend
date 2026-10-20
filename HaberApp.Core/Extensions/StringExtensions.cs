namespace HaberApp.Core.Extensions
{
    public static class StringExtensions
    {
        extension(string str)
        {
            public string ToCamelCase()
            {
                if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
                    return str;

                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
        }
    }
}
