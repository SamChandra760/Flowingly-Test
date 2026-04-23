using FlowinglyImport.Api.Common;
using FlowinglyImport.Api.Models;

namespace FlowinglyImport.Api.Services;

public interface IImportParsingService
{
    Result<ImportParseResponse> Parse(string text);
}
