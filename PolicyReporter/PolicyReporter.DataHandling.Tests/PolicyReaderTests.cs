using FluentAssertions;
using Moq;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.DataHandling.Parsers;

namespace PolicyReporter.DataHandling.Tests;

[TestClass]
public sealed class PolicyReaderTests
{
    private PolicyReader _subject = null!;
    private IPolicyRepository _policyRepo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _policyRepo = Mock.Of<IPolicyRepository>();
        _subject = new PolicyReader(_policyRepo);
    }

    [TestMethod]
    public void Read_ReturnsSelf()
    {
        IPolicySource policySource = Mock.Of<IPolicySource>();
        IPolicyParserStrategy strategy = Mock.Of<IPolicyParserStrategy>();

        IPolicyReader result = _subject.Read(policySource, strategy);

        result.Should().BeSameAs(_subject);
    }

    [TestMethod]
    public async Task Execute_WhenReadHasNotBeenCalled_ReturnsDefaultResult()
    {
        ReadResults result = await _subject.Execute();

        result.Should().Be(new ReadResults(SourceCount: 0, PolicyCount: 0));
    }

    [TestMethod]
    public async Task Execute_WhenReadHasBeenCalled_ReturnsCumulativeResult()
    {
        _ = SetUpExampleSources();

        ReadResults result = await _subject.Execute();

        result.Should().Be(new ReadResults(SourceCount: 2, PolicyCount: 3));
    }

    [TestMethod]
    public async Task Execute_WhenReadHasBeenCalled_ParsesPoliciesFromAllRequestedSources()
    {
        List<(IPolicySource PolicySource, IPolicyParserStrategy Strategy)> mocked = SetUpExampleSources();

        ReadResults result = await _subject.Execute();

        Mock.Get(mocked[0].Strategy).Verify(p => p.ParsePolicies(mocked[0].PolicySource), Times.Once);
        Mock.Get(mocked[1].Strategy).Verify(p => p.ParsePolicies(mocked[1].PolicySource), Times.Once);
    }

    [TestMethod]
    public async Task Execute_WhenReadHasBeenCalled_UpsertsNormalisedRecordsInDestinationRepository()
    {
        var flushDelegate = Mock.Of<Func<Task>>();
        Mock.Get(_policyRepo).Setup(r => r.Upsert(It.IsAny<Policy>())).ReturnsAsync(flushDelegate);
        List<(IPolicySource PolicySource, IPolicyParserStrategy Strategy)> mocked = SetUpExampleSources();

        ReadResults result = await _subject.Execute();

        Mock.Get(_policyRepo)
            .Verify(p => p.Upsert(new Policy
             {
                 SourceId = 1,
                 PolicyId = "POL1",
                 InsuredAmount = 10000,
                 StartDate = DateOnly.Parse("2025/01/28"),
                 EndDate = DateOnly.Parse("2026/01/28"),
                 Currency = "GBP",
                 Customer = "Acme",
             }), Times.Once);
        Mock.Get(_policyRepo)
            .Verify(p => p.Upsert(new Policy
            {
                SourceId = 2,
                PolicyId = "POL2",
                InsuredAmount = 20000,
                StartDate = DateOnly.Parse("2025/01/29"),
                EndDate = DateOnly.Parse("2026/01/29"),
                Currency = "USD",
                Customer = "Fresh Co",
            }), Times.Once);
        Mock.Get(_policyRepo)
            .Verify(p => p.Upsert(new Policy
            {
                SourceId = 2,
                PolicyId = "POL3",
                InsuredAmount = 30000,
                StartDate = DateOnly.Parse("2025/01/29"),
                EndDate = DateOnly.Parse("2026/01/29"),
                Currency = "USD",
                Customer = "SpamWorks",
            }), Times.Once);
    }

    private List<(IPolicySource PolicySource, IPolicyParserStrategy Strategy)> SetUpExampleSources()
    {
        List<(IPolicySource, IPolicyParserStrategy)> mocked = new();

        IPolicySource policySource = Mock.Of<IPolicySource>();
        IPolicyParserStrategy strategy = Mock.Of<IPolicyParserStrategy>();
        mocked.Add((policySource, strategy));

        Mock.Get(strategy)
            .Setup(s => s.ParsePolicies(policySource))
            .Returns(new[] {
                new Policy
                {
                    SourceId = 1,
                    PolicyId = "POL1",
                    InsuredAmount = 10000,
                    StartDate = DateOnly.Parse("2025/01/28"),
                    EndDate = DateOnly.Parse("2026/01/28"),
                    Currency = "GBP",
                    Customer = "Acme",
                }
            }.ToAsyncEnumerable());

        _ = _subject.Read(policySource, strategy);

        policySource = Mock.Of<IPolicySource>();
        strategy = Mock.Of<IPolicyParserStrategy>();
        mocked.Add((policySource, strategy));

        Mock.Get(strategy)
            .Setup(s => s.ParsePolicies(policySource))
            .Returns(new[] {
                new Policy
                {
                    SourceId = 2,
                    PolicyId = "POL2",
                    InsuredAmount = 20000,
                    StartDate = DateOnly.Parse("2025/01/29"),
                    EndDate = DateOnly.Parse("2026/01/29"),
                    Currency = "USD",
                    Customer = "Fresh Co",
                },
                new Policy
                {
                    SourceId = 2,
                    PolicyId = "POL3",
                    InsuredAmount = 30000,
                    StartDate = DateOnly.Parse("2025/01/29"),
                    EndDate = DateOnly.Parse("2026/01/29"),
                    Currency = "USD",
                    Customer = "SpamWorks",
                }
            }.ToAsyncEnumerable());

        _ = _subject.Read(policySource, strategy);

        return mocked;
    }
}
