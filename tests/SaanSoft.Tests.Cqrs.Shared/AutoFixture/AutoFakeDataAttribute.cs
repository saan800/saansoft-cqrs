using AutoFixture.Xunit2;

namespace SaanSoft.Tests.Cqrs.Shared.AutoFixture;

[AttributeUsage(AttributeTargets.Method)]
public class AutoFakeDataAttribute() : AutoDataAttribute(AutoFixtureExtensions.Create);
