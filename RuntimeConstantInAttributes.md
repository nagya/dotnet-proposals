# "Runtime constants" in attribute parameters

See [Switch and pattern matching on "runtime constants"](RuntimeConstantInPatterns.md) for the definition of "runtime constant" and an example on how they are a useful extension of compile time constants.

This proposal further narrows the gap between compile time constants and runtime constants, by allowing runtime constants to be used as parameters to attributes.

The value types `RuntimeConstant` and `RuntimeConstant<T>` would be added to the framework, each wrapping a `MemberInfo` (`PropertyInfo` or `FieldInfo` only.)  Both of these types would be allowed as types of attribute parameters. The corresponding attribute argument can be given with a reference to a const field, static readonly field, or static init-only property. The type of the field/property can be any type for `RuntimeConstant`, and must be of type `T` (or subclass) for `RuntimeConstant<T>`.

Via reflection, these attribute arguments are exposed as `MemberInfo` (`FieldInfo` or `PropertyInfo`), and for analyzers as `IPropertySymbol` or `IFieldSymbol`.

Example:

```csharp
public sealed class LocationAttribute : Attribute
{
    public LocationAttribute(RuntimeConstant<City> city) {}
}

static class Cities
{
    public static readonly City BellevueWA = new City("Bellevue", "WA", 47.614444, -122.1925);
}

//...

[Location(Cities.BellevueWA)]
class Foo {}
```


Framework types to be added:

```csharp
public readonly struct RuntimeConstant
{
    public MemberInfo info { get; }
    
    public RuntimeConstant(FieldInfo info) => this.info = info;
    public RuntimeConstant(PropertyInfo info) => this.info = info;
    
    public object? GetValue() => info is PropertyInfo pi ? return pi.GetValue(null) : ((FieldInfo)info).GetValue(null);
}

public readonly struct RuntimeConstant<T>
{
    public RuntimeConstant constant { get; }
    
    public RuntimeConstant(FieldInfo info)
    {
        if (!typeof(T).IsAssignableFrom(info.FieldType)) throw new InvalidCastException();
        constant = new RuntimeConstant(info);
    }
    public RuntimeConstant(PropertyInfo info)
    {
        if (!typeof(T).IsAssignableFrom(info.PropertyType)) throw new InvalidCastException();
        constant = new RuntimeConstant(info);
    }
    
    public T GetValue() => (T)constant.GetValue();
}
```
