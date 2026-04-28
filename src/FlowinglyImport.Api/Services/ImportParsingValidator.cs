using System.Globalization;
using FlowinglyImport.Api.Common;

namespace FlowinglyImport.Api.Services;

public class ImportParsingValidator : IImportParsingValidator
{
    public IReadOnlyList<ValidationError> Validate(IReadOnlyDictionary<string, string> fields)
    {
        var errors = new List<ValidationError>();

        // Whole message to be rejected when <total> is missing.
        if (!fields.TryGetValue("total", out var totalText) || string.IsNullOrWhiteSpace(totalText))
        {
            errors.Add(new ValidationError("total", "Missing required <total> tag."));
            return errors;
        }

        if (!decimal.TryParse(
            totalText,
            NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out _))
        {
            errors.Add(new ValidationError("total", "<total> must contain a valid numeric value."));
        }

        return errors;
    }
}
