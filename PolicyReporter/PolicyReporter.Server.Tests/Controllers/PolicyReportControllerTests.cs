using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.Server.Controllers;
using PolicyReporter.Server.DTOs;
using PolicyReporter.Server.Models;
using PolicyReporter.Server.Services;

namespace PolicyReporter.Server.Tests.Controllers;

[TestClass]
public sealed class PolicyReportControllerTests
{
    private PolicyReportController _subject = null!;
    private IPolicyReportService _policyReportService = null!;

    [TestInitialize]
    public void SetUp()
    {
        _policyReportService = Mock.Of<IPolicyReportService>();
        IServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton<PolicyReportController>()
            .AddSingleton<IPolicyReportService>(_policyReportService)
            .AddAutoMapper(typeof(MappingProfile))
            .BuildServiceProvider();

        _subject = serviceProvider.GetRequiredService<PolicyReportController>();
    }

    [TestMethod]
    public async Task Get_WhenActiveOnlyIsFalse_ReturnsReportOfAllMappedPolicies()
    {
        Report report = new()
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 2,
                AveragePolicyDurationDays = 100,
                CustomerCount = 2,
                InsuredAmountTotal = 123456,
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
        };
        Mock.Get(_policyReportService)
            .Setup(s => s.GetPolicies("14"))
            .ReturnsAsync(report)
            .Verifiable();

        ReportDto result = await _subject.Get(broker: "14", activeOnly: false);

        result.Should().BeEquivalentTo(new ReportDto
        {
            Statistics = new PolicyStatisticsDto
            {
                PolicyCount = 2,
                AveragePolicyDurationDays = 100,
                CustomerCount = 2,
                InsuredAmountTotal = 123456,
            },
            Policies = [
                new PolicyDto
                {
                    PolicyId = "p001",
                    SourceId = 1,
                    Currency = "GBP",
                    Customer = "Customer1",
                    InsuredAmount = 50,
                    StartDate = new DateOnly(2023, 6, 1),
                    EndDate = new DateOnly(2024, 6, 1),
                },
                new PolicyDto
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
        Mock.Get(_policyReportService).VerifyAll();
    }

    [TestMethod]
    public async Task Get_WhenActiveOnlyIsTrue_ReturnsReportOfActiveMappedPolicies()
    {
        Report report = new()
        {
            Statistics = new PolicyStatistics
            {
                PolicyCount = 1,
                AveragePolicyDurationDays = 0,
                CustomerCount = 1,
                InsuredAmountTotal = 123456,
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
        };
        Mock.Get(_policyReportService)
            .Setup(s => s.GetActivePolicies("63"))
            .ReturnsAsync(report)
            .Verifiable();

        ReportDto result = await _subject.Get(broker: "63", activeOnly: true);

        result.Should().BeEquivalentTo(new ReportDto
        {
            Statistics = new PolicyStatisticsDto
            {
                PolicyCount = 1,
                AveragePolicyDurationDays = 0,
                CustomerCount = 1,
                InsuredAmountTotal = 123456,
            },
            Policies = [
                new PolicyDto
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
        Mock.Get(_policyReportService).VerifyAll();
    }
}
