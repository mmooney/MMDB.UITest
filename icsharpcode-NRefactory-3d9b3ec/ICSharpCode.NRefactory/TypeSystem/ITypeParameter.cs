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
using System.Diagnostics.Contracts;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	#if WITH_CONTRACTS
	[ContractClass(typeof(ITypeParameterContract))]
	#endif
	public interface ITypeParameter : IType, IFreezable
	{
		/// <summary>
		/// Get the type of this type parameter's owner.
		/// </summary>
		/// <returns>EntityType.TypeDefinition or EntityType.Method</returns>
		EntityType OwnerType { get; }
		
		/// <summary>
		/// Gets the index of the type parameter in the type parameter list of the owning method/class.
		/// </summary>
		int Index { get; }
		
		/// <summary>
		/// Gets the list of attributes declared on this type parameter.
		/// </summary>
		IList<IAttribute> Attributes { get; }
		
		/// <summary>
		/// Gets the variance of this type parameter.
		/// </summary>
		VarianceModifier Variance { get; }
		
		/// <summary>
		/// Gets the region where the type parameter is defined.
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Gets the effective base class of this type parameter.
		/// </summary>
		IType GetEffectiveBaseClass(ITypeResolveContext context);
		
		/// <summary>
		/// Gets the effective interface set of this type parameter.
		/// </summary>
		IEnumerable<IType> GetEffectiveInterfaceSet(ITypeResolveContext context);
		
		/// <summary>
		/// Gets the constraints of this type parameter.
		/// </summary>
		ITypeParameterConstraints GetConstraints(ITypeResolveContext context);
	}
	
	/// <summary>
	/// Represents the constraints on a type parameter.
	/// </summary>
	public interface ITypeParameterConstraints : IList<IType>
	{
		/// <summary>
		/// Gets if the type parameter has the 'new()' constraint.
		/// </summary>
		bool HasDefaultConstructorConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'class' constraint.
		/// </summary>
		bool HasReferenceTypeConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'struct' constraint.
		/// </summary>
		bool HasValueTypeConstraint { get; }
		
		/// <summary>
		/// Applies a substitution to the constraints.
		/// </summary>
		ITypeParameterConstraints ApplySubstitution(TypeVisitor substitution);
	}
	
	/// <summary>
	/// Represents the variance of a type parameter.
	/// </summary>
	public enum VarianceModifier : byte
	{
		/// <summary>
		/// The type parameter is not variant.
		/// </summary>
		Invariant,
		/// <summary>
		/// The type parameter is covariant (used in output position).
		/// </summary>
		Covariant,
		/// <summary>
		/// The type parameter is contravariant (used in input position).
		/// </summary>
		Contravariant
	};
	
	#if WITH_CONTRACTS
	[ContractClassFor(typeof(ITypeParameter))]
	abstract class ITypeParameterContract : ITypeContract, ITypeParameter
	{
		int ITypeParameter.Index {
			get {
				Contract.Ensures(Contract.Result<int>() >= 0);
				return 0;
			}
		}
		
		IList<IAttribute> ITypeParameter.Attributes {
			get {
				Contract.Ensures(Contract.Result<IList<IAttribute>>() != null);
				return null;
			}
		}
		
		IList<ITypeReference> ITypeParameter.Constraints {
			get {
				Contract.Ensures(Contract.Result<IList<ITypeReference>>() != null);
				return null;
			}
		}
		
		bool ITypeParameter.HasDefaultConstructorConstraint {
			get { return false; }
		}
		
		bool ITypeParameter.HasReferenceTypeConstraint {
			get { return false; }
		}
		
		bool ITypeParameter.HasValueTypeConstraint {
			get { return false; }
		}
		
		IType ITypeParameter.BoundTo {
			get { return null; }
		}
		
		ITypeParameter ITypeParameter.UnboundTypeParameter {
			get {
				ITypeParameter @this = this;
				Contract.Ensures((Contract.Result<ITypeParameter>() != null) == (@this.BoundTo != null));
				return null;
			}
		}
		
		VarianceModifier ITypeParameter.Variance {
			get { return VarianceModifier.Invariant; }
		}
		
		bool IFreezable.IsFrozen {
			get { return false; }
		}
		
		void IFreezable.Freeze()
		{
		}
		
		EntityType ITypeParameter.OwnerType {
			get { return EntityType.None; }
		}
		
		DomRegion ITypeParameter.Region {
			get { return DomRegion.Empty; }
		}
		
		IType ITypeParameter.GetEffectiveBaseClass(ITypeResolveContext context)
		{
			Contract.Requires(context != null);
			Contract.Ensures(Contract.Result<IType>() != null);
		}
		
		IEnumerable<IType> ITypeParameter.GetEffectiveInterfaceSet(ITypeResolveContext context)
		{
			Contract.Requires(context != null);
			Contract.Ensures(Contract.Result<IEnumerable<IType>>() != null);
		}
	}
	#endif
}
