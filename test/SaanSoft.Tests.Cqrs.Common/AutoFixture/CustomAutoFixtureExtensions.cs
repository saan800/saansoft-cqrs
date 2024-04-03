using AutoFixture;

namespace SaanSoft.Tests.Cqrs.Common.AutoFixture;

public static class CustomAutoFixtureExtensions
{
    public static Fixture Create()
    {
        // Uses the common VetDB.TestFramework configuration
        var fixture = AutoFixtureExtensions.CreateFixture();


        return fixture;
    }
}

