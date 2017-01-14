using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Additio.Configuration.Tests
{
    internal class AutoMockedDataAttribute : CompositeDataAttribute
    {
        public AutoMockedDataAttribute()
            : this(new AutoNsubstitueDataAttribute())
        { }

        public AutoMockedDataAttribute(params object[] values)
            : this(new AutoNsubstitueDataAttribute(), values)
        { }

        private AutoMockedDataAttribute(AutoNsubstitueDataAttribute autoDataAttributeAttribute, params object[] values)
            : base(new InlineDataAttribute(values), autoDataAttributeAttribute)
        { }

        private class AutoNsubstitueDataAttribute : AutoDataAttribute
        {
            public AutoNsubstitueDataAttribute()
                : base(new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
            { }
        }
    }
}
