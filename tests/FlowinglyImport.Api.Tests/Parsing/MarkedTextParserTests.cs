using FlowinglyImport.Api.Parsing;

namespace FlowinglyImport.Api.Tests.Parsing;

public sealed class MarkedTextParserTests
{
    private readonly MarkedTextParser sut = new();

    [Fact]
    public void Parse_ReturnsLeafTags_WhenTextContainsNestedAndInlineTags()
    {
        var text = """
            <expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal card</payment_method></expense>
            Please reserve <vendor>Seaside Steakhouse</vendor> on <date>27 April 2022</date>.
            """;

        var result = sut.Parse(text);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Contains(result.Value, tag => tag.Name == "cost_centre" && tag.Value == "DEV632");
        Assert.Contains(result.Value, tag => tag.Name == "total" && tag.Value == "35,000");
        Assert.Contains(result.Value, tag => tag.Name == "payment_method" && tag.Value == "personal card");
        Assert.Contains(result.Value, tag => tag.Name == "vendor" && tag.Value == "Seaside Steakhouse");
        Assert.DoesNotContain(result.Value, tag => tag.Name == "expense");
    }

    [Fact]
    public void Parse_ReturnsError_WhenOpeningTagHasNoClosingTag()
    {
        var result = sut.Parse("Please reserve <vendor>Seaside Steakhouse");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Message.Contains("<vendor>"));
    }

    [Fact]
    public void Parse_ReturnsError_WhenOpeningTagHasDifferentClosingTag()
    {
        var result = sut.Parse("<total>35,000</cost_centre>");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Message.Contains("<total>"));
    }
}
