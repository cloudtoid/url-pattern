# Cloudtoid.UrlPattern

Match URL paths and extract named values with readable patterns. Supports prefix and exact matching, optional sections, wildcards, and regular expressions. Compiled patterns are cached for reuse.

## Install

Requires .NET 10 or later.

```sh
dotnet add package Cloudtoid.UrlPattern
```

## Example

```csharp
using Cloudtoid.UrlPattern;

var engine = new PatternEngine();
var match = engine.Match(
    pattern: "exact: /category/:category/product/:product",
    path: "/category/furniture/product/black-couch");

Console.WriteLine(match.Variables["category"]); // furniture
Console.WriteLine(match.Variables["product"]);  // black-couch
```

Pass the URL path only, starting with `/`; exclude the scheme, host, query string, and fragment. `Match` throws when matching fails; use `TryMatch` to handle a non-match without an exception.

## Pattern syntax

- `:name` captures a named value.
- `*` matches characters within a path segment, excluding `/`.
- Parentheses mark optional sections, such as `/products(/:id)`.
- `exact: ` requires the entire path to match.
- `prefix: ` matches a prefix and is the default mode.
- `regex: ` enables regular expressions with named captures.

For dependency injection, call `services.AddUrlPattern()` and inject `IPatternEngine`.

[Full syntax and examples](https://github.com/cloudtoid/url-pattern) · [Report an issue](https://github.com/cloudtoid/url-pattern/issues) · [MIT license](https://github.com/cloudtoid/url-pattern/blob/master/LICENSE)
