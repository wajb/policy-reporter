using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using PolicyReporter.DataHandling.Models;

namespace PolicyReporter.DataHandling.Tests;

[TestClass]
public sealed class PolicyRepositoryTests
{
    private PolicyRepository _subject = null!;
    private PolicyDbContext _dbContext = null!;

    [TestInitialize]
    public void SetUp()
    {
        DbContextOptions<PolicyDbContext> dbContextOptions = new DbContextOptionsBuilder<PolicyDbContext>()
            .UseInMemoryDatabase(databaseName: "Policies")
            .Options;

        _dbContext = new PolicyDbContext(dbContextOptions);
        _subject = new PolicyRepository(_dbContext);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task Upsert_ReturnsDelegate()
    {
        Policy policy = new()
        {
            PolicyId = "p15",
            SourceId = 3,
            Currency = "USD",
            Customer = "Acme",
            InsuredAmount = 10,
        };

        Func<Task> result = await _subject.Upsert(policy);

        using AssertionScope _ = new();
        result.Should().NotBeNull();
        _dbContext.ChangeTracker.HasChanges().Should().BeTrue();
    }

    [TestMethod]
    public async Task Upsert_InvokingReturnedDelegateCommitsChangesToDatabase()
    {
        Policy policy = new()
        {
            PolicyId = "p15",
            SourceId = 3,
            Currency = "USD",
            Customer = "Acme",
            InsuredAmount = 10,
        };
        Func<Task> flush = await _subject.Upsert(policy);

        await flush();

        _dbContext.ChangeTracker.HasChanges().Should().BeFalse();
    }

    [TestMethod]
    public async Task Upsert_WhenRecordWithMatchingPolicyIdAndSourceIdExists_UpdatesRecords()
    {
        _dbContext.Policies.Add(new()
        {
            PolicyId = "p01",
            SourceId = 1,
            Currency = "USD",
            Customer = "Acme",
            InsuredAmount = 100,
        });
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
        Policy policy = new()
        {
            PolicyId = "p01",
            SourceId = 1,
            Currency = "GBP",
            Customer = "Emca",
            InsuredAmount = 10000,
        };

        Func<Task> flush = await _subject.Upsert(policy);
        await flush();

        _dbContext.Policies.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(policy);
    }

    [TestMethod]
    [DataRow("p02", 2, DisplayName = "Differing PolicyId and SourceId")]
    [DataRow("p01", 2, DisplayName = "Differing SourceId only")]
    [DataRow("p02", 1, DisplayName = "Differing PolicyId only")]
    public async Task Upsert_WhenRecordWithMatchingPolicyIdAndSourceIdDoesNotExist_AddsRecords(string policyId, int sourceId)
    {
        _dbContext.Policies.Add(new()
        {
            PolicyId = "p01",
            SourceId = 1,
            Currency = "USD",
            Customer = "Acme",
            InsuredAmount = 100,
        });
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
        Policy policy = new()
        {
            PolicyId = policyId,
            SourceId = sourceId,
            Currency = "GBP",
            Customer = "Emca",
            InsuredAmount = 10000,
        };

        Func<Task> flush = await _subject.Upsert(policy);
        await flush();

        _dbContext.Policies.Should().HaveCount(2)
            .And.ContainEquivalentOf(policy);
    }

    [TestMethod]
    public async Task GetAllPolicies_ReturnsAllPolicies()
    {
        List<Policy> policies =
        [
            new()
            {
                PolicyId = "p01",
                SourceId = 1,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1000,
            },
            new()
            {
                PolicyId = "p11",
                SourceId = 2,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1001,
            },
            new()
            {
                PolicyId = "p12",
                SourceId = 2,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1002,
            }
        ];
        _dbContext.Policies.AddRange(policies);
        await _dbContext.SaveChangesAsync();

        List<Policy> result = await _subject.GetAllPolicies();

        result.Should().HaveCount(3).And.BeEquivalentTo(policies);
    }

    [TestMethod]
    public async Task GetPoliciesBySourceId_ReturnsAllPoliciesWithMatchingSourceId()
    {
        List<Policy> policies =
        [
            new()
            {
                PolicyId = "p01",
                SourceId = 1,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1000,
            },
            new()
            {
                PolicyId = "p11",
                SourceId = 2,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1001,
            },
            new()
            {
                PolicyId = "p12",
                SourceId = 2,
                Currency = "USD",
                Customer = "Acme",
                InsuredAmount = 1002,
            }
        ];
        _dbContext.Policies.AddRange(policies);
        await _dbContext.SaveChangesAsync();

        List<Policy> result = await _subject.GetPoliciesBySourceId(2);

        result.Should().HaveCount(2).And.BeEquivalentTo(policies.Skip(1));
    }
}
