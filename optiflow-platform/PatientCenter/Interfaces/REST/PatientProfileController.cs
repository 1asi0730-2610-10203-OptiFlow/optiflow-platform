using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST;

[ApiController]
[Route("api/v1/patient-center")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Center")]
[Authorize]
[AllowWithoutAccount]
[AllowWithoutSubscription]
public class PatientProfileController(
    IPatientQueryService patientQueryService,
    ILogger<PatientProfileController> logger)
    : ControllerBase
{
    [HttpGet("patients/by-email")]
    [SwaggerOperation(
        Summary = "Gets a patient by email",
        Description = "Returns the patient data for the authenticated patient user",
        OperationId = "GetPatientByEmail")]
    [SwaggerResponse(200, "Patient found")]
    [SwaggerResponse(404, "Patient not found")]
    public async Task<ActionResult> GetPatientByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await patientQueryService.Handle(
                new GetPatientByEmailQuery(email), cancellationToken);

            if (patient is null) return NotFound();

            return Ok(new
            {
                id        = patient.Id,
                firstName = patient.FirstName,
                lastName  = patient.LastName,
                email     = patient.Email,
                phone     = patient.Phone,
                birthDate = patient.BirthDate.ToString("yyyy-MM-dd")
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient by email {Email}", email);
            return Problem(statusCode: 500);
        }
    }
}