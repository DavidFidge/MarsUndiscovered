using Microsoft.Extensions.Configuration;

namespace MarsUndiscovered.Extensions;

public static class ConfigurationExtensions
{
    public static string GetValueOrEnvironmentVariable(this IConfiguration configuration, string variableName)
    {
        var value = configuration?.GetValue<string>(variableName, null);

        if (String.IsNullOrEmpty(value))
            value = Environment.GetEnvironmentVariable(variableName);

        return value;
    }
}