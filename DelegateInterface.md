# Delegates implementing interfaces

Allow delegates to implement single-method interfaces.

Motivation: Simplifies code and prevents allocations of wrapper classes. It should also be simple to implement as delegates are already compiler-generated classes.

Utilities such as `Array.Sort` would not need to be implemented twice for both `IComparer<T>` and `Comparison<T>` or pay the cost of allocating a wrapper `IComparer` around a `Comparison` or vice-versa.

Example:

```csharp
public delegate int Comparison<in T>(T x, T y) : IComparer<T>.Compare;
```

 

