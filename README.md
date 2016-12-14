## UrlQuery

This library converts API filter expressions in the form of `item eq 7 or value gt 17` into linq expressions to query a datasource

## Supported operators

The set of filter operators supported is defined in the [Microsoft REST Api Guidelines](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#97-filtering), specifically:

Operator             | Description           | Example
-------------------- | --------------------- | -----------------------------------------------------
*Comparison*         |                       |
eq                   | Equal                 | firstname eq 'Bruce'
ne                   | Not equal             | country ne 'Atlantis'
gt                   | Greater than          | weight gt 200
ge                   | Greater than or equal | height ge 75
lt                   | Less than             | quantity lt 20
le                   | Less than or equal    | price le 9.99
*Logical*            |                       |
and                  | Logical and           | price le 75 and cost gt 40
or                   | Logical or            | superpower eq 'speed' or superpower eq 'strength'
not                  | Logical negation      | not secretidentity eq 'Oliver Queen'
*Grouping*           |                       |
( )                  | Precedence grouping   | (priority eq 1 or city eq 'Metropolis') and price gt 100

## Usage

There's an interface, `IFilterExpression` for registering with your IOC container or you can new up `WhereExpression`. The one method `FromString<T>(string queryString)` that accepts a type that's used to resolve the property names and the `$filter` string.

```csharp
var filterString = "firstname eq 'Arthur' and not lastname eq 'Curry'";

var expressionGenerator = new WhereExpression();
var expression = expressionGenerator.FromString<SuperHero>(filterString);

using (var context = new SuperHeroContext()) 
{ 
    var heros = context.SuperHeros.Where(expression);
}
```

A static method is also available

```csharp
var expression = WhereExpression.Build.FromString<SuperHero>(filterString);
```

