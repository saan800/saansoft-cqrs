using AutoFixture.Xunit2;

namespace SaanSoft.Tests.Cqrs.Common.AutoFixture;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoFakeItEasyDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoFakeItEasyDataAttribute(params object[] arguments)
#pragma warning disable CS8974 // Converting method group to non-delegate type
        : base(CustomAutoFixtureExtensions.Create, arguments) { }
#pragma warning restore CS8974 // Converting method group to non-delegate type
}
