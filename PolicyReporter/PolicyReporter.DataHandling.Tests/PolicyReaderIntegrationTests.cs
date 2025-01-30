using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolicyReporter.DataHandling.Extensions;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.DataHandling.Parsers;

namespace PolicyReporter.DataHandling.Tests;

[TestClass]
public sealed class PolicyReaderIntegrationTests
{
    private const string DataFilePath = "./PolicyReporter.DataHandling.Tests/Data";

    private ServiceProvider _serviceProvider = null!;
    private IPolicyReader _subject = null!;
    private PolicyDbContext _dbContext = null!;

    [TestInitialize]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .AddPolicyHandling()
            .AddDbContext<PolicyDbContext>(options => options.UseInMemoryDatabase(databaseName: "Policies"))
            .BuildServiceProvider();

        _subject = _serviceProvider.GetRequiredService<IPolicyReader>();
        _dbContext = _serviceProvider.GetRequiredService<PolicyDbContext>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task PolicyReader_WithSinglePolicySource_StoresNormalisedPolicyRecords()
    {
        ReadResults readResults = await _subject
            .Read(new FilePolicySource($"{DataFilePath}/testbroker1.csv", sourceId: 1),
                _serviceProvider.GetRequiredService<NumberedPolicyParser>())
            .Execute();

        using AssertionScope _ = new();
        readResults.Should().BeEquivalentTo(new
        {
            PolicyCount = 40,
            SourceCount = 1,
        });
        _dbContext.Policies.Should().HaveCount(40)
            .And.ContainEquivalentOf(new Policy
            {
                SourceId = 1,
                PolicyId = "POL022",
                InsuredAmount = 630000,
                StartDate = DateOnly.Parse("2024/10/14"),
                EndDate = DateOnly.Parse("2025/10/14"),
                Currency = "GBP",
                Customer = "Business XYZ",
            });
    }

    [TestMethod]
    public async Task PolicyReader_WithMultiplePolicySources_StoresNormalisedPolicyRecords()
    {
        ReadResults readResults = await _subject
            .Read(new FilePolicySource($"{DataFilePath}/testbroker1.csv", sourceId: 1),
                _serviceProvider.GetRequiredService<NumberedPolicyParser>())
            .Read(new FilePolicySource($"{DataFilePath}/testbroker2.csv", sourceId: 2),
                _serviceProvider.GetRequiredService<ReferencedPolicyParser>())
            .Execute();

        using AssertionScope _ = new();
        readResults.Should().BeEquivalentTo(new
        {
            PolicyCount = 61,
            SourceCount = 2,
        });
        _dbContext.Policies.Should().HaveCount(61)
            .And.ContainEquivalentOf(new Policy
            {
                SourceId = 1,
                PolicyId = "POL022",
                InsuredAmount = 630000,
                StartDate = DateOnly.Parse("2024/10/14"),
                EndDate = DateOnly.Parse("2025/10/14"),
                Currency = "GBP",
                Customer = "Business XYZ",
            })
            .And.ContainEquivalentOf(new Policy
            {
                SourceId = 2,
                PolicyId = "REF072",
                InsuredAmount = 865000,
                StartDate = DateOnly.Parse("2022/09/20"),
                EndDate = DateOnly.Parse("2023/09/20"),
                Currency = "GBP",
                Customer = "Enterprise S9T",
            });
    }
}
