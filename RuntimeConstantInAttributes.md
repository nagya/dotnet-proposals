# "Runtime constants" in attribute parameters

See [Switch and pattern matching on "runtime constants"](RuntimeConstantInPatterns.md) for the definition of "runtime constant" and an example on how they are a useful extension of compile time constants.

This proposal further narrows the gap between compile time constants and runtime constants, by allowing runtime constants to be used as parameters to attributes.

The value type `RuntimeConstant<T>` would be added to the framework, wrapping a `MemberInfo` (`PropertyInfo` or `FieldInfo` only.) This type would be allowed to be used as the type of attribute parameters. The corresponding attribute argument can be given with a reference to a const field, static readonly field, or static get-only property. The type of the field/property must be of `T` or subclass. `RuntimeConstant<object>` can be used to allow a field/property of any type.

`RuntimeConstant<T>` instances can also be created from regular code with the constructor syntax, similarly how a delegate instance is created from a method group.

Via reflection, attribute arguments of this type are exposed as `MemberInfo` (`FieldInfo` or `PropertyInfo`), and for analyzers as `IFieldSymbol` or `IPropertySymbol`.

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

//...

void Bar()
{
    RuntimeConstant<City> city = new RuntimeConstant<City>(Cities.BellevueWA);
}
```


Framework type to be added.  Ideally it would not have public constructors, as it would only be created during attribute instantiation, or via special IL like delegates, and so the constructors could omit the runtime guards.

```csharp
public readonly struct RuntimeConstant<T>
{
    public MemberInfo info { get; }
    
    public RuntimeConstant(FieldInfo info)
    {
        if (!typeof(T).IsAssignableFrom(info.FieldType)) throw new InvalidCastException();
        // also guard for IsLiteral || (IsStatic && IsInitOnly) ?
        this.info = info;
    }
    public RuntimeConstant(PropertyInfo info)
    {
        if (!typeof(T).IsAssignableFrom(info.PropertyType)) throw new InvalidCastException();
        // also guard for static, get-only?
        this.info = info;
    }
    
    public T GetValue() => (T)(info is PropertyInfo pi ? pi.GetValue(null) : ((FieldInfo)info).GetValue(null));
}
```
