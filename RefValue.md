## Proposed API
```csharp
namespace System
{
    public static class Nullable
    {
        public static ref readonly T RefValue<T>(this in T? n) where T : struct;
        public static ref readonly T RefValueOrDefault<T>(this in T? n) where T : struct;
        public static ref readonly T RefValueOrDefault<T>(this in T? n, in T defaultValue) where T : struct;
    }
}
```

These parallel the Value property and the GetValueOrDefault methods of `Nullable<T>`,
but return the value by reference. They're extension methods because of CS8170:
Struct members cannot return 'this' or other instance members by reference.

They're useful for accessing the value wrapped by a `Nullable<T>` without
incurring a copy.


## Reference implementation
```csharp
namespace System
{
    public static class Nullable
    {
        public static ref readonly T RefValue<T>(this in T? n) where T : struct
        {
            if (n.HasValue)
                return ref n.value;
            else
                throw new InvalidOperationException();
        }

        public static ref readonly T RefValueOrDefault<T>(this in T? n) where T : struct
        {
            return ref n.value;
        }
       
        public static ref readonly T RefValueOrDefault<T>(this in T? n, in T defaultValue) where T : struct
        {
            return ref n.HasValue ? ref n.value : ref defaultValue;
        }
    }
}
```
