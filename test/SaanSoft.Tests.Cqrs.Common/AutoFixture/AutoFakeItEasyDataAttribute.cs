using AutoFixture.Xunit2;

namespace SaanSoft.Tests.Cqrs.Common.AutoFixture;

[AttributeUsage(AttributeTargets.Method)]
public class AutoFakeItEasyDataAttribute() : AutoDataAttribute(CustomAutoFixtureExtensions.Create);
