using AutoFixture;
using AutoFixture.AutoFakeItEasy;

namespace SaanSoft.Tests.Cqrs.Shared.AutoFixture;

public static class AutoFixtureExtensions
{
    public static Fixture Create()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoFakeItEasyCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        }
        );
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        fixture.Register(A.Fake<HttpMessageHandler>);
        fixture.Freeze<HttpMessageHandler>();

        fixture.Register<HttpClient>(() =>
        {
            var httpMessageHandler = fixture.Freeze<HttpMessageHandler>();
            return new HttpClient(httpMessageHandler)
            {
                // inject a default baseUri for httpClient, this is required for dependencies that expect
                // HttpClientFactory to configure their defaults correctly, in that case even if you mock out the
                // message handler things will fail when generating the target uri
                BaseAddress = new Uri("http://localhost")
            };
        });

        return fixture;
    }
}
