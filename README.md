<a href="https://github.com/cloudtoid"><img src="https://raw.githubusercontent.com/cloudtoid/assets/master/logos/cloudtoid-black-red.png" width="100"></a>

# URL Pattern Matcher

![](https://github.com/cloudtoid/url-pattern/workflows/publish/badge.svg) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/cloudtoid/url-patterns/blob/master/LICENSE)

We all know regular expressions are not the most user-friendly programming constructs. With this URL Pattern Matcher library, you can say goodbye to regex, and define URL path patterns that are simple to compose, easy to read, and a joy to debug.

Cloudtoid's URL Pattern Matcher library is optimized for speed. It compiles the new patterns and caches the compiled version for future use. The pattern matcher component is tuned to pattern match without the need for backtracking.

This library supports `.netstandard2.1+` and is optimized for [.net dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) but can also be used without DI.

```csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: "/category/:cat/product/:prod",
    path: "/category/furniture/product/black-couch");

// match.Variables["cat"] == "furniture";
// match.Variables["prod"] == "black-couch";
```

This library has almost 100% test code coverage with plenty of tests.

## NuGet Package

The NuGet package for this library is published [here](https://www.nuget.org/packages/Cloudtoid.UrlPattern/).

## URL Path

This library only matches the path section of the URL. That is the section after the host and does not include the [query string](https://en.wikipedia.org/wiki/Query_string) or the [URL fragment](https://en.wikipedia.org/wiki/Fragment_identifier) portions. For instance, in the URL below, it is the `/path/over/there` section of the URL and must always start with a `'/'`.

`https://host:80/path/over/there?query#fragment`

## Pattern Modes

The syntax for the URL pattern is pretty simple and comes in 3 flavors:

- **Prefix match**: This is the default behavior. However, you can be explicit about this and start the pattern with `"prefix: "`.
- **Exact match**: In this mode the entire path should match the pattern. For exact matches, start your pattern with `"exact: "`.
- **Regex match**: For regex patterns, start the pattern with `"regex: "`. More on this below.

## Variables

A variable starts with `':'` and follows a name. The valid characters in a name are `[a..zA..Z0..9_]` and the first character cannot be a number.

In the example above (`"/category/:cat/product/:prod"`), `cat` and `prod` are both variables.

## Optional Sections

Optional elements are included in paranthesis: `'('` and `')'`. For instance `/product` is an optional segment in `"/category/:cat(/product)/:prod"`. The followig paths are a match for this pattern:

- `/category/bike/product/bmx-123`
- `/category/bike/bmx-123`

variables:

- cat = "bike"
- prod = "bmx-123"

## Wildcards

`'*'` in patterns are wildcards and match all characters except for `'/'`.

```csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: "/category/*/product/:prod",
    path: "/category/furniture/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Escape Character

You can use `"\\"` to escape `':'`, `'*'`, `'('`, `')'`.

```csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: @"/category/\\:furniture/product/:prod",
    path: "/category/:furniture/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Regular Expressions

The pattern engine also supports [.net's regular expression syntax](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference). Please note that only named captures such as `"(?<prod>.+)"` are allowed. These captures are treated as variables.

```csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: @"regex: \A/product(/(?<prod>.+))?",
    path: "/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Use with Dependency Injection

On your instance of [`IServiceCollection`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection), call the `AddUrlPattern` extension method:

```csharp
services.AddUrlPattern();
```

You can then inject an instance of `IPatternEngine` and use that as the entry point to the URL pattern matcher library. Please note that the `IPatternEngine` is registered as a singleton with the DI container.

## Use without Dependency Injection

Simply create an instance of `PatternEngine` and use `Match` or `TryMatch` methods to perform a pattern match.

```csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: "/api/message",
    path: "/api/message/echo");
```

The pattern engine compiles the pattern and caches it so that it can be reused in the future. The cache is in the context of the current instance of `PatternEngine`. Therefore, to improve the performance of your application, create and use a singleton instance of `PatternEngine`.

## Advanced Usage

This library has 3 public interfaces that can be used directly:

- `IPatternEngine`: This is the main entry point. If you don't need granular control over the compilation and matching, then use this interface.
- `IPatternCompiler`: It parses and compiles the URL pattern and returns the compiled pattern.
- `IPatternMatcher`: It matches a path to a compiled pattern generated by the `IPatternCompiler`.

Advanced users can also provide their own implementations of the above interfaces and register those with the dependency injection container.

## How to Contribute

- Create a branch from `master`.
- Ensure that all tests pass.
- Keep the code coverage number above 99.5% by adding new tests or modifying the existing tests.
- Send a pull request.

## Author

[**Pedram Rezaei**](https://www.linkedin.com/in/pedramrezaei/): Pedram is a software architect at Microsoft with years of experience building highly scalable and reliable cloud-native applications for Microsoft.

## Credits

This project was inspired by a similar npm package: [url-pattern](https://github.com/snd/url-pattern).
