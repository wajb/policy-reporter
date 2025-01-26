using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PolicyReporter.Server.DTOs;
using PolicyReporter.Server.Models;
using PolicyReporter.Server.Services;

namespace PolicyReporter.Server.Controllers;

/// <summary>
/// Controller for accessing policy report data.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PolicyReportController : ControllerBase
{
    private readonly IPolicyReportService _service;
    private readonly IMapper _mapper;

    /// <summary>
    /// Creates a <see cref="PolicyReportController"/> instance.
    /// </summary>
    /// <param name="service">Service to process requests.</param>
    /// <param name="mapper">Mapper for converting data types.</param>
    public PolicyReportController(IPolicyReportService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a report of all matching policies.
    /// </summary>
    /// <param name="broker">Name or identifier of source broker of policies to query for; or <see langword="null"/> for
    /// all.</param>
    /// <param name="activeOnly">Whether to filter for only active policies.</param>
    /// <returns>Policy report.</returns>
    [HttpGet(Name = "GetPolicyReport")]
    public async Task<ReportDto> Get(string? broker, bool activeOnly = false)
    {
        Report report = activeOnly
            ? await _service.GetActivePolicies(broker)
            : await _service.GetPolicies(broker);

        return _mapper.Map<ReportDto>(report);

        // TODO paging?
    }
}
