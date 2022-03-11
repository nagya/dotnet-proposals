using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly ref struct ByRef<T>
    {
        private readonly Span<T> span;
        public ByRef(ref T target) => this.span = MemoryMarshal.CreateSpan(ref target, 0);
        public ref T Target => ref MemoryMarshal.GetReference(span);
        public static ByRef<T> Null => default;
        public bool IsNull => span == Span<T>.Empty;
        public bool HasTarget => !IsNull;

        public static implicit operator ByRefReadOnly<T>(ByRef<T> ptr) => new ByRefReadOnly<T>(ptr.span);
        public static implicit operator ByRef(ByRef<T> ptr) => ByRef.New(ref ptr.Target);
        public static implicit operator ByRefReadOnly(ByRef<T> ptr) => ByRefReadOnly.New(in ptr.Target);
        public static explicit operator ByRef<T>(ByRef tptr) => new ByRef<T>(ref tptr.TargetAs<T>());
        
        public static bool operator ==(ByRef<T> left, ByRef<T> right) => left.span == right.span;
        public static bool operator !=(ByRef<T> left, ByRef<T> right) => left.span != right.span;
        
        public override string? ToString() => IsNull || Target == null ? "" : Target.ToString(); 
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public readonly ref struct ByRefReadOnly<T>
    {
        private readonly ReadOnlySpan<T> span;
        internal ByRefReadOnly(ReadOnlySpan<T> span) => this.span = span;
        public ByRefReadOnly(in T target) => this.span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in target), 0);
        public ref readonly T Target => ref MemoryMarshal.GetReference(span);
        public static ByRefReadOnly<T> Null => default;
        public bool IsNull => span == ReadOnlySpan<T>.Empty;
        public bool HasTarget => !IsNull;

        public static implicit operator ByRefReadOnly(ByRefReadOnly<T> ptr) => ByRefReadOnly.New(in ptr.Target);
        public static explicit operator ByRefReadOnly<T>(ByRef tptr) => new ByRefReadOnly<T>(in tptr.TargetAs<T>());
        public static explicit operator ByRefReadOnly<T>(ByRefReadOnly tptr) => new ByRefReadOnly<T>(in tptr.TargetAs<T>());

        public static bool operator ==(ByRefReadOnly<T> left, ByRefReadOnly<T> right) => left.span == right.span;
        public static bool operator !=(ByRefReadOnly<T> left, ByRefReadOnly<T> right) => left.span != right.span;
        
        public override string? ToString() => IsNull || Target == null ? "" : Target.ToString(); 
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public readonly ref struct ByRef
    {
        private readonly ByRefInfo? info;
        private readonly ByRef<Any> ptr;

        private ByRef(ByRefInfo info, ByRef<Any> ptr)
        {
            this.info = info;
            this.ptr = ptr;
        }

        public static ByRef Default => default;
        public bool IsDefault => info == null;
        public bool HasType => info != null;
        
        public static ByRef Null<T>() => new ByRef(ByRefInfo<T>.instance, ByRef<Any>.Null);
        public bool IsNull => ptr.IsNull;
        public bool HasTarget => ptr.HasTarget;
        
        public static ByRef New<T>(ref T target)
        {
            return new ByRef(ByRefInfo<T>.instance, new ByRef<Any>(ref Unsafe.As<T, Any>(ref target)));
        }
        
        public Type? Type => info?.Type;
        
        public bool Is<T>() => info is ByRefInfo<T>;
        
        public ref T TargetAs<T>()
        {
            if (Is<T>())
                return ref Unsafe.As<Any, T>(ref ptr.Target);
            else
                throw new InvalidCastException();
        }

        public bool TryGetTarget<T>([MaybeNullWhen(false)] out T value)
        {
            if (Is<T>() && HasTarget)
            {
                value = Unsafe.As<Any, T>(ref ptr.Target);
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
        
        public static implicit operator ByRefReadOnly(ByRef tptr) => new ByRefReadOnly(tptr.info, tptr.ptr);

        public static bool operator ==(ByRef left, ByRef right) => left.info == right.info && left.ptr == right.ptr;
        public static bool operator !=(ByRef left, ByRef right) => !(left == right);
        
        public object? ToObject() => IsNull ? null : info!.ToObject(ref ptr.Target);
        public override string? ToString() => IsNull ? "" : info!.ToString(ref ptr.Target); 
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }
    
    public readonly ref struct ByRefReadOnly
    {
        private readonly ByRefInfo? info;
        private readonly ByRef<Any> ptr;

        internal ByRefReadOnly(ByRefInfo? info, ByRef<Any> ptr)
        {
            this.info = info;
            this.ptr = ptr;
        }
        
        public static ByRefReadOnly Default => default;
        public bool IsDefault => info == null;
        public bool HasType => info != null;
        
        public static ByRefReadOnly Null<T>() => new ByRefReadOnly(ByRefInfo<T>.instance, ByRef<Any>.Null);
        public bool IsNull => ptr.IsNull;
        public bool HasTarget => ptr.HasTarget;

        public static ByRefReadOnly New<T>(in T target)
        {
            return new ByRefReadOnly(ByRefInfo<T>.instance, new ByRef<Any>(ref Unsafe.As<T, Any>(ref Unsafe.AsRef(in target))));
        }

        public Type? Type => info?.Type;
        
        public bool Is<T>() => info is IByRefInfo<T>;
        
        public ref readonly T TargetAs<T>()
        {
            if (Is<T>())
                return ref Unsafe.As<Any, T>(ref ptr.Target);
            else
                throw new InvalidCastException();
        }
        
        public bool TryGetTarget<T>([MaybeNullWhen(false)] out T value)
        {
            if (Is<T>() && HasTarget)
            {
                value = Unsafe.As<Any, T>(ref ptr.Target);
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
        
        public static bool operator ==(ByRefReadOnly left, ByRefReadOnly right) => left.info == right.info && left.ptr == right.ptr;
        public static bool operator !=(ByRefReadOnly left, ByRefReadOnly right) => !(left == right);

        public object? ToObject() => IsNull ? null : info!.ToObject(ref ptr.Target);
        public override string? ToString() => IsNull ? "" : info!.ToString(ref ptr.Target); 
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }
    
    internal readonly struct Any {}

    internal abstract class ByRefInfo
    {
        private protected ByRefInfo() {}
        internal abstract Type Type { get; }
        internal abstract string? ToString(ref Any target);
        internal abstract object? ToObject(ref Any target);
    }
    
    internal interface IByRefInfo<out T> {}

    internal sealed class ByRefInfo<T> : ByRefInfo, IByRefInfo<T>
    {
        private ByRefInfo() {}
        internal static readonly ByRefInfo<T> instance = new ByRefInfo<T>();
        
        internal override Type Type => typeof(T);
        internal override string? ToString(ref Any target) => Unsafe.As<Any, T>(ref target)?.ToString() ?? "";
        internal override object? ToObject(ref Any target) => Unsafe.As<Any, T>(ref target);
    }
}