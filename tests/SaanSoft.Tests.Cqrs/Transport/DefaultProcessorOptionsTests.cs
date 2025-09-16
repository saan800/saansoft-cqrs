using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Tests.Cqrs.Transport;

public class DefaultProcessorOptionsTests
{
    [Theory]
    [AutoFakeData]
    public void Clone(bool waitForExecution, bool expectSingleHandler, DefaultProcessorOptions defaultOptions)
    {
        var externalProcessorOptions = defaultOptions.Clone(waitForExecution, expectSingleHandler);

        externalProcessorOptions.WaitForExecution.Should().Be(waitForExecution);
        externalProcessorOptions.ExpectSingleHandler.Should().Be(expectSingleHandler);
        externalProcessorOptions.Timeout.Should().Be(defaultOptions.Timeout);
    }
}
