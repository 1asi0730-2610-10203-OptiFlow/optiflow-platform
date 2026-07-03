using System.Net.Mime;
using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.Clinical.Interfaces.REST.Resources;
using optiflow_platform.Clinical.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Clinical.Interfaces.REST;

/// <summary>Clinical records controller — retrieves clinical records per patient.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Clinical Records")]
[Authorize]
public class ClinicalRecordsController(
    IClinicalRecordQueryService clinicalRecordQueryService,
    ILogger<ClinicalRecordsController> logger)
    : ControllerBase
{
    /// <summary>Gets all clinical records.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all clinical records", OperationId = "GetAllClinicalRecords")]
    [SwaggerResponse(200, "List of clinical records", typeof(IEnumerable<ClinicalRecordResource>))]
    public async Task<ActionResult> GetAllClinicalRecords(CancellationToken cancellationToken = default)
    {
        var result = await clinicalRecordQueryService.Handle(new GetAllClinicalRecordsQuery(), cancellationToken);
        return Ok(result.Select(ClinicalRecordResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a clinical record by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a clinical record by id", OperationId = "GetClinicalRecordById")]
    [SwaggerResponse(200, "Clinical record found", typeof(ClinicalRecordResource))]
    [SwaggerResponse(404, "Clinical record not found")]
    public async Task<ActionResult> GetClinicalRecordById(int id, CancellationToken cancellationToken = default)
    {
        var result = await clinicalRecordQueryService.Handle(new GetClinicalRecordByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(ClinicalRecordResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>Gets the clinical record for a specific patient.</summary>
    [HttpGet("by-patient/{patientId:int}")]
    [SwaggerOperation(Summary = "Gets a clinical record by patient id", OperationId = "GetClinicalRecordByPatientId")]
    [SwaggerResponse(200, "Clinical record found", typeof(ClinicalRecordResource))]
    [SwaggerResponse(404, "No clinical record found for this patient")]
    public async Task<ActionResult> GetClinicalRecordByPatientId(int patientId, CancellationToken cancellationToken = default)
    {
        var result = await clinicalRecordQueryService.Handle(
            new GetClinicalRecordByPatientIdQuery(patientId), cancellationToken);
        if (result is null) return NotFound();
        return Ok(ClinicalRecordResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}

/// <summary>Prescriptions controller — manages optical prescriptions linked to clinical records.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Prescriptions")]
[Authorize]
public class PrescriptionsController(
    IPrescriptionCommandService prescriptionCommandService,
    IPrescriptionQueryService prescriptionQueryService,
    ILogger<PrescriptionsController> logger)
    : ControllerBase
{
    /// <summary>Creates a new prescription for a clinical record.</summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a prescription",
        Description = "Adds a new optical prescription to the given clinical record",
        OperationId = "CreatePrescription")]
    [SwaggerResponse(201, "Prescription created", typeof(PrescriptionResource))]
    [SwaggerResponse(400, "Validation error", typeof(string))]
    [SwaggerResponse(404, "Clinical record not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreatePrescription(
        [FromBody] CreatePrescriptionResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreatePrescriptionCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result  = await prescriptionCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Prescription, CreatePrescriptionError>.Success s =>
                    CreatedAtAction(nameof(GetPrescriptionsByRecord),
                        new { recordId = s.Value.ClinicalRecordId },
                        PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
                Result<Prescription, CreatePrescriptionError>.Failure
                    { Error: CreatePrescriptionError.ClinicalRecordNotFound } =>
                    NotFound($"Clinical record {resource.ClinicalRecordId} not found."),
                _ => Problem(title: "Unexpected error", detail: "Could not create prescription.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while creating prescription for record {RecordId}", resource.ClinicalRecordId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating prescription for record {RecordId}", resource.ClinicalRecordId);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while creating the prescription.", statusCode: 500);
        }
    }

    /// <summary>Gets all prescriptions.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all prescriptions", OperationId = "GetAllPrescriptions")]
    [SwaggerResponse(200, "List of prescriptions", typeof(IEnumerable<PrescriptionResource>))]
    public async Task<ActionResult> GetAllPrescriptions(CancellationToken cancellationToken = default)
    {
        var result = await prescriptionQueryService.Handle(new GetAllPrescriptionsQuery(), cancellationToken);
        return Ok(result.Select(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets all prescriptions for a clinical record.</summary>
    [HttpGet("by-record/{recordId:int}")]
    [SwaggerOperation(Summary = "Gets prescriptions by clinical record id", OperationId = "GetPrescriptionsByRecord")]
    [SwaggerResponse(200, "List of prescriptions for the record", typeof(IEnumerable<PrescriptionResource>))]
    public async Task<ActionResult> GetPrescriptionsByRecord(int recordId, CancellationToken cancellationToken = default)
    {
        var result = await prescriptionQueryService.Handle(
            new GetPrescriptionsByRecordIdQuery(recordId), cancellationToken);
        return Ok(result.Select(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
