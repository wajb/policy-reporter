using AutoMapper;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.Server.DTOs;
using PolicyReporter.Server.Models;

namespace PolicyReporter.Server;

/// <summary>
/// Provides configuration for type mapping.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Creates a <see cref="MappingProfile"/> instance.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Policy, PolicyDto>();
        CreateMap<PolicyStatisticsDto, PolicyStatisticsDto>();
        CreateMap<Report, ReportDto>();
    }
}
