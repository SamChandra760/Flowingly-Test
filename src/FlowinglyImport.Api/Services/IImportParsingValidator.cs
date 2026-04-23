using FlowinglyImport.Api.Common;

namespace FlowinglyImport.Api.Services;

public interface IImportParsingValidator
{
    IReadOnlyList<ValidationError> Validate(IReadOnlyDictionary<string, string> fields);
}
