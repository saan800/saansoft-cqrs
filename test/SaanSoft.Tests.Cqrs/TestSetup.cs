namespace SaanSoft.Tests.Cqrs;

public abstract class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();
        A.CallTo(() => Logger.IsEnabled(A<LogLevel>.Ignored)).Returns(true);

        ServiceCollection = new ServiceCollection();
    }

    private IServiceProvider? _serviceProvider = null;
    protected IServiceProvider GetServiceProvider()
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();
}
