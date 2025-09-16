using AutoFixture.Xunit2;

namespace SaanSoft.Tests.Cqrs.Shared.AutoFixture;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoFakeDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoFakeDataAttribute(params object[] arguments)
#pragma warning disable CS8974 // Converting method group to non-delegate type
        : base(AutoFixtureExtensions.Create, arguments) { }
#pragma warning restore CS8974 // Converting method group to non-delegate type
}
