using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Tests.Cqrs.Transport;

public class DefaultExternalMessageRouterOptionsTests
{
    [Theory]
    [AutoFakeData]
    public void Clone(bool waitForExecution, bool expectSingleHandler, DefaultExternalMessageRouterOptions defaultOptions)
    {
        var externalProcessorOptions = defaultOptions.Clone(waitForExecution, expectSingleHandler);

        externalProcessorOptions.WaitForExecution.Should().Be(waitForExecution);
        externalProcessorOptions.ExpectSingleHandler.Should().Be(expectSingleHandler);
        externalProcessorOptions.Timeout.Should().Be(defaultOptions.Timeout);
    }
}
