# Self-nullable value types and enums

Support representing nullable structs and enums with special values instead of a `Nullable<T>` wrapper.

Motivation: Many value types (especially those that wrap a reference type) already have plenty of room in them to represent a null value (e.g. with the null value of the wrapped reference type), so it's preferable to represent their null value that way as opposed to wrapping with `Nullable<T>`. Wrapping adds an additional boolean field, that, due to padding, can easily double the size of the value type. Also not having to say `.Value` in `if (x != null) x.Value.Foo()` is nice.

Details: Structs and enums that support this implement an interface (or use a keyword) and their parameterless constructor provides the null value, or if no such constructor, `default(T)` is the null value. The compiler enforces that the `HasValue` property is pure, i.e. has the `readonly` keyword. (Alternative: static interface method generates the null value.)

```csharp
 public struct Point : INullable
 public null struct Point // alternative
 {
     public readonly double x, y;
     
     public Point() => x = y = double.NaN;
     
     public readonly bool HasValue => !double.IsNaN(x);
 }
```
 
Usage is consistent with reference types, including warnings, nullability-related attributes, null-coalescing operators, comparison and pattern match against null, etc. Notably, a self-nullable value type can be compared to null even if it does not implement any equality operators.

```csharp
 Point? point = ...; // in IL, this is just a Point, NOT a Nullable<Point>
  
 if (point != null)
 {
     double x = point.x; // okay
 }
  
 double y = point.y; // not okay
```

Null analysis warns by default if accessing a method/property/etc on a value that may be null, but no runtime check for null is generated. The `null` and `notnull` keywords on a method/property may be used to change this behavior: `null` keyword means no warning; `notnull` means warning and runtime guard.

In other words, some methods may be marked as not being well-defined on a null value:

```csharp
 public null struct Point
 {
     ...
     public notnull Point Rotated(double degrees) { ... }
 }
```
Which is lowered to:
```csharp
 public Point Rotated(double degrees)
 {
     if (!HasValue) throw new NullReferenceException();
     ...
 }
 ```
 
Conversely, methods may be marked as explicitly okay to call on null instances:

```csharp
 public null struct Point
 {
     public null string ToString() { ... } // null analysis treats 'this' parameter as possibly null
 }
```

Boxing and unboxing treats self-nullable value types like a `Nullable<T>`, i.e. null boxes and unboxes to null and never throws `NullReferenceException`. Wrapping a self-nullable value type with a `Nullable<T>` is not allowed.


Enum example:

```csharp
public null enum CompareResult 
{
  null = int.MinValue,
  Smaller = -1,
  Equal = 0,
  Larger = 1
}
```

Generics: code like this should be valid and `T?` would mean `T` in IL if `T` is reference type or self-nullable value type, and `Nullable<T>` otherwise.

```
public T? GetOrNull<T>() where T : notnull
{
   // ok to return T or null
}
```

If this is too challenging to implement (e.g. because it's difficult to say `T` or `Nullable<T>` in IL), then a new kind of type constraint would likely be needed that requires reference type or self-nullable value type. Alternatively, self-nullable value types could just satisfy the `where T : class` constraint. The main goal with respect to generics is that helpers like this example should not need to have two separate overloads, one for reference types and one for self-nullable value types (and ideally also not a third one for `Nullable<T>`).
