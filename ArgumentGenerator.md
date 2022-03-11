# Source code generators for optional arguments

Source code generators should be able to generate expressions (not just constants) to fill in missing optional arguments at call sites, like the compiler does for parameters decorated with `CallerMemberNameAttribute`.

Motivation: This allows for safer and more expressive APIs and usages by allowing developers to create code generators that automate the passing of factories, tokens (e.g., `CancellationToken`), and settings (e.g., `CultureInfo`).

Developers would be able to define custom attributes to be used on parameter declarations. Such an attribute declaration would subclass the framework-provided `GeneratedArgumentAttribute` or `OptionalGeneratedArgumentAttribute`, and source generators would register themselves to generate missing arguments for parameters annotated with the attribute. The "optional" distinction specifies whether it'd be a compile error if no source generator is registered for a particular attribute.

Example attribute declaration:

```csharp
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FooFactoryAttribute : GeneratedArgumentAttribute {}
```

Corresponding code generator:

```csharp
[ArgumentGenerator("Example.FooFactoryAttribute")]
public class FooFactoryGenerator : IArgumentGenerator {...}
```

Sample code where the code generator would be used:

```csharp
public Foo New(int x, [FooFactory] Factory factory = default) 
{ ... }
  
void DoSomething([FooFactory] Factory factory = default)
{
    // correct factory argument (passing through the incoming parameter) 
    // would be provided by a custom source generator enabled by the proposed feature
    Foo foo = New(0); 
}
```
