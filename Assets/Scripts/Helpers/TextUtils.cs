public static class TextUtils
{
    public static string CamelCaseToLabel(this string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
            return string.Empty;

        var label = System.Text.RegularExpressions.Regex.Replace(
            fieldName,
            "(?<!^)([A-Z])",
            " $1"
        );

        // Capitalize the first character
        return char.ToUpper(label[0]) + label.Substring(1);
    }
}