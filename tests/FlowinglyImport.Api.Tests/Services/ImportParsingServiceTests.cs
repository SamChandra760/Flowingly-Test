using FlowinglyImport.Api.Options;
using FlowinglyImport.Api.Parsing;
using FlowinglyImport.Api.Services;

namespace FlowinglyImport.Api.Tests.Services;

public sealed class ImportParsingServiceTests
{
    private readonly ImportParsingService sut = new(
        new MarkedTextParser(),
        new ImportParsingValidator(),
        Microsoft.Extensions.Options.Options.Create(new TaxOptions { Rate = 0.15m }));

    [Fact]
    public void Parse_ReturnsCalculatedTotalsAndFields_WhenTextIsValid()
    {
        var text = """
            Hi Patricia,
            <expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal card</payment_method></expense>
            Please create a reservation at the <vendor>Seaside Steakhouse</vendor> on <date>27 April 2022</date>.
            """;

        var result = sut.Parse(text);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("DEV632", result.Value.CostCentre);
        Assert.Equal(35000m, result.Value.TotalIncludingTax);
        Assert.Equal(30434.78m, result.Value.TotalExcludingTax);
        Assert.Equal(4565.22m, result.Value.SalesTax);
        Assert.Equal("Seaside Steakhouse", result.Value.Fields["vendor"]);
    }

    [Fact]
    public void Parse_DefaultsCostCentre_WhenCostCentreIsMissing()
    {
        var result = sut.Parse("<total>35,000</total>");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("UNKNOWN", result.Value.CostCentre);
    }

    [Fact]
    public void Parse_ReturnsValidationError_WhenTotalIsMissing()
    {
        var result = sut.Parse("<cost_centre>DEV632</cost_centre>");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Field == "total");
    }

    [Fact]
    public void Parse_ReturnsValidationError_WhenTotalIsNotNumeric()
    {
        var result = sut.Parse("<total>abc</total>");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Message.Contains("numeric"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_ReturnsValidationError_WhenTextIsBlank(string text)
    {
        var result = sut.Parse(text);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Field == "text" && error.Message == "Text is required.");
    }

    [Fact]
    public void Parse_ReturnsValidationError_WhenTaxRateIsNotConfigured()
    {
        var serviceWithoutTaxRate = new ImportParsingService(
            new MarkedTextParser(),
            new ImportParsingValidator(),
            Microsoft.Extensions.Options.Options.Create(new TaxOptions()));

        var result = serviceWithoutTaxRate.Parse("<total>35,000</total>");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Field == "taxRate");
    }
}
