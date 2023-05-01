namespace MyPersonalizedTodos.API.Extensions;

public static class StringExtensions
{
    // TODO: maybe using the methods as validation rules would be a great idea.
    public static bool IsConvertiableToEnum<TEnum>(this string value) where TEnum : struct
    {
        if (int.TryParse(value, out int convertedValue))
            return Enum.IsDefined(typeof(TEnum), convertedValue);
        else
            return Enum.TryParse<TEnum>(value, ignoreCase: true, out _);
    }

    public static TEnum ConvertToEnum<TEnum>(this string value) where TEnum : struct
    {
        return Enum.Parse<TEnum>(value, ignoreCase: true);
    }
}
