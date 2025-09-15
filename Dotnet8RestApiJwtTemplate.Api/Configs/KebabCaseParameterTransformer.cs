using System.Text.RegularExpressions;

namespace Dotnet8RestApiJwtTemplate.Api.Configs;

public sealed class KebabCaseParameterTransformer : IOutboundParameterTransformer, IParameterPolicy
{
    private static readonly Regex CamelToKebab =
        new("([a-z0-9])([A-Z])", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public string? TransformOutbound(object? value)
    {
        if (value is null) return null;
        var s = value.ToString()!;
        s = CamelToKebab.Replace(s, "$1-$2").Replace('_', '-');
        return s.ToLowerInvariant();
    }
}