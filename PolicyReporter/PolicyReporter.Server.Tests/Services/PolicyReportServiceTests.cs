using FluentAssertions;
using Moq;
using PolicyReporter.DataHandling;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.Server.Models;
using PolicyReporter.Server.Services;

namespace PolicyReporter.Server.Tests.Services;

[TestClass]
public sealed class PolicyReportServiceTests
{
    private PolicyReportService _subject = null!;
    private IPolicyRepository _policyRepo = null!;
    [TestInitialize]
    public void StartUp()
    {
        _policyRepo = Mock.Of<IPolicyRepository>();
        Mock.Get(_policyRepo)
            .Setup(r => r.GetAllPolicies())
            .ReturnsAsync(new List<Policy>
            {
                new Policy
                {
                    PolicyId = "p001",
                    SourceId = 1,
                    Currency = "GBP",
                    Customer = "Customer1",
                    InsuredAmount = 50,
                    StartDate = new DateOnly(2023, 6, 1),
                    EndDate = new DateOnly(2024, 6, 1),
                },
                new Policy
                {
                    PolicyId = "p002",
                    SourceId = 2,
                    Currency = "GBP",
                    Customer = "Customer2",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            });
        Mock.Get(_policyRepo)
            .Setup(r => r.GetPoliciesBySourceId(3))
            .ReturnsAsync(new List<Policy>
            {
                new Policy
                {
                    PolicyId = "p003",
                    SourceId = 3,
                    Currency = "GBP",
                    Customer = "Customer5",
                    InsuredAmount = 250,
                    StartDate = new DateOnly(2022, 6, 1),
                    EndDate = new DateOnly(2024, 6, 1),
                },
                new Policy
                {
                    PolicyId = "p004",
                    SourceId = 3,
                    Currency = "GBP",
                    Customer = "Customer6",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            });
        _subject = new PolicyReportService(_policyRepo);
    }

    [TestMethod]
    public async Task GetPolicies_WhenBrokerIsNotSpecified_ReturnsReportOfAllPolicies()
    {
        Report result = await _subject.GetPolicies(brokerNameOrId: null);

        result.Should().BeEquivalentTo(new Report
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 2,
                AveragePolicyDurationDays = 366,
                CustomerCount = 2,
                InsuredAmountTotal = 200,
            },
            Policies = [
                new Policy
                {
                    PolicyId = "p001",
                    SourceId = 1,
                    Currency = "GBP",
                    Customer = "Customer1",
                    InsuredAmount = 50,
                    StartDate = new DateOnly(2023, 6, 1),
                    EndDate = new DateOnly(2024, 6, 1),
                },
                new Policy
                {
                    PolicyId = "p002",
                    SourceId = 2,
                    Currency = "GBP",
                    Customer = "Customer2",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            ]
        });
    }

    [TestMethod]
    public async Task GetPolicies_WhenBrokerIsSpecified_ReturnsReportOfPoliciesAssociatedWithBroker()
    {
        Report result = await _subject.GetPolicies(brokerNameOrId: "3");

        result.Should().BeEquivalentTo(new Report
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 2,
                AveragePolicyDurationDays = 731,
                CustomerCount = 2,
                InsuredAmountTotal = 400,
            },
            Policies = [
                new Policy
                {
                    PolicyId = "p003",
                    SourceId = 3,
                    Currency = "GBP",
                    Customer = "Customer5",
                    InsuredAmount = 250,
                    StartDate = new DateOnly(2022, 6, 1),
                    EndDate = new DateOnly(2024, 6, 1),
                },
                new Policy
                {
                    PolicyId = "p004",
                    SourceId = 3,
                    Currency = "GBP",
                    Customer = "Customer6",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            ]
        });
    }

    [TestMethod]
    public async Task GetPolicies_WhenZeroPoliciesHaveStartAndEndDates_ReturnsReportWithAveragePolicyDurationStatisticOfZero()
    {
        Mock.Get(_policyRepo).Reset();
        Mock.Get(_policyRepo)
            .Setup(r => r.GetAllPolicies())
            .ReturnsAsync(new List<Policy>
            {
                new Policy
                {
                    PolicyId = "p098",
                    SourceId = 1,
                    Currency = "GBP",
                    Customer = "Customer98",
                    InsuredAmount = 2,
                    StartDate = null,
                    EndDate = new DateOnly(2030, 1, 1),
                },
                new Policy
                {
                    PolicyId = "p099",
                    SourceId = 2,
                    Currency = "GBP",
                    Customer = "Customer99",
                    InsuredAmount = 6,
                    StartDate = new DateOnly(2020, 1, 1),
                    EndDate = null,
                }
            });
        Report result = await _subject.GetPolicies(brokerNameOrId: null);

        result.Statistics.Should().BeEquivalentTo(new PolicyStatistics
        {
            PolicyCount = 2,
            AveragePolicyDurationDays = 0,
            CustomerCount = 2,
            InsuredAmountTotal = 8,
        });
    }

    [TestMethod]
    public async Task GetActivePolicies_WhenBrokerIsNotSpecified_ReturnsReportOfAllPoliciesWithStartAndEndDatesSpanningCurrentDate()
    {
        Report result = await _subject.GetActivePolicies(brokerNameOrId: null);

        result.Should().BeEquivalentTo(new Report
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 1,
                AveragePolicyDurationDays = 0,
                CustomerCount = 1,
                InsuredAmountTotal = 150,
            },
            Policies = [
                new Policy
                {
                    PolicyId = "p002",
                    SourceId = 2,
                    Currency = "GBP",
                    Customer = "Customer2",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            ]
        });
    }

    [TestMethod]
    public async Task GetActivePolicies_WhenBrokerIsSpecified_ReturnsReportOfPoliciesAssociatedWithBrokerWithStartAndEndDatesSpanningCurrentDate()
    {
        Report result = await _subject.GetActivePolicies(brokerNameOrId: "3");

        result.Should().BeEquivalentTo(new Report
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 1,
                AveragePolicyDurationDays = 0,
                CustomerCount = 1,
                InsuredAmountTotal = 150,
            },
            Policies = [
                new Policy
                {
                    PolicyId = "p004",
                    SourceId = 3,
                    Currency = "GBP",
                    Customer = "Customer6",
                    InsuredAmount = 150,
                    StartDate = new DateOnly(2023, 1, 1),
                    EndDate = null,
                }
            ]
        });
    }
}
