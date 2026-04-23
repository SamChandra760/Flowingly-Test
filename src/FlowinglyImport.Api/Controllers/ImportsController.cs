using FlowinglyImport.Api.Models;
using FlowinglyImport.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlowinglyImport.Api.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly IImportParsingService importParsingService;
    private readonly ILogger<ImportsController> logger;

    public ImportsController(
        IImportParsingService importParsingService,
        ILogger<ImportsController> logger)
    {
        this.importParsingService = importParsingService;
        this.logger = logger;
    }

    [HttpPost("parse")]
    [ProducesResponseType(typeof(ImportParseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Parse([FromBody] ImportParseRequest request)
    {
        logger.LogInformation("Parsing import text.");

        var result = importParsingService.Parse(request.Text);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        logger.LogWarning(
            "Import parsing failed validation with {ErrorCount} error(s).",
            result.Errors.Count);

        var modelState = new ModelStateDictionary();

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.Field, error.Message);
        }

        return ValidationProblem(
            modelStateDictionary: modelState,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Import text could not be parsed.");
    }
}
