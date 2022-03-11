# Switch and pattern matching on "runtime constants"

A "runtime constant" is a static readonly field or static get-only property of a deeply immutable value or reference type, that is conceptually as constant as a named scalar or string declared with the `const` keyword.

The proposal is to allow such runtime constants to be used in case statements, pattern matching against, etc, for each operator the underlying type implements (equality, comparison).

Motivation: Simplifies many common patterns, enabling developers to declare constants of structured data types and have the same level of language support for them as for scalar and string constants.

Example: The main point in this example is that the named runtime constant cannot be represented with a single scalar or string, as its identity has two components, city name and state ID, and it also contains additional metadata, the geo-location. Without the proposed feature, developers currently have to resort to workarounds such as defining a parallel enum or string constants, in addition to the runtime constant, to be able to use the identity in a switch statement or pattern. 

```csharp
 static readonly City BellevueWA = new City("Bellevue", "WA", 47.614444, -122.1925);
  
 switch (address.city)
 {
     case BellevueWA: // if (address.city == BellevueWA)
         ...
 }
```
