using AutoFixture.Xunit2;

namespace SaanSoft.Tests.Cqrs.Common.AutoFixture;

[AttributeUsage(AttributeTargets.Method)]
public class AutoFakeDataAttribute() : AutoDataAttribute(AutoFixtureExtensions.Create);
