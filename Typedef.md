# Typedef

Typedef (as in C/C++) is a feature similar to a global using, in that it defines an additional alias name for an existing type (or type expression), but in a manner that works across assemblies, and the intent of using the typedef in code, instead of the type it aliases, is recorded in metadata which can be read by analyzers. 

Typedef declarations can have generic parameters and attributes, and they live in namespaces, just like type declarations. Internally, the typedefs themselves could be represented as e.g. static classes in IL and their use would be represented in metadata similarly to nullable annotations, i.e., attributes on parameters etc.

Motivation: Analyzers can be aware of the typedef's semantics (or infer it from metadata) and provide not just type safety, but stronger guarantees associated with those semantics. Nullability annotations are a precedent for how associating "paint" with types, combined with compile time analysis, is a legitimate approach to improving type safety of the code beyond what the traditional type system allows.

Example:

```csharp
typedef Width = System.Double;
```

Alternatively, 

```csharp
typedef Width : System.Double
{
    // extension methods
}
```

By default, `Width` just means `Double` everywhere, but the fact that it is a `Width` is available in metadata for analyzers to use, just like nullable annotations are available for analyzers to use. Analyzer rules could be hard-coded or driven by metadata annotated with attributes on the typedef.   

```csharp
 void Draw(Width width) => ...;
  
 // in another file or assembly:
 Width width = 3;
 Draw(width); // okay
  
 Height height = 3;
 Draw(height); // an analyzer could raise a warning here
```
