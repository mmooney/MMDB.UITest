﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Holds information about a conversion between two types.
	/// </summary>
	public struct Conversion : IEquatable<Conversion>
	{
		public static readonly Conversion None = default(Conversion);
		public static readonly Conversion IdentityConversion = new Conversion(1);
		public static readonly Conversion ImplicitNumericConversion = new Conversion(2);
		public static readonly Conversion ImplicitEnumerationConversion = new Conversion(3);
		public static readonly Conversion ImplicitNullableConversion = new Conversion(4);
		public static readonly Conversion NullLiteralConversion = new Conversion(5);
		public static readonly Conversion ImplicitReferenceConversion = new Conversion(6);
		public static readonly Conversion BoxingConversion = new Conversion(7);
		public static readonly Conversion ImplicitDynamicConversion = new Conversion(8);
		public static readonly Conversion ImplicitConstantExpressionConversion = new Conversion(9);
		const int userDefinedImplicitConversionKind = 10;
		const int liftedUserDefinedImplicitConversionKind = 11;
		public static readonly Conversion ImplicitPointerConversion = new Conversion(12);
		const int anonymousFunctionConversionKind = 13;
		const int methodGroupConversionKind = 14;
		public static readonly Conversion ExplicitNumericConversion = new Conversion(15);
		public static readonly Conversion ExplicitEnumerationConversion = new Conversion(16);
		public static readonly Conversion ExplicitNullableConversion = new Conversion(17);
		public static readonly Conversion ExplicitReferenceConversion = new Conversion(18);
		public static readonly Conversion UnboxingConversion = new Conversion(19);
		public static readonly Conversion ExplicitDynamicConversion = new Conversion(20);
		public static readonly Conversion ExplicitPointerConversion = new Conversion(21);
		const int userDefinedExplicitConversionKind = 22;
		const int liftedUserDefinedExplicitConversionKind = 23;
		
		const int lastImplicitConversion = methodGroupConversionKind;
		
		static readonly string[] conversionNames = {
			"None",
			"Identity conversion",
			"Implicit numeric conversion",
			"Implicit enumeration conversion",
			"Implicit nullable conversion",
			"Null literal conversion",
			"Implicit reference conversion",
			"Boxing conversion",
			"Implicit dynamic conversion",
			"Implicit constant expression conversion",
			"User-defined implicit conversion",
			"Lifted user-defined implicit conversion",
			"Implicit pointer conversion",
			"Anonymous function conversion",
			"Method group conversion",
			"Explicit numeric conversion",
			"Explicit enumeration conversion",
			"Explicit nullable conversion",
			"Explicit reference conversion",
			"Unboxing conversion",
			"Explicit dynamic conversion",
			"Explicit pointer conversion",
			"User-defined explicit conversion",
			"Lifted user-defined explicit conversion"
		};
		
		public static Conversion UserDefinedImplicitConversion(IMethod operatorMethod, bool isLifted)
		{
			if (operatorMethod == null)
				throw new ArgumentNullException("operatorMethod");
			return new Conversion(isLifted ? liftedUserDefinedImplicitConversionKind : userDefinedImplicitConversionKind, operatorMethod);
		}
		
		public static Conversion UserDefinedExplicitConversion(IMethod operatorMethod, bool isLifted)
		{
			if (operatorMethod == null)
				throw new ArgumentNullException("operatorMethod");
			return new Conversion(isLifted ? liftedUserDefinedExplicitConversionKind : userDefinedExplicitConversionKind, operatorMethod);
		}
		
		public static Conversion MethodGroupConversion(IMethod chosenMethod)
		{
			if (chosenMethod == null)
				throw new ArgumentNullException("chosenMethod");
			return new Conversion(methodGroupConversionKind, chosenMethod);
		}
		
		/// <summary>
		/// Creates a new anonymous function conversion.
		/// </summary>
		/// <param name="data">Used by ResolveVisitor to pass the LambdaTypeHypothesis.</param>
		public static Conversion AnonymousFunctionConversion(object data)
		{
			return new Conversion(anonymousFunctionConversionKind, data);
		}
		
		readonly int kind;
		internal readonly object data;
		
		public Conversion(int kind, object data = null)
		{
			this.kind = kind;
			this.data = data;
		}
		
		/// <summary>
		/// Gets whether this conversion is an implicit conversion.
		/// </summary>
		public bool IsImplicitConversion {
			get { return kind > 0 && kind <= lastImplicitConversion; }
		}
		
		/// <summary>
		/// Gets whether this conversion is an explicit conversion.
		/// </summary>
		public bool IsExplicitConversion {
			get { return kind > lastImplicitConversion; }
		}
		
		/// <summary>
		/// Gets whether this conversion is user-defined.
		/// </summary>
		public bool IsUserDefined {
			get {
				switch (kind) {
					case userDefinedImplicitConversionKind:
					case liftedUserDefinedImplicitConversionKind:
					case userDefinedExplicitConversionKind:
					case liftedUserDefinedExplicitConversionKind:
						return true;
					default:
						return false;
				}
			}
		}
		
		/// <summary>
		/// Gets whether this conversion is a lifted version of a user-defined conversion operator.
		/// </summary>
		/// <remarks>Lifted versions of builtin conversion operators are classified as nullable-conversion</remarks>
		public bool IsLifted {
			get {
				return kind == liftedUserDefinedImplicitConversionKind || kind == liftedUserDefinedExplicitConversionKind;
			}
		}
		
		/// <summary>
		/// Gets whether this conversion is a method group conversion.
		/// </summary>
		public bool IsMethodGroupConversion {
			get { return kind == methodGroupConversionKind; }
		}
		
		/// <summary>
		/// Gets whether this conversion is an anonymous function conversion.
		/// </summary>
		public bool IsAnonymousFunctionConversion {
			get { return kind == anonymousFunctionConversionKind; }
		}
		
		/// <summary>
		/// Gets the method associated with this conversion.
		/// For user-defined conversions, this is the method being called.
		/// For method-group conversions, this is the method that was chosen from the group.
		/// </summary>
		public IMethod Method {
			get { return data as IMethod; }
		}
		
		public override string ToString()
		{
			string name = conversionNames[kind];
			if (data != null)
				return name + " (" + data + ")";
			else
				return name;
		}
		
		public bool IsValid {
			get { return kind > 0; }
		}
		
		public static implicit operator bool(Conversion conversion)
		{
			return conversion.kind > 0;
		}
		
		#region Equals and GetHashCode implementation
		public override int GetHashCode()
		{
			if (data != null)
				return kind ^ data.GetHashCode();
			else
				return kind;
		}
		
		public override bool Equals(object obj)
		{
			return (obj is Conversion) && Equals((Conversion)obj);
		}
		
		public bool Equals(Conversion other)
		{
			return this.kind == other.kind && object.Equals(this.data, other.data);
		}
		
		public static bool operator ==(Conversion lhs, Conversion rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Conversion lhs, Conversion rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	
	/// <summary>
	/// Contains logic that determines whether an implicit conversion exists between two types.
	/// </summary>
	public class Conversions
	{
		readonly Dictionary<TypePair, Conversion> implicitConversionCache = new Dictionary<TypePair, Conversion>();
		readonly ITypeResolveContext context;
		readonly IType objectType;
		
		public Conversions(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.objectType = KnownTypeReference.Object.Resolve(context);
			this.dynamicErasure = new DynamicErasure(this);
		}
		
		/// <summary>
		/// Gets the Conversions instance for the specified <see cref="ITypeResolveContext"/>.
		/// This will make use of the context's cache manager (if available) to reuse the Conversions instance.
		/// </summary>
		public static Conversions Get(ITypeResolveContext context)
		{
			CacheManager cache = context.CacheManager;
			if (cache != null) {
				Conversions conversions = cache.GetThreadLocal(typeof(Conversions)) as Conversions;
				if (conversions == null) {
					conversions = new Conversions(context);
					cache.SetThreadLocal(typeof(Conversions), conversions);
				}
				return conversions;
			} else {
				return new Conversions(context);
			}
		}
		
		#region TypePair (for caching)
		struct TypePair : IEquatable<TypePair>
		{
			public readonly IType FromType;
			public readonly IType ToType;
			
			public TypePair(IType fromType, IType toType)
			{
				this.FromType = fromType;
				this.ToType = toType;
			}
			
			public override bool Equals(object obj)
			{
				return (obj is TypePair) && Equals((TypePair)obj);
			}
			
			public bool Equals(TypePair other)
			{
				return this.FromType.Equals(other.FromType) && this.ToType.Equals(other.ToType);
			}
			
			public override int GetHashCode()
			{
				unchecked {
					return 1000000007 * FromType.GetHashCode() + 1000000009 * ToType.GetHashCode();
				}
			}
		}
		#endregion
		
		#region ImplicitConversion
		public Conversion ImplicitConversion(ResolveResult resolveResult, IType toType)
		{
			if (resolveResult == null)
				throw new ArgumentNullException("resolveResult");
			if (resolveResult.IsCompileTimeConstant) {
				if (ImplicitEnumerationConversion(resolveResult, toType))
					return Conversion.ImplicitEnumerationConversion;
				if (ImplicitConstantExpressionConversion(resolveResult, toType))
					return Conversion.ImplicitConstantExpressionConversion;
			}
			Conversion c;
			c = ImplicitConversion(resolveResult.Type, toType);
			if (c) return c;
			c = AnonymousFunctionConversion(resolveResult, toType);
			if (c) return c;
			c = MethodGroupConversion(resolveResult, toType);
			return c;
		}
		
		public Conversion ImplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			TypePair pair = new TypePair(fromType, toType);
			Conversion c;
			if (implicitConversionCache.TryGetValue(pair, out c))
				return c;
			
			// C# 4.0 spec: §6.1
			c = StandardImplicitConversion(fromType, toType);
			if (!c) {
				c = UserDefinedImplicitConversion(fromType, toType);
			}
			implicitConversionCache[pair] = c;
			return c;
		}
		
		public Conversion StandardImplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			// C# 4.0 spec: §6.3.1
			if (IdentityConversion(fromType, toType))
				return Conversion.IdentityConversion;
			if (ImplicitNumericConversion(fromType, toType))
				return Conversion.ImplicitNumericConversion;
			if (ImplicitNullableConversion(fromType, toType))
				return Conversion.ImplicitNullableConversion;
			if (NullLiteralConversion(fromType, toType))
				return Conversion.NullLiteralConversion;
			if (ImplicitReferenceConversion(fromType, toType))
				return Conversion.ImplicitReferenceConversion;
			if (BoxingConversion(fromType, toType))
				return Conversion.BoxingConversion;
			if (fromType.Kind == TypeKind.Dynamic)
				return Conversion.ImplicitDynamicConversion;
			if (ImplicitTypeParameterConversion(fromType, toType)) {
				// Implicit type parameter conversions that aren't also
				// reference conversions are considered to be boxing conversions
				return Conversion.BoxingConversion;
			}
			if (ImplicitPointerConversion(fromType, toType))
				return Conversion.ImplicitPointerConversion;
			return Conversion.None;
		}
		
		/// <summary>
		/// Gets whether the type 'fromType' is convertible to 'toType'
		/// using one of the conversions allowed when satisying constraints (§4.4.4)
		/// </summary>
		public bool IsConstraintConvertible(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			if (IdentityConversion(fromType, toType))
				return true;
			if (ImplicitReferenceConversion(fromType, toType))
				return true;
			if (BoxingConversion(fromType, toType) && !NullableType.IsNullable(fromType))
				return true;
			if (ImplicitTypeParameterConversion(fromType, toType))
				return true;
			return false;
		}
		#endregion
		
		#region ExplicitConversion
		public Conversion ExplicitConversion(ResolveResult resolveResult, IType toType)
		{
			if (resolveResult == null)
				throw new ArgumentNullException("resolveResult");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			if (resolveResult.Type.Kind == TypeKind.Dynamic)
				return Conversion.ExplicitDynamicConversion;
			Conversion c = ImplicitConversion(resolveResult, toType);
			if (c)
				return c;
			else
				return ExplicitConversionImpl(resolveResult.Type, toType);
		}
		
		public Conversion ExplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			if (fromType.Kind == TypeKind.Dynamic)
				return Conversion.ExplicitDynamicConversion;
			Conversion c = ImplicitConversion(fromType, toType);
			if (c)
				return c;
			else
				return ExplicitConversionImpl(fromType, toType);
		}
		
		Conversion ExplicitConversionImpl(IType fromType, IType toType)
		{
			// This method is called after we already checked for implicit conversions,
			// so any remaining conversions must be explicit.
			if (AnyNumericConversion(fromType, toType))
				return Conversion.ExplicitNumericConversion;
			if (ExplicitEnumerationConversion(fromType, toType))
				return Conversion.ExplicitEnumerationConversion;
			if (ExplicitNullableConversion(fromType, toType))
				return Conversion.ExplicitNullableConversion;
			if (ExplicitReferenceConversion(fromType, toType))
				return Conversion.ExplicitReferenceConversion;
			if (UnboxingConversion(fromType, toType))
				return Conversion.UnboxingConversion;
			if (ExplicitTypeParameterConversion(fromType, toType)) {
				// Explicit type parameter conversions that aren't also
				// reference conversions are considered to be unboxing conversions
				return Conversion.UnboxingConversion;
			}
			if (ExplicitPointerConversion(fromType, toType))
				return Conversion.ExplicitPointerConversion;
			return UserDefinedExplicitConversion(fromType, toType);
		}
		#endregion
		
		#region Identity Conversion
		/// <summary>
		/// Gets whether there is an identity conversion from <paramref name="fromType"/> to <paramref name="toType"/>
		/// </summary>
		public bool IdentityConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.1
			return fromType.AcceptVisitor(dynamicErasure).Equals(toType.AcceptVisitor(dynamicErasure));
		}
		
		readonly DynamicErasure dynamicErasure;
		
		sealed class DynamicErasure : TypeVisitor
		{
			readonly IType objectType;
			
			public DynamicErasure(Conversions conversions)
			{
				this.objectType = conversions.objectType;
			}
			
			public override IType VisitOtherType(IType type)
			{
				if (type == SharedTypes.Dynamic)
					return objectType;
				else
					return base.VisitOtherType(type);
			}
		}
		#endregion
		
		#region Numeric Conversions
		static readonly bool[,] implicitNumericConversionLookup = {
			//       to:   short  ushort  int   uint   long   ulong
			// from:
			/* char   */ { false, true , true , true , true , true  },
			/* sbyte  */ { true , false, true , false, true , false },
			/* byte   */ { true , true , true , true , true , true  },
			/* short  */ { false, false, true , false, true , false },
			/* ushort */ { false, false, true , true , true , true  },
			/* int    */ { false, false, false, false, true , false },
			/* uint   */ { false, false, false, false, true , true  },
		};
		
		bool ImplicitNumericConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.2
			
			TypeCode from = ReflectionHelper.GetTypeCode(fromType);
			TypeCode to = ReflectionHelper.GetTypeCode(toType);
			if (to >= TypeCode.Single && to <= TypeCode.Decimal) {
				// Conversions to float/double/decimal exist from all integral types,
				// and there's a conversion from float to double.
				return from >= TypeCode.Char && from <= TypeCode.UInt64
					|| from == TypeCode.Single && to == TypeCode.Double;
			} else {
				// Conversions to integral types: look at the table
				return from >= TypeCode.Char && from <= TypeCode.UInt32
					&& to >= TypeCode.Int16 && to <= TypeCode.UInt64
					&& implicitNumericConversionLookup[from - TypeCode.Char, to - TypeCode.Int16];
			}
		}
		
		bool IsNumericType(IType type)
		{
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			return c >= TypeCode.Char && c <= TypeCode.Decimal;
		}
		
		bool AnyNumericConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.2 + §6.2.1
			return IsNumericType(fromType) && IsNumericType(toType);
		}
		#endregion
		
		#region Enumeration Conversions
		bool ImplicitEnumerationConversion(ResolveResult rr, IType toType)
		{
			// C# 4.0 spec: §6.1.3
			Debug.Assert(rr.IsCompileTimeConstant);
			TypeCode constantType = ReflectionHelper.GetTypeCode(rr.Type);
			if (constantType >= TypeCode.SByte && constantType <= TypeCode.Decimal && Convert.ToDouble(rr.ConstantValue) == 0) {
				return NullableType.GetUnderlyingType(toType).Kind == TypeKind.Enum;
			}
			return false;
		}
		
		bool ExplicitEnumerationConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.2.2
			if (fromType.Kind == TypeKind.Enum) {
				return toType.Kind == TypeKind.Enum || IsNumericType(toType);
			} else if (IsNumericType(fromType)) {
				return toType.Kind == TypeKind.Enum;
			}
			return false;
		}
		#endregion
		
		#region Nullable Conversions
		bool ImplicitNullableConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.4
			if (NullableType.IsNullable(toType)) {
				IType t = NullableType.GetUnderlyingType(toType);
				IType s = NullableType.GetUnderlyingType(fromType); // might or might not be nullable
				return IdentityConversion(s, t) || ImplicitNumericConversion(s, t);
			} else {
				return false;
			}
		}
		
		bool ExplicitNullableConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.4
			if (NullableType.IsNullable(toType) || NullableType.IsNullable(fromType)) {
				IType t = NullableType.GetUnderlyingType(toType);
				IType s = NullableType.GetUnderlyingType(fromType);
				return IdentityConversion(s, t) || AnyNumericConversion(s, t) || ExplicitEnumerationConversion(s, t);
			} else {
				return false;
			}
		}
		#endregion
		
		#region Null Literal Conversion
		bool NullLiteralConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.5
			if (SharedTypes.Null.Equals(fromType)) {
				return NullableType.IsNullable(toType) || toType.IsReferenceType(context) == true;
			} else {
				return false;
			}
		}
		#endregion
		
		#region Implicit Reference Conversion
		bool ImplicitReferenceConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.6
			
			// reference conversions are possible only if both types are known to be reference types
			if (!(fromType.IsReferenceType(context) == true && toType.IsReferenceType(context) == true))
				return false;
			
			ArrayType fromArray = fromType as ArrayType;
			if (fromArray != null) {
				ArrayType toArray = toType as ArrayType;
				if (toArray != null) {
					// array covariance (the broken kind)
					return fromArray.Dimensions == toArray.Dimensions
						&& ImplicitReferenceConversion(fromArray.ElementType, toArray.ElementType);
				}
				// conversion from single-dimensional array S[] to IList<T>:
				ParameterizedType toPT = toType as ParameterizedType;
				if (fromArray.Dimensions == 1 && toPT != null && toPT.TypeParameterCount == 1
				    && toPT.Namespace == "System.Collections.Generic"
				    && (toPT.Name == "IList" || toPT.Name == "ICollection" || toPT.Name == "IEnumerable"))
				{
					// array covariance plays a part here as well (string[] is IList<object>)
					return IdentityConversion(fromArray.ElementType, toPT.GetTypeArgument(0))
						|| ImplicitReferenceConversion(fromArray.ElementType, toPT.GetTypeArgument(0));
				}
				// conversion from any array to System.Array and the interfaces it implements:
				ITypeDefinition systemArray = context.GetTypeDefinition("System", "Array", 0, StringComparer.Ordinal);
				return systemArray != null && (systemArray.Equals(toType) || ImplicitReferenceConversion(systemArray, toType));
			}
			
			// now comes the hard part: traverse the inheritance chain and figure out generics+variance
			return IsSubtypeOf(fromType, toType);
		}
		
		// Determines whether s is a subtype of t.
		// Helper method used for ImplicitReferenceConversion, BoxingConversion and ImplicitTypeParameterConversion
		
		int subtypeCheckNestingDepth;
		
		bool IsSubtypeOf(IType s, IType t)
		{
			// conversion to dynamic + object are always possible
			if (t.Equals(SharedTypes.Dynamic) || t.Equals(objectType))
				return true;
			try {
				if (++subtypeCheckNestingDepth > 10) {
					// Subtyping in C# is undecidable
					// (see "On Decidability of Nominal Subtyping with Variance" by Andrew J. Kennedy and Benjamin C. Pierce),
					// so we'll prevent infinite recursions by putting a limit on the nesting depth of variance conversions.
					
					// No real C# code should use generics nested more than 10 levels deep, and even if they do, most of
					// those nestings should not involve variance.
					return false;
				}
				// let GetAllBaseTypes do the work for us
				foreach (IType baseType in s.GetAllBaseTypes(context)) {
					if (IdentityOrVarianceConversion(baseType, t))
						return true;
				}
				return false;
			} finally {
				subtypeCheckNestingDepth--;
			}
		}
		
		bool IdentityOrVarianceConversion(IType s, IType t)
		{
			ITypeDefinition def = s.GetDefinition();
			if (def != null && def.Equals(t.GetDefinition())) {
				ParameterizedType ps = s as ParameterizedType;
				ParameterizedType pt = t as ParameterizedType;
				if (ps != null && pt != null) {
					// C# 4.0 spec: §13.1.3.2 Variance Conversion
					for (int i = 0; i < def.TypeParameters.Count; i++) {
						IType si = ps.GetTypeArgument(i);
						IType ti = pt.GetTypeArgument(i);
						if (IdentityConversion(si, ti))
							continue;
						ITypeParameter xi = def.TypeParameters[i];
						switch (xi.Variance) {
							case VarianceModifier.Covariant:
								if (!ImplicitReferenceConversion(si, ti))
									return false;
								break;
							case VarianceModifier.Contravariant:
								if (!ImplicitReferenceConversion(ti, si))
									return false;
								break;
							default:
								return false;
						}
					}
				} else if (ps != null || pt != null) {
					return false; // only of of them is parameterized, or counts don't match? -> not valid conversion
				}
				return true;
			}
			return false;
		}
		#endregion
		
		#region Explicit Reference Conversion
		bool ExplicitReferenceConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.2.4
			
			// reference conversions are possible only if both types are known to be reference types
			if (!(fromType.IsReferenceType(context) == true && toType.IsReferenceType(context) == true))
				return false;
			
			// There's lots of additional rules, but they're not really relevant,
			// as they are only used to identify invalid casts, and we don't care about reporting those.
			return true;
		}
		#endregion
		
		#region Boxing Conversions
		bool BoxingConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.7
			fromType = NullableType.GetUnderlyingType(fromType);
			if (fromType.IsReferenceType(context) == false && toType.IsReferenceType(context) == true)
				return IsSubtypeOf(fromType, toType);
			else
				return false;
		}
		
		bool UnboxingConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.2.5
			toType = NullableType.GetUnderlyingType(toType);
			if (fromType.IsReferenceType(context) == true && toType.IsReferenceType(context) == false)
				return IsSubtypeOf(toType, fromType);
			else
				return false;
		}
		#endregion
		
		#region Implicit Constant-Expression Conversion
		bool ImplicitConstantExpressionConversion(ResolveResult rr, IType toType)
		{
			// C# 4.0 spec: §6.1.9
			Debug.Assert(rr.IsCompileTimeConstant);
			TypeCode fromTypeCode = ReflectionHelper.GetTypeCode(rr.Type);
			TypeCode toTypeCode = ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(toType));
			if (fromTypeCode == TypeCode.Int64) {
				long val = (long)rr.ConstantValue;
				return val >= 0 && toTypeCode == TypeCode.UInt64;
			} else if (fromTypeCode == TypeCode.Int32) {
				int val = (int)rr.ConstantValue;
				switch (toTypeCode) {
					case TypeCode.SByte:
						return val >= SByte.MinValue && val <= SByte.MaxValue;
					case TypeCode.Byte:
						return val >= Byte.MinValue && val <= Byte.MaxValue;
					case TypeCode.Int16:
						return val >= Int16.MinValue && val <= Int16.MaxValue;
					case TypeCode.UInt16:
						return val >= UInt16.MinValue && val <= UInt16.MaxValue;
					case TypeCode.UInt32:
						return val >= 0;
					case TypeCode.UInt64:
						return val >= 0;
				}
			}
			return false;
		}
		#endregion
		
		#region Conversions involving type parameters
		/// <summary>
		/// Implicit conversions involving type parameters.
		/// </summary>
		bool ImplicitTypeParameterConversion(IType fromType, IType toType)
		{
			if (fromType.Kind != TypeKind.TypeParameter)
				return false; // not a type parameter
			if (fromType.IsReferenceType(context) == true)
				return false; // already handled by ImplicitReferenceConversion
			return IsSubtypeOf(fromType, toType);
		}
		
		bool ExplicitTypeParameterConversion(IType fromType, IType toType)
		{
			if (toType.Kind == TypeKind.TypeParameter) {
				return fromType.Kind == TypeKind.TypeParameter || fromType.IsReferenceType(context) == true;
			} else {
				return fromType.Kind == TypeKind.TypeParameter && toType.Kind == TypeKind.Interface;
			}
		}
		#endregion
		
		#region Pointer Conversions
		bool ImplicitPointerConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §18.4 Pointer conversions
			if (fromType is PointerType && toType is PointerType && toType.ReflectionName == "System.Void*")
				return true;
			if (SharedTypes.Null.Equals(fromType) && toType is PointerType)
				return true;
			return false;
		}
		
		bool ExplicitPointerConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §18.4 Pointer conversions
			if (fromType.Kind == TypeKind.Pointer) {
				return toType.Kind == TypeKind.Pointer || IsIntegerType(toType);
			} else {
				return toType.Kind == TypeKind.Pointer && IsIntegerType(fromType);
			}
		}
		
		bool IsIntegerType(IType type)
		{
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			return c >= TypeCode.SByte && c <= TypeCode.UInt64;
		}
		#endregion
		
		#region User-Defined Conversions
		/// <summary>
		/// Gets whether type A is encompassed by type B.
		/// </summary>
		bool IsEncompassedBy(IType a, IType b)
		{
			return a.Kind != TypeKind.Interface && b.Kind != TypeKind.Interface && StandardImplicitConversion(a, b);
		}
		
		bool IsEncompassingOrEncompassedBy(IType a, IType b)
		{
			return a.Kind != TypeKind.Interface && b.Kind != TypeKind.Interface
				&& (StandardImplicitConversion(a, b) || StandardImplicitConversion(b, a));
		}
		
		Conversion UserDefinedImplicitConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec §6.4.4 User-defined implicit conversions
			var operators = GetApplicableConversionOperators(fromType, toType, false);
			// TODO: Find most specific conversion
			if (operators.Count > 0)
				return Conversion.UserDefinedImplicitConversion(operators[0].Method, operators[0].IsLifted);
			else
				return Conversion.None;
		}
		
		Conversion UserDefinedExplicitConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec §6.4.5 User-defined implicit conversions
			var operators = GetApplicableConversionOperators(fromType, toType, true);
			// TODO: Find most specific conversion
			if (operators.Count > 0)
				return Conversion.UserDefinedExplicitConversion(operators[0].Method, operators[0].IsLifted);
			else
				return Conversion.None;
		}
		
		class OperatorInfo
		{
			public readonly IMethod Method;
			public readonly IType SourceType;
			public readonly IType TargetType;
			public readonly bool IsLifted;
			
			public OperatorInfo(IMethod method, IType sourceType, IType targetType, bool isLifted)
			{
				this.Method = method;
				this.SourceType = sourceType;
				this.TargetType = targetType;
				this.IsLifted = isLifted;
			}
		}
		
		List<OperatorInfo> GetApplicableConversionOperators(IType fromType, IType toType, bool isExplicit)
		{
			// Find the candidate operators:
			Predicate<IMethod> opFilter;
			if (isExplicit)
				opFilter = m => m.IsStatic && m.IsOperator && m.Name == "op_Explicit" && m.Parameters.Count == 1;
			else
				opFilter = m => m.IsStatic && m.IsOperator && m.Name == "op_Implicit" && m.Parameters.Count == 1;
			
			var operators = NullableType.GetUnderlyingType(fromType).GetMethods(context, opFilter)
				.Concat(NullableType.GetUnderlyingType(toType).GetMethods(context, opFilter)).Distinct();
			// Determine whether one of them is applicable:
			List<OperatorInfo> result = new List<OperatorInfo>();
			foreach (IMethod op in operators) {
				IType sourceType = op.Parameters[0].Type.Resolve(context);
				IType targetType = op.ReturnType.Resolve(context);
				// Try if the operator is applicable:
				bool isApplicable;
				if (isExplicit) {
					isApplicable = IsEncompassingOrEncompassedBy(fromType, sourceType)
						&& IsEncompassingOrEncompassedBy(targetType, toType);
				} else {
					isApplicable = IsEncompassedBy(fromType, sourceType) && IsEncompassedBy(targetType, toType);
				}
				if (isApplicable) {
					result.Add(new OperatorInfo(op, sourceType, targetType, false));
				}
				// Try if the operator is applicable in lifted form:
				if (NullableType.IsNonNullableValueType(sourceType, context)
				    && NullableType.IsNonNullableValueType(targetType, context))
				{
					IType liftedSourceType = NullableType.Create(sourceType, context);
					IType liftedTargetType = NullableType.Create(targetType, context);
					if (isExplicit) {
						isApplicable = IsEncompassingOrEncompassedBy(fromType, liftedSourceType)
							&& IsEncompassingOrEncompassedBy(liftedTargetType, toType);
					} else {
						isApplicable = IsEncompassedBy(fromType, liftedSourceType) && IsEncompassedBy(liftedTargetType, toType);
					}
					if (isApplicable) {
						result.Add(new OperatorInfo(op, liftedSourceType, liftedTargetType, true));
					}
				}
			}
			return result;
		}
		#endregion
		
		#region AnonymousFunctionConversion
		Conversion AnonymousFunctionConversion(ResolveResult resolveResult, IType toType)
		{
			// C# 4.0 spec §6.5 Anonymous function conversions
			LambdaResolveResult f = resolveResult as LambdaResolveResult;
			if (f == null)
				return Conversion.None;
			if (!f.IsAnonymousMethod) {
				// It's a lambda, so conversions to expression trees exist
				// (even if the conversion leads to a compile-time error, e.g. for statement lambdas)
				toType = UnpackExpressionTreeType(toType);
			}
			IMethod d = toType.GetDelegateInvokeMethod();
			if (d == null)
				return Conversion.None;
			
			IType[] dParamTypes = new IType[d.Parameters.Count];
			for (int i = 0; i < dParamTypes.Length; i++) {
				dParamTypes[i] = d.Parameters[i].Type.Resolve(context);
			}
			IType dReturnType = d.ReturnType.Resolve(context);
			
			if (f.HasParameterList) {
				// If F contains an anonymous-function-signature, then D and F have the same number of parameters.
				if (d.Parameters.Count != f.Parameters.Count)
					return Conversion.None;
				
				if (f.IsImplicitlyTyped) {
					// If F has an implicitly typed parameter list, D has no ref or out parameters.
					foreach (IParameter p in d.Parameters) {
						if (p.IsOut || p.IsRef)
							return Conversion.None;
					}
				} else {
					// If F has an explicitly typed parameter list, each parameter in D has the same type
					// and modifiers as the corresponding parameter in F.
					for (int i = 0; i < f.Parameters.Count; i++) {
						IParameter pD = d.Parameters[i];
						IParameter pF = f.Parameters[i];
						if (pD.IsRef != pF.IsRef || pD.IsOut != pF.IsOut)
							return Conversion.None;
						if (!dParamTypes[i].Equals(pF.Type.Resolve(context)))
							return Conversion.None;
					}
				}
			} else {
				// If F does not contain an anonymous-function-signature, then D may have zero or more parameters of any
				// type, as long as no parameter of D has the out parameter modifier.
				foreach (IParameter p in d.Parameters) {
					if (p.IsOut)
						return Conversion.None;
				}
			}
			
			return f.IsValid(dParamTypes, dReturnType, this);
		}

		static IType UnpackExpressionTreeType(IType type)
		{
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null && pt.TypeParameterCount == 1 && pt.Name == "Expression" && pt.Namespace == "System.Linq.Expressions") {
				return pt.GetTypeArgument(0);
			} else {
				return type;
			}
		}
		#endregion
		
		#region MethodGroupConversion
		Conversion MethodGroupConversion(ResolveResult resolveResult, IType toType)
		{
			// C# 4.0 spec §6.6 Method group conversions
			MethodGroupResolveResult rr = resolveResult as MethodGroupResolveResult;
			if (rr == null)
				return Conversion.None;
			IMethod m = toType.GetDelegateInvokeMethod();
			if (m == null)
				return Conversion.None;
			
			ResolveResult[] args = new ResolveResult[m.Parameters.Count];
			for (int i = 0; i < args.Length; i++) {
				IParameter param = m.Parameters[i];
				IType parameterType = param.Type.Resolve(context);
				if ((param.IsRef || param.IsOut) && parameterType.Kind == TypeKind.ByReference) {
					parameterType = ((ByReferenceType)parameterType).ElementType;
					args[i] = new ByReferenceResolveResult(parameterType, param.IsOut);
				} else {
					args[i] = new ResolveResult(parameterType);
				}
			}
			var or = rr.PerformOverloadResolution(context, args, allowExpandingParams: false, conversions: this);
			if (or.FoundApplicableCandidate)
				return Conversion.MethodGroupConversion((IMethod)or.BestCandidate);
			else
				return Conversion.None;
		}
		#endregion
		
		#region BetterConversion
		/// <summary>
		/// Gets the better conversion (C# 4.0 spec, §7.5.3.3)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		public int BetterConversion(ResolveResult resolveResult, IType t1, IType t2)
		{
			LambdaResolveResult lambda = resolveResult as LambdaResolveResult;
			if (lambda != null) {
				if (!lambda.IsAnonymousMethod) {
					t1 = UnpackExpressionTreeType(t1);
					t2 = UnpackExpressionTreeType(t2);
				}
				IMethod m1 = t1.GetDelegateInvokeMethod();
				IMethod m2 = t2.GetDelegateInvokeMethod();
				if (m1 == null || m2 == null)
					return 0;
				int r = BetterConversionTarget(t1, t2);
				if (r != 0)
					return r;
				if (m1.Parameters.Count != m2.Parameters.Count)
					return 0;
				IType[] parameterTypes = new IType[m1.Parameters.Count];
				for (int i = 0; i < parameterTypes.Length; i++) {
					parameterTypes[i] = m1.Parameters[i].Type.Resolve(context);
					if (!parameterTypes[i].Equals(m2.Parameters[i].Type.Resolve(context)))
						return 0;
				}
				if (lambda.HasParameterList && parameterTypes.Length != lambda.Parameters.Count)
					return 0;
				
				IType ret1 = m1.ReturnType.Resolve(context);
				IType ret2 = m2.ReturnType.Resolve(context);
				if (ret1.Kind == TypeKind.Void && ret2.Kind != TypeKind.Void)
					return 1;
				if (ret1.Kind != TypeKind.Void && ret2.Kind == TypeKind.Void)
					return 2;
				
				IType inferredRet = lambda.GetInferredReturnType(parameterTypes);
				r = BetterConversion(inferredRet, ret1, ret2);
				if (r == 0 && lambda.IsAsync) {
					ret1 = UnpackTask(ret1);
					ret2 = UnpackTask(ret2);
					inferredRet = UnpackTask(inferredRet);
					if (ret1 != null && ret2 != null && inferredRet != null)
						r = BetterConversion(inferredRet, ret1, ret2);
				}
				return r;
			} else {
				return BetterConversion(resolveResult.Type, t1, t2);
			}
		}
		
		/// <summary>
		/// Unpacks the generic Task[T]. Returns null if the input is not Task[T].
		/// </summary>
		static IType UnpackTask(IType type)
		{
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null && pt.TypeParameterCount == 1 && pt.Name == "Task" && pt.Namespace == "System.Threading.Tasks") {
				return pt.GetTypeArgument(0);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the better conversion (C# 4.0 spec, §7.5.3.4)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		public int BetterConversion(IType s, IType t1, IType t2)
		{
			bool ident1 = IdentityConversion(s, t1);
			bool ident2 = IdentityConversion(s, t2);
			if (ident1 && !ident2)
				return 1;
			if (ident2 && !ident1)
				return 2;
			return BetterConversionTarget(t1, t2);
		}
		
		/// <summary>
		/// Gets the better conversion target (C# 4.0 spec, §7.5.3.5)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		int BetterConversionTarget(IType t1, IType t2)
		{
			bool t1To2 = ImplicitConversion(t1, t2);
			bool t2To1 = ImplicitConversion(t2, t1);
			if (t1To2 && !t2To1)
				return 1;
			if (t2To1 && !t1To2)
				return 2;
			TypeCode t1Code = ReflectionHelper.GetTypeCode(t1);
			TypeCode t2Code = ReflectionHelper.GetTypeCode(t2);
			if (IsBetterIntegralType(t1Code, t2Code))
				return 1;
			if (IsBetterIntegralType(t2Code, t1Code))
				return 2;
			return 0;
		}
		
		bool IsBetterIntegralType(TypeCode t1, TypeCode t2)
		{
			// signed types are better than unsigned types
			switch (t1) {
				case TypeCode.SByte:
					return t2 == TypeCode.Byte || t2 == TypeCode.UInt16 || t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int16:
					return t2 == TypeCode.UInt16 || t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int32:
					return t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int64:
					return t2 == TypeCode.UInt64;
				default:
					return false;
			}
		}
		#endregion
	}
}
