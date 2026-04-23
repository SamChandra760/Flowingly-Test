namespace FlowinglyImport.Api.Models;

public class ImportParseResponse
{
    public string CostCentre { get; set; } = "UNKNOWN";

    public decimal TotalIncludingTax { get; set; }

    public decimal TotalExcludingTax { get; set; }

    public decimal SalesTax { get; set; }

    public decimal TaxRate { get; set; }

    public Dictionary<string, string> Fields { get; set; } = [];
}
