using System.Diagnostics.CodeAnalysis;

namespace Authentication.OAuth2.Test;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}