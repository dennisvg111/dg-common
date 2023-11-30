using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace DG.Common.Tests.TestHelpers
{
    public static class FluentLinguisticComparison
    {
        public static AndConstraint<StringAssertions> BeLexographicallyLessThan(this StringAssertions parent, string expected)
        {
            Execute.Assertion
                .Given(() => parent.Subject)
                .ForCondition((v) => v.CompareTo(expected) < 0)
                .FailWith($"Value {parent.Subject} should be lexographically less than {expected}.");
            return new AndConstraint<StringAssertions>(parent);
        }

        public static AndConstraint<StringAssertions> BeLexographicallyLessThanOrEqualTo(this StringAssertions parent, string expected)
        {
            Execute.Assertion
                .Given(() => parent.Subject)
                .ForCondition((v) => v.CompareTo(expected) <= 0)
                .FailWith($"Value {parent.Subject} should be lexographically less than or equal to {expected}.");
            return new AndConstraint<StringAssertions>(parent);
        }
    }
}
