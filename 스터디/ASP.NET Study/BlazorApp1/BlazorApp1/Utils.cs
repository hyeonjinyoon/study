using System.Text;
namespace BlazorApp1;

public static class Utils
{
    public static string ToSnakeCase(this string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        if (text.Length < 2)
            return text;

        var builder = new StringBuilder(text.Length * 2);
        builder.Append(char.ToLowerInvariant(text[0]));
        for (var i = 1; i < text.Length; ++i)
        {
            var c = text[i];
            if (char.IsUpper(c))
            {
                builder
                    .Append('_')
                    .Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    public static string SnakeToPascal(this string text)
        => string.Concat(text
            .Split('_')
            .Select(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase));
}
