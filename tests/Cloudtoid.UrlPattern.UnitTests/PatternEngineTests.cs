﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public class PatternEngineTests
    {
        private readonly IPatternEngine engine;

        public PatternEngineTests()
        {
            var services = new ServiceCollection().AddUrlPattern();
            var serviceProvider = services.BuildServiceProvider();
            engine = serviceProvider.GetRequiredService<IPatternEngine>();
        }

        [TestMethod]
        public void PatternEngineUseAsLibrary()
        {
            var engine = new PatternEngine();
            engine.TryMatch("/category", "/category/cat1/product/prod1", out var match, out var why).Should().BeTrue();
            match.Should().NotBeNull();
            match!.PathSuffix.Should().Be("/cat1/product/prod1");
            why.Should().BeNull();
        }

        [TestMethod]
        public void PatternEngineTryCompile()
        {
            engine.TryCompile("/category", out var compiledPattern, out var errors).Should().BeTrue();
            compiledPattern.Should().NotBeNull();
            compiledPattern!.Regex.ToString().Should().Be(@"\A/category");
            errors.Should().BeNull();
        }

        [TestMethod]
        public void TryMatchVariableNotPresentTest()
        {
            engine.TryMatch("/category/:catid(/product/:pid)", "/category/home", out var match, out var why).Should().BeTrue();
            match.Should().NotBeNull();
            match!.PathSuffix.Should().Be(string.Empty);
            match.Variables.Should().BeEquivalentTo(new Dictionary<string, string>() { ["catid"] = "home" });
            why.Should().BeNull();
        }

        [TestMethod]
        public void TryMatchTests()
        {
            ShouldNotMatch("/segment", "/product");
            ShouldMatch("/product", "/product");
            ShouldNotMatch("/product/", "/product");
            ShouldMatch("/product/", "/product/");
            ShouldNotMatch("/product/*/", "/product/1234");
            ShouldMatch("/product/*/", "/product/1234/");
            ShouldMatch("/product/1*/", "/product/1234/");
            ShouldNotMatch("/product/13*/", "/product/1234");
            ShouldMatch("/product/(1*/)", "/product/");
            ShouldMatch("/product/(1*/)", "/product/1234/");
            ShouldMatch("/product/(1*/)", "/product/1234", "1234");
            ShouldMatch("/product/(1*(/))", "/product/1234");
            ShouldMatch("(/product)/(1*(/))", "/product/1234");
            ShouldMatch("(/product)/(1*(/))", "/1234");
            ShouldMatch("(/product)/(1*(/))", "/1234/");
            ShouldMatch("(/product)/(1*/)", "/1234/");
            ShouldMatch("/product/:id", "/product/1234", variables: ("id", "1234"));
            ShouldMatch("/product(/:id)", "/product/1234", variables: ("id", "1234"));
            ShouldMatch("/product(/:id)", "/product");
            ShouldMatch("/", "/product", pathSuffix: "product");
            ShouldMatch("(/product)", "/", pathSuffix: "/");
            ShouldMatch("/(product)", "/");

            ShouldMatch(
                pattern: "/category/:category/product/:product",
                path: "/category/bike/product/1234",
                string.Empty,
                ("category", "bike"),
                ("product", "1234"));

            ShouldMatch(
                pattern: "/category/:category(/product/:product)",
                path: "/category/bike/product/1234",
                string.Empty,
                ("category", "bike"),
                ("product", "1234"));

            ShouldMatch(
                pattern: "/category/:category(/product/:product)",
                path: "/category/bike",
                variables: ("category", "bike"));

            ShouldMatch(
               pattern: "/category/*(/product/:product)",
               path: "/category/bike/product/1234",
               variables: ("product", "1234"));

            ShouldMatch(
               pattern: "/category/*(/product/:product)",
               path: "/category/bike");

            ShouldNotMatch(
               pattern: "/category/*(/product/:product)",
               path: "/bike(/product/:product)");

            ShouldMatch(
               pattern: "/category/*(/product/:product)",
               path: "/category/bike");

            ShouldNotMatch(
              pattern: "/category/*(/product/:product)",
              path: "/bike(/product/:product)");

            ShouldMatch(
               pattern: @"/category/\\*(/product/:product)",
               path: "/category/*/product/1234",
               variables: ("product", "1234"));

            ShouldMatch(
               pattern: @"/category/\\*(/product/:product)",
               path: "/category/*");

            ShouldMatch(
               pattern: @"/category/\\*/product/:product",
               path: "/category/*/product/1234",
               variables: ("product", "1234"));

            ShouldMatch(
               pattern: @"/category/\\:product/",
               path: "/category/:product/");

            ShouldMatch(
               pattern: @"/category/\\(:product\\)",
               path: "/category/(1234)",
               variables: ("product", "1234"));

            ShouldNotMatch(
               pattern: @"/category/\\(:product\\)",
               path: "/category/1234");

            ShouldMatch(
               pattern: @"/category/\(:product\)",
               path: @"/category/\1234\",
               variables: ("product", "1234"));

            ShouldMatch(
               pattern: @"/category/*",
               path: "/category/1234");

            ShouldMatch(
               pattern: @"/category/*",
               path: "/category/1234/",
               pathSuffix: "/");

            ShouldMatch(
               pattern: @"/category/*",
               path: "/category/");

            ShouldNotMatch(
               pattern: @"exact: /category/",
               path: "/category/test");

            ShouldMatch(
               pattern: @"/category/",
               path: "/category/bike/",
               pathSuffix: "bike/");

            ShouldMatch(
               pattern: @"/category/",
               path: "/category/bike/product/1234",
               pathSuffix: "bike/product/1234");

            ShouldMatch(
               pattern: @"/catego",
               path: "/category/bike/product/1234",
               pathSuffix: "ry/bike/product/1234");

            ShouldMatch(
               pattern: @"/category/*/product",
               path: "/category/bike/product");

            ShouldMatch(
               pattern: @"regex: \/category\/(?<category>.+)\/product",
               path: "/category/bike/product",
               variables: ("category", "bike"));

            ShouldMatch(
               pattern: @"regex: \/category\/(?<category>.+)\/product/?",
               path: "/category/bike/product",
               variables: ("category", "bike"));

            ShouldMatch(
               pattern: @"regex: \/category\/(?<category>.+)\/product",
               path: "/category/bike/product/123/test",
               pathSuffix: "/123/test",
               variables: ("category", "bike"));
        }

        [TestMethod]
        public void PatternEngineCacheTests()
        {
            var pattern = @"/: variable";
            var path = "/category/test";

            engine.TryMatch(pattern, path, out var _, out var why).Should().BeFalse();
            why.Should().Contain("// not from cache");

            engine.TryMatch(pattern, path, out var _, out why).Should().BeFalse();
            why.Should().NotContain("// not from cache");
        }

        [TestMethod]
        public void PatternEngineMatchShouldThrow()
        {
            var pattern = @"/: variable";
            var path = "/category/test";
            Action act = () => engine.Match(pattern, path);

            act.Should().ThrowExactly<PatternException>("*There is a variable with an empty or invalid name*");
        }

        [TestMethod]
        public void PatternEngineMatchSimple()
        {
            var pattern = @"/:variable";
            var path = "/value";
            Action act = () => engine.Match(pattern, path);
            act.Should().NotThrow();
        }

        private void ShouldMatch(
            string pattern,
            PathString path,
            string? pathSuffix = null,
            params (string Name, string Value)[] variables)
        {
            engine.TryMatch(pattern, path, out var match, out var why).Should().BeTrue();
            why.Should().BeNull();
            match.Should().NotBeNull();
            match!.PathSuffix.Should().BeEquivalentTo(pathSuffix ?? string.Empty);
            match.Variables.Select(v => (v.Key, v.Value)).Should().BeEquivalentTo(variables);
        }

        private void ShouldNotMatch(string pattern, string path)
        {
            engine.TryMatch(pattern, path, out var match, out var why).Should().BeFalse();
            why.Should().NotBeNull();
            match.Should().BeNull();
        }
    }
}
