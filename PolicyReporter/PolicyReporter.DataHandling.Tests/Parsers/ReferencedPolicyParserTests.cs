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
public sealed class ReferencedPolicyParserTests
{
    private ReferencedPolicyParser _subject = null!;
    private ICsvReaderFactory _csvReaderFactory = null!;

    [TestInitialize]
    public void SetUp()
    {
        _csvReaderFactory = Mock.Of<ICsvReaderFactory>();
        _subject = new ReferencedPolicyParser(_csvReaderFactory);
    }

    [TestMethod]
    public async Task ParsePolicies_StreamsRecordsFromCsvReader()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        List<SourceReferencedPolicy> sourcePolicies =
        [
            new SourceReferencedPolicy { PolicyRef = "P01", CompanyDescription = "Co1" }
        ];
        CsvReader csvReader = new StubbedCsvReader<SourceReferencedPolicy>(reader)
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
        List<SourceReferencedPolicy> sourcePolicies =
        [
            new SourceReferencedPolicy
            {
                PolicyRef = "P01",
                CompanyDescription = "Co1",
                CoverageAmount = 999,
                InitiationDate = new DateOnly(2025, 1, 30),
                ExpirationDate = new DateOnly(2025, 2, 28),
            },
            new SourceReferencedPolicy
            {
                PolicyRef = "P02",
                CompanyDescription = "Co2",
                CoverageAmount = 1000,
                InitiationDate = null,
                ExpirationDate = null,
            }
        ];
        CsvReader csvReader = new StubbedCsvReader<SourceReferencedPolicy>(reader)
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
        StubbedCsvReader<SourceReferencedPolicy> csvReader = new(reader);
        csvReader.Records = GetAsyncEnumeratorThatThrows<SourceReferencedPolicy>(
            new HeaderValidationException(csvReader.Context, invalidHeaders: []));
        Mock.Get(policySource)
            .Setup(s => s.GetReader())
            .Returns(reader);
        Mock.Get(_csvReaderFactory)
            .Setup(f => f.CreateCsvReader(reader, It.IsAny<CsvConfiguration>()))
            .Returns(csvReader);

        Func<Task> act = async () => await _subject.ParsePolicies(policySource).ToListAsync();

        await act.Should().ThrowAsync<PolicyParseException>()
            .WithMessage("Failed to parse CSV source with ReferencedPolicyParser strategy.")
            .WithInnerException<PolicyParseException, HeaderValidationException>();
    }

    [TestMethod]
    public async Task ParsePolicies_WhenCsvReaderThrowsReaderException_ThrowsPolicyParseException()
    {
        StreamReader reader = new(new MemoryStream());
        IPolicySource policySource = Mock.Of<IPolicySource>();
        StubbedCsvReader<SourceReferencedPolicy> csvReader = new(reader);
        csvReader.Records = GetAsyncEnumeratorThatThrows<SourceReferencedPolicy>(new ReaderException(csvReader.Context));
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
