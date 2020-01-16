# API proposal: Nullable.RefValue, RefValueOrDefault dotnet/runtime#1534

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

To avoid a defensive copy, the implementation uses the backing field of 
`HasValue`, instead of the property, so the field would have to be made internal.
An alternative would be to mark the property `readonly`.

```csharp
namespace System
{
    public static class Nullable
    {
        public static ref readonly T RefValue<T>(this in T? n) where T : struct
        {
            if (n.hasValue)
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
            return ref n.hasValue ? ref n.value : ref defaultValue;
        }
    }
}
```
