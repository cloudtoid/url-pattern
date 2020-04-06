# URL Path Pattern Matcher

We all know regular expressions are not the most user-friendly programming constructs. With this library, you can say goodbye to regex and define URL path patterns that are simple to compose, easy to read, and a joy to debug.

Cloudtoid's URL Pattern Matcher library is optimized for speed. It compiles the new patterns and caches the compiled version for future use. The pattern matcher component is tuned to pattern match without the need for backtracking.

This library supports `.netstandard2.1+` and is optimized for [.net dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1) but can also be used without DI.

``` csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: "/category/:cat/product/:prod",
    path: "/category/furniture/product/black-couch");

// match.Variables["cat"] == "furniture";
// match.Variables["prod"] == "black-couch";
```

## Pattern Modes

The syntax for the URL pattern is pretty simple and comes in 3 flavors:

- **Prefix match**: This is the default behavior. However, you can be explicit about this and start the pattern with `"prefix: "`.
- **Exact match**: In the exact match mode, the entire path should match the pattern. For exact matches, start your pattern with `"exact: "`.
- **Regex match**: For regex patterns, simply start the pattern with `"regex: "`. More on this later.

## Variables

A variable starts with `':'` and follows a name. The valid characters in a name are valid characters are `[a..zA..Z0..9_]`. However, the very first character cannot be a number.

In the example above (`"/category/:cat/product/:prod"`), `cat` and `prod` are both variables.

## Optional Sections

Optional elements are simply included in paranthesis: `'('` and `')'`. For example, `"/category/:cat(/product)/:prod"` pattern has an optiona section: `/product`. The followig paths are a match for this pattern:

- `/category/bike/product/bmx-123`
- `/category/bike/bmx-123`

variables:

- cat = "bike"
- prod = "bmx-123"

## Wildcards

`*` in patterns are wildcards and match all characters except for `'/'`.

``` csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: "/category/*/product/:prod",
    path: "/category/furniture/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Escape Character

You can use `"\\"` to escape `':'`, `'*'`, `'('`, `')'`.

``` csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: @"/category/\\:furniture/product/:prod",
    path: "/category/:furniture/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Regular Expressions

The pattern engine also supports .net's regular expression syntax. Please note that only named captures such as `"(?<prod>.+)"` are allowed and are treated as variables. 

``` csharp
var engine = new PatternEngine();
var match = engine.Match(
    pattern: @"regex: \A/product(/(?<prod>.+))?",
    path: "/product/black-couch");

// match.Variables["prod"] == "black-couch";
```

## Use with Dependency Injection
TODO: 

## Use without Dependency Injection
TODO: MUST cache the PatternEngine.

## How to Contribute
TODO 

## NuGet Package
TODO

## Advanced Usage
TODO: Use of Compiler and Matcher directly
TODO: Use of DI to change impl or compiler, matcher, etc...
