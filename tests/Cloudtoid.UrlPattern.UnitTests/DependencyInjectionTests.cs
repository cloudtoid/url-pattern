using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void RegisterOnlyOnceTest()
        {
            var services = new ServiceCollection();

            services
                .AddUrlPattern()
                .AddUrlPattern()
                .AddUrlPattern();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetServices<IPatternTypeResolver>().Should().HaveCount(1);
            serviceProvider.GetServices<IPatternParser>().Should().HaveCount(1);
            serviceProvider.GetServices<IPatternValidator>().Should().HaveCount(1);
            serviceProvider.GetServices<IPatternCompiler>().Should().HaveCount(1);
            serviceProvider.GetServices<IPatternMatcher>().Should().HaveCount(1);
        }
    }
}
