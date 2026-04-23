using System.Globalization;
using FlowinglyImport.Api.Common;
using FlowinglyImport.Api.Models;
using FlowinglyImport.Api.Options;
using FlowinglyImport.Api.Parsing;
using Microsoft.Extensions.Options;

namespace FlowinglyImport.Api.Services;

public class ImportParsingService : IImportParsingService
{
    private readonly MarkedTextParser parser;
    private readonly IImportParsingValidator validator;
    private readonly IOptions<TaxOptions> taxOptions;

    public ImportParsingService(
        MarkedTextParser parser,
        IImportParsingValidator validator,
        IOptions<TaxOptions> taxOptions)
    {
        this.parser = parser;
        this.validator = validator;
        this.taxOptions = taxOptions;
    }

    public Result<ImportParseResponse> Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result<ImportParseResponse>.Failure(
                [new ValidationError("text", "Text is required.")]);
        }

        var parseResult = parser.Parse(text);
        if (!parseResult.IsSuccess)
        {
            return Result<ImportParseResponse>.Failure(parseResult.Errors);
        }

        var fields = BuildFields(parseResult.Value ?? []);
        var validationErrors = validator.Validate(fields);

        if (validationErrors.Count > 0)
        {
            return Result<ImportParseResponse>.Failure(validationErrors);
        }

        var taxRate = taxOptions.Value.Rate;
        if (taxRate <= 0)
        {
            return Result<ImportParseResponse>.Failure(
                [new ValidationError("taxRate", "Tax rate must be configured before parsing imports.")]);
        }

        var totalIncludingTax = ParseTotal(fields["total"]);
        var totalExcludingTax = decimal.Round(totalIncludingTax / (1 + taxRate), 2, MidpointRounding.AwayFromZero);
        var salesTax = decimal.Round(totalIncludingTax - totalExcludingTax, 2, MidpointRounding.AwayFromZero);

        var response = new ImportParseResponse
        {
            CostCentre = fields.GetValueOrDefault("cost_centre", "UNKNOWN"),
            TotalIncludingTax = totalIncludingTax,
            TotalExcludingTax = totalExcludingTax,
            SalesTax = salesTax,
            TaxRate = taxRate,
            Fields = fields
        };

        return Result<ImportParseResponse>.Success(response);
    }

    private static Dictionary<string, string> BuildFields(IReadOnlyList<ParsedTag> tags)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var tag in tags)
        {
            fields[tag.Name] = tag.Value;
        }

        return fields;
    }

    private static decimal ParseTotal(string value)
    {
        return decimal.Parse(
            value,
            NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture);
    }
}
