# Make `ByReference<T>` public

Make `ByReference<T>` public and remove the obsolete language restrictions on the use of `TypedReference`. Alternatively, support ref fields in ref structs.

Motivation: Performance improvement in reflection-like or serialization APIs that return interior pointers into heap objects along with additional metadata.

Details: Discussed extensively on the C# language github. Note that it can be already trivially implemented ([see appendix](ByRef.cs)) with ref structs that wrap a `Span<T>`, but that approach has the overhead of having to store the length of the span.
