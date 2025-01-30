using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Moq;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.DataHandling.Parsers;
using PolicyReporter.DataHandling.Tests.Parsers.TestUtils;
using static PolicyReporter.DataHandling.Tests.Parsers.TestUtils.TestUtils;

namespace PolicyReporter.DataHandling.Tests;

[TestClass]
public sealed class NumberedPolicyParserTests
{
    private NumberedPolicyParser _subject = null!;
    private ICsvReaderFactory _csvReaderFactory = null!;

    [TestInitialize]
    public void SetUp()
    {
        _csvReaderFactory = Mock.Of<ICsvReaderFactory>();
        _subject = new NumberedPolicyParser(_csvReaderFactory);
    }

    [TestMethod]
    public async Task ParsePolicies_StreamsRecordsFromCsvReader()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        List<SourceNumberedPolicy> sourcePolicies =
        [
            new SourceNumberedPolicy { PolicyNumber = "P01", BusinessDescription = "Co1" }
        ];
        CsvReader csvReader = new StubbedCsvReader<SourceNumberedPolicy>(reader)
        {
            Records = sourcePolicies.ToAsyncEnumerable()
        };
        Mock.Get(policySource)
            .Setup(s => s.GetReader())
            .Returns(reader);
        Mock.Get(_csvReaderFactory)
            .Setup(f => f.CreateCsvReader(reader, It.IsAny<CsvConfiguration>()))
            .Returns(csvReader);

        _ = await _subject.ParsePolicies(policySource).ToListAsync();

        Mock.Get(_csvReaderFactory).VerifyAll();
    }

    [TestMethod]
    public async Task ParsePolicies_MapsPolicyDataFromSourcePolicyFields()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        List<SourceNumberedPolicy> sourcePolicies =
        [
            new SourceNumberedPolicy
            {
                PolicyNumber = "P01",
                BusinessDescription = "Co1",
                InsuredAmount = 999,
                StartDate = new DateOnly(2025, 1, 30),
                EndDate = new DateOnly(2025, 2, 28),
            },
            new SourceNumberedPolicy
            {
                PolicyNumber = "P02",
                BusinessDescription = "Co2",
                InsuredAmount = 1000,
                StartDate = null,
                EndDate = null,
            }
        ];
        CsvReader csvReader = new StubbedCsvReader<SourceNumberedPolicy>(reader)
        {
            Records = sourcePolicies.ToAsyncEnumerable()
        };
        Mock.Get(policySource)
            .Setup(s => s.GetReader())
            .Returns(reader);
        Mock.Get(policySource)
            .Setup(s => s.SourceId)
            .Returns(6);
        Mock.Get(_csvReaderFactory)
            .Setup(f => f.CreateCsvReader(reader, It.IsAny<CsvConfiguration>()))
            .Returns(csvReader);

        List<Policy> result = await _subject.ParsePolicies(policySource).ToListAsync();

        result.Should().BeEquivalentTo([
            new Policy
            {
                SourceId = 6,
                PolicyId = "P01",
                Customer = "Co1",
                InsuredAmount = 999,
                Currency = "GBP",
                StartDate = new DateOnly(2025, 1, 30),
                EndDate = new DateOnly(2025, 2, 28),
            },
            new Policy
            {
                SourceId = 6,
                PolicyId = "P02",
                Customer = "Co2",
                InsuredAmount = 1000,
                Currency = "GBP",
                StartDate = null,
                EndDate = null,
            }
        ]);
    }

    [TestMethod]
    public async Task ParsePolicies_WhenCsvReaderThrowsHeaderValidationException_ThrowsPolicyParseException()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        StubbedCsvReader<SourceNumberedPolicy> csvReader = new(reader);
        csvReader.Records = GetAsyncEnumeratorThatThrows<SourceNumberedPolicy>(
            new HeaderValidationException(csvReader.Context, invalidHeaders: []));
        Mock.Get(policySource)
            .Setup(s => s.GetReader())
            .Returns(reader);
        Mock.Get(_csvReaderFactory)
            .Setup(f => f.CreateCsvReader(reader, It.IsAny<CsvConfiguration>()))
            .Returns(csvReader);

        Func<Task> act = async () => await _subject.ParsePolicies(policySource).ToListAsync();

        await act.Should().ThrowAsync<PolicyParseException>()
            .WithMessage("Failed to parse CSV source with NumberedPolicyParser strategy.")
            .WithInnerException<PolicyParseException, HeaderValidationException>();
    }

    [TestMethod]
    public async Task ParsePolicies_WhenCsvReaderThrowsReaderException_ThrowsPolicyParseException()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        StubbedCsvReader<SourceNumberedPolicy> csvReader = new(reader);
        csvReader.Records = GetAsyncEnumeratorThatThrows<SourceNumberedPolicy>(new ReaderException(csvReader.Context));
        Mock.Get(policySource)
            .Setup(s => s.GetReader())
            .Returns(reader);
        Mock.Get(_csvReaderFactory)
            .Setup(f => f.CreateCsvReader(reader, It.IsAny<CsvConfiguration>()))
            .Returns(csvReader);

        Func<Task> act = async () => await _subject.ParsePolicies(policySource).ToListAsync();

        await act.Should().ThrowAsync<PolicyParseException>()
            .WithMessage("Failed to parse policy record value.")
            .WithInnerException<PolicyParseException, ReaderException>();
    }
}
