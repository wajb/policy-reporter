using PolicyReporter.DataHandling;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.Server.Models;

namespace PolicyReporter.Server.Services;

/// <inheritdoc cref="IPolicyReportService"/>
/// <param name="policyRepo">Repository for policies.</param>
public class PolicyReportService(IPolicyRepository policyRepo) : IPolicyReportService
{
    private readonly IPolicyRepository _policyRepo = policyRepo;
    
    /// <inheritdoc/>
    public Task<Report> GetPolicies(string? brokerNameOrId = null) => GetPolicies(brokerNameOrId, filter: null);

    /// <inheritdoc/>
    public Task<Report> GetActivePolicies(string? brokerNameOrId = null)
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

        return GetPolicies(brokerNameOrId, filter: p => p.StartDate <= currentDate && p.EndDate > currentDate);
    }

    private async Task<Report> GetPolicies(string? brokerNameOrId, Func<Policy, bool>? filter)
    {
        List<Policy> policies = ParseSourceId(brokerNameOrId, out int sourceId)
            ? await _policyRepo.GetPoliciesBySourceId(sourceId)
            : await _policyRepo.GetAllPolicies();

        if (filter is not null)
            policies = policies.Where(filter).ToList();

        return new Report
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = policies.Count,
                CustomerCount = policies.GroupBy(p => p.Customer).Count(),
                InsuredAmountTotal = policies.Sum(p => p.InsuredAmount),
                AveragePolicyDurationDays = GetAveragePolicyDurationDays(policies),
            },
            Policies = policies,
        };
    }

    private static bool ParseSourceId(string? brokerNameOrId, out int brokerId)
    {
        brokerId = default;

        if (brokerNameOrId is null)
            return false;

        if (int.TryParse(brokerNameOrId, out brokerId))
            return true;

        // TODO look up broker names to find source ID

        return false;
    }

    private static int GetAveragePolicyDurationDays(List<Policy> policies)
    {
        List<Policy> policiesWithDates = policies
            .Where(p => p.StartDate.HasValue && p.EndDate.HasValue)
            .ToList();

        return policiesWithDates.Any()
            ? (int) policiesWithDates.Average(p => p.EndDate!.Value.DayNumber - p.StartDate!.Value.DayNumber)
            : 0;
    }
}
