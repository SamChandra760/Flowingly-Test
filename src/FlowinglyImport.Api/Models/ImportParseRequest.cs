using System.ComponentModel.DataAnnotations;

namespace FlowinglyImport.Api.Models;

public class ImportParseRequest
{
    [Required]
    [MinLength(1)]
    public string Text { get; set; } = string.Empty;
}
