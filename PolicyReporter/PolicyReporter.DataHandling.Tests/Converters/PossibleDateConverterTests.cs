using System.Runtime.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Moq;
using PolicyReporter.DataHandling.Converters;

namespace PolicyReporter.DataHandling.Tests;

[TestClass]
public sealed class PossibleDateConverterTests
{
    private PossibleDateConverter _subject = null!;

    [TestInitialize]
    public void SetUp()
    {
        _subject = new PossibleDateConverter();
    }

    [TestMethod]
    public void ConvertFromString_WhenTextIsValidDate_ReturnsDate()
    {
        object? result = _subject.ConvertFromString("01-02-2025", Mock.Of<IReaderRow>(), GetMemberMapData());

        result.Should().BeOfType<DateOnly>().Which.ToString().Should().Be("01/02/2025");
    }

    [TestMethod]
    public void ConvertFromString_WhenTextIsInvalidDate_ReturnsNull()
    {
        object? result = _subject.ConvertFromString("bad date", Mock.Of<IReaderRow>(), GetMemberMapData());

        result.Should().BeNull();
    }

#pragma warning disable SYSLIB0050 // Type or member is obsolete
    private static MemberMapData GetMemberMapData() =>
        (MemberMapData) FormatterServices.GetUninitializedObject(typeof(MemberMapData));
#pragma warning restore SYSLIB0050 // Type or member is obsolete
}
