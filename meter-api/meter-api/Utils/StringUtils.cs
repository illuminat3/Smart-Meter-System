namespace meter_api.Utils;

public static class StringUtils
{
    public static string ToCamelCase(this string str)
    {
        return string.IsNullOrEmpty(str) || char.IsLower(str[0]) ? str : char.ToLower(str[0]) + str[1..];
    }
}