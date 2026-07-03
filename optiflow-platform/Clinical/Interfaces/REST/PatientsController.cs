using System.Net.Mime;
using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.Clinical.Interfaces.REST.Resources;
using optiflow_platform.Clinical.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Clinical.Interfaces.REST;

/// <summary>Patients controller — manages patient registration and updates.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patients")]
[Authorize]
public class PatientsController(
    IPatientCommandService patientCommandService,
    IPatientQueryService patientQueryService,
    ILogger<PatientsController> logger)
    : ControllerBase
{
    /// <summary>Creates a new patient and automatically opens a clinical record for them.</summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a patient",
        Description = "Registers a new patient and auto-creates a linked clinical record",
        OperationId = "CreatePatient")]
    [SwaggerResponse(201, "Patient created", typeof(PatientResource))]
    [SwaggerResponse(400, "Validation error or duplicate DNI", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreatePatient(
        [FromBody] CreatePatientResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreatePatientCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result  = await patientCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Patient, CreatePatientError>.Success s =>
                    CreatedAtAction(nameof(GetPatientById),
                        new { id = s.Value.Id },
                        PatientResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
                Result<Patient, CreatePatientError>.Failure { Error: CreatePatientError.DniAlreadyExists } =>
                    BadRequest($"A patient with DNI '{resource.Dni}' already exists."),
                _ => Problem(title: "Unexpected error", detail: "Could not create patient.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while creating patient {Dni}", resource.Dni);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating patient {Dni}", resource.Dni);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while creating the patient.", statusCode: 500);
        }
    }

    /// <summary>Gets all patients.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all patients", OperationId = "GetAllPatients")]
    [SwaggerResponse(200, "List of patients", typeof(IEnumerable<PatientResource>))]
    public async Task<ActionResult> GetAllPatients(CancellationToken cancellationToken = default)
    {
        var result = await patientQueryService.Handle(new GetAllPatientsQuery(), cancellationToken);
        return Ok(result.Select(PatientResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a patient by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a patient by id", OperationId = "GetPatientById")]
    [SwaggerResponse(200, "Patient found", typeof(PatientResource))]
    [SwaggerResponse(404, "Patient not found")]
    public async Task<ActionResult> GetPatientById(int id, CancellationToken cancellationToken = default)
    {
        var result = await patientQueryService.Handle(new GetPatientByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(PatientResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>Updates a patient's personal data.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Updates a patient", OperationId = "UpdatePatient")]
    [SwaggerResponse(200, "Patient updated", typeof(PatientResource))]
    [SwaggerResponse(400, "Validation error or duplicate DNI", typeof(string))]
    [SwaggerResponse(404, "Patient not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdatePatient(
        int id,
        [FromBody] UpdatePatientResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdatePatientCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var result  = await patientCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Patient, UpdatePatientError>.Success s =>
                    Ok(PatientResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
                Result<Patient, UpdatePatientError>.Failure { Error: UpdatePatientError.PatientNotFound } =>
                    NotFound(),
                Result<Patient, UpdatePatientError>.Failure { Error: UpdatePatientError.DniAlreadyExists } =>
                    BadRequest($"A patient with DNI '{resource.Dni}' already exists."),
                _ => Problem(title: "Unexpected error", detail: "Could not update patient.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while updating patient {PatientId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating patient {PatientId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while updating the patient.", statusCode: 500);
        }
    }
}
