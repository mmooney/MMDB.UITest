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
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.Resolver.ConstantValues;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Produces type and member definitions from the DOM.
	/// </summary>
	public class TypeSystemConvertVisitor : DepthFirstAstVisitor<object, IEntity>
	{
		readonly CSharpParsedFile parsedFile;
		UsingScope usingScope;
		DefaultTypeDefinition currentTypeDefinition;
		DefaultMethod currentMethod;
		
		IInterningProvider interningProvider = new SimpleInterningProvider();
		
		/// <summary>
		/// Gets/Sets the interning provider to use.
		/// The default value is a new <see cref="SimpleInterningProvider"/> instance.
		/// </summary>
		public IInterningProvider InterningProvider {
			get { return interningProvider; }
			set { interningProvider = value; }
		}
		
		/// <summary>
		/// Creates a new TypeSystemConvertVisitor.
		/// </summary>
		/// <param name="pc">The parent project content (used as owner for the types being created).</param>
		/// <param name="fileName">The file name (used for DomRegions).</param>
		public TypeSystemConvertVisitor(IProjectContent pc, string fileName)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.parsedFile = new CSharpParsedFile(fileName, new UsingScope(pc));
			this.usingScope = parsedFile.RootUsingScope;
		}
		
		/// <summary>
		/// Creates a new TypeSystemConvertVisitor and initializes it with a given context.
		/// </summary>
		/// <param name="parsedFile">The parsed file to which members should be added.</param>
		/// <param name="currentUsingScope">The current using scope.</param>
		/// <param name="currentTypeDefinition">The current type definition.</param>
		public TypeSystemConvertVisitor(CSharpParsedFile parsedFile, UsingScope currentUsingScope = null, DefaultTypeDefinition currentTypeDefinition = null)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			this.parsedFile = parsedFile;
			this.usingScope = currentUsingScope ?? parsedFile.RootUsingScope;
			this.currentTypeDefinition = currentTypeDefinition;
		}
		
		public CSharpParsedFile Convert(AstNode node)
		{
			node.AcceptVisitor(this, null);
			return parsedFile;
		}
		
		public CSharpParsedFile ParsedFile {
			get { return parsedFile; }
		}
		
		DomRegion MakeRegion(TextLocation start, TextLocation end)
		{
			return new DomRegion(parsedFile.FileName, start.Line, start.Column, end.Line, end.Column);
		}
		
		DomRegion MakeRegion(AstNode node)
		{
			if (node == null || node.IsNull)
				return DomRegion.Empty;
			else
				return MakeRegion(node.StartLocation, node.EndLocation);
		}
		
		DomRegion MakeBraceRegion(AstNode node)
		{
			if (node == null || node.IsNull)
				return DomRegion.Empty;
			else
				return MakeRegion(node.GetChildByRole(AstNode.Roles.LBrace).StartLocation,
				                  node.GetChildByRole(AstNode.Roles.RBrace).EndLocation);
		}
		
		#region Compilation Unit
		public override IEntity VisitCompilationUnit (CompilationUnit unit, object data)
		{
			parsedFile.Errors = unit.Errors;
			return base.VisitCompilationUnit (unit, data);
		}
		#endregion
		
		#region Using Declarations
		public override IEntity VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration, object data)
		{
			usingScope.ExternAliases.Add(externAliasDeclaration.Name);
			return null;
		}
		
		public override IEntity VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			ITypeOrNamespaceReference u = ConvertType(usingDeclaration.Import, SimpleNameLookupMode.TypeInUsingDeclaration) as ITypeOrNamespaceReference;
			if (u != null) {
				if (interningProvider != null)
					u = interningProvider.Intern(u);
				usingScope.Usings.Add(u);
			}
			return null;
		}
		
		public override IEntity VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration, object data)
		{
			ITypeOrNamespaceReference u = ConvertType(usingDeclaration.Import, SimpleNameLookupMode.TypeInUsingDeclaration) as ITypeOrNamespaceReference;
			if (u != null) {
				if (interningProvider != null)
					u = interningProvider.Intern(u);
				usingScope.UsingAliases.Add(new KeyValuePair<string, ITypeOrNamespaceReference>(usingDeclaration.Alias, u));
			}
			return null;
		}
		#endregion
		
		#region Namespace Declaration
		public override IEntity VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			DomRegion region = MakeRegion(namespaceDeclaration);
			UsingScope previousUsingScope = usingScope;
			foreach (Identifier ident in namespaceDeclaration.Identifiers) {
				usingScope = new UsingScope(usingScope, NamespaceDeclaration.BuildQualifiedName(usingScope.NamespaceName, ident.Name));
				usingScope.Region = region;
			}
			base.VisitNamespaceDeclaration(namespaceDeclaration, data);
			parsedFile.UsingScopes.Add(usingScope); // add after visiting children so that nested scopes come first
			usingScope = previousUsingScope;
			return null;
		}
		#endregion
		
		#region Type Definitions
		DefaultTypeDefinition CreateTypeDefinition(string name)
		{
			DefaultTypeDefinition newType;
			if (currentTypeDefinition != null) {
				newType = new DefaultTypeDefinition(currentTypeDefinition, name);
				foreach (var typeParameter in currentTypeDefinition.TypeParameters)
					newType.TypeParameters.Add(typeParameter);
				currentTypeDefinition.NestedTypes.Add(newType);
			} else {
				newType = new DefaultTypeDefinition(parsedFile, usingScope.NamespaceName, name);
				parsedFile.TopLevelTypeDefinitions.Add(newType);
			}
			return newType;
		}
		
		public override IEntity VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			var td = currentTypeDefinition = CreateTypeDefinition(typeDeclaration.Name);
			td.Region = MakeRegion(typeDeclaration);
			td.BodyRegion = MakeBraceRegion(typeDeclaration);
			td.AddDefaultConstructorIfRequired = true;
			
			ApplyModifiers(td, typeDeclaration.Modifiers);
			switch (typeDeclaration.ClassType) {
				case ClassType.Enum:
					td.Kind = TypeKind.Enum;
					break;
				case ClassType.Interface:
					td.Kind = TypeKind.Interface;
					td.IsAbstract = true; // interfaces are implicitly abstract
					break;
				case ClassType.Struct:
					td.Kind = TypeKind.Struct;
					td.IsSealed = true; // enums/structs are implicitly sealed
					break;
			}
			
			ConvertAttributes(td.Attributes, typeDeclaration.Attributes);
			
			ConvertTypeParameters(td.TypeParameters, typeDeclaration.TypeParameters, typeDeclaration.Constraints, EntityType.TypeDefinition);
			
			foreach (AstType baseType in typeDeclaration.BaseTypes) {
				td.BaseTypes.Add(ConvertType(baseType, SimpleNameLookupMode.BaseTypeReference));
			}
			
			foreach (AttributedNode member in typeDeclaration.Members) {
				member.AcceptVisitor(this, data);
			}
			
			td.HasExtensionMethods = td.Methods.Any(m => m.IsExtensionMethod);
			
			currentTypeDefinition = (DefaultTypeDefinition)currentTypeDefinition.DeclaringTypeDefinition;
			if (interningProvider != null) {
				td.ApplyInterningProvider(interningProvider);
			}
			return td;
		}
		
		public override IEntity VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			var td = currentTypeDefinition = CreateTypeDefinition(delegateDeclaration.Name);
			td.Kind = TypeKind.Delegate;
			td.Region = MakeRegion(delegateDeclaration);
			td.BaseTypes.Add(multicastDelegateReference);
			
			ApplyModifiers(td, delegateDeclaration.Modifiers);
			td.IsSealed = true; // delegates are implicitly sealed
			
			ConvertTypeParameters(td.TypeParameters, delegateDeclaration.TypeParameters, delegateDeclaration.Constraints, EntityType.TypeDefinition);
			
			ITypeReference returnType = ConvertType(delegateDeclaration.ReturnType);
			List<IParameter> parameters = new List<IParameter>();
			ConvertParameters(parameters, delegateDeclaration.Parameters);
			AddDefaultMethodsToDelegate(td, returnType, parameters);
			
			foreach (AttributeSection section in delegateDeclaration.Attributes) {
				if (section.AttributeTarget == "return") {
					ConvertAttributes(td.Methods.Single(m => m.Name == "Invoke").ReturnTypeAttributes, section);
					ConvertAttributes(td.Methods.Single(m => m.Name == "EndInvoke").ReturnTypeAttributes, section);
				} else {
					ConvertAttributes(td.Attributes, section);
				}
			}
			
			currentTypeDefinition = (DefaultTypeDefinition)currentTypeDefinition.DeclaringTypeDefinition;
			if (interningProvider != null) {
				td.ApplyInterningProvider(interningProvider);
			}
			return td;
		}
		
		static readonly ITypeReference multicastDelegateReference = typeof(MulticastDelegate).ToTypeReference();
		static readonly IParameter delegateObjectParameter = MakeParameter(KnownTypeReference.Object, "object");
		static readonly IParameter delegateIntPtrMethodParameter = MakeParameter(typeof(IntPtr).ToTypeReference(), "method");
		static readonly IParameter delegateAsyncCallbackParameter = MakeParameter(typeof(AsyncCallback).ToTypeReference(), "callback");
		static readonly IParameter delegateResultParameter = MakeParameter(typeof(IAsyncResult).ToTypeReference(), "result");
		
		static IParameter MakeParameter(ITypeReference type, string name)
		{
			DefaultParameter p = new DefaultParameter(type, name);
			p.Freeze();
			return p;
		}
		
		/// <summary>
		/// Adds the 'Invoke', 'BeginInvoke', 'EndInvoke' methods, and a constructor, to the <paramref name="delegateType"/>.
		/// </summary>
		public static void AddDefaultMethodsToDelegate(DefaultTypeDefinition delegateType, ITypeReference returnType, IEnumerable<IParameter> parameters)
		{
			if (delegateType == null)
				throw new ArgumentNullException("delegateType");
			if (returnType == null)
				throw new ArgumentNullException("returnType");
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			
			DomRegion region = new DomRegion(delegateType.Region.FileName, delegateType.Region.BeginLine, delegateType.Region.BeginColumn);
			
			DefaultMethod invoke = new DefaultMethod(delegateType, "Invoke");
			invoke.Accessibility = Accessibility.Public;
			invoke.IsSynthetic = true;
			foreach (var p in parameters)
				invoke.Parameters.Add(p);
			invoke.ReturnType = returnType;
			invoke.Region = region;
			delegateType.Methods.Add(invoke);
			
			DefaultMethod beginInvoke = new DefaultMethod(delegateType, "BeginInvoke");
			beginInvoke.Accessibility = Accessibility.Public;
			beginInvoke.IsSynthetic = true;
			foreach (var p in parameters)
				beginInvoke.Parameters.Add(p);
			beginInvoke.Parameters.Add(delegateAsyncCallbackParameter);
			beginInvoke.Parameters.Add(delegateObjectParameter);
			beginInvoke.ReturnType = delegateResultParameter.Type;
			beginInvoke.Region = region;
			delegateType.Methods.Add(beginInvoke);
			
			DefaultMethod endInvoke = new DefaultMethod(delegateType, "EndInvoke");
			endInvoke.Accessibility = Accessibility.Public;
			endInvoke.IsSynthetic = true;
			endInvoke.Parameters.Add(delegateResultParameter);
			endInvoke.ReturnType = invoke.ReturnType;
			endInvoke.Region = region;
			delegateType.Methods.Add(endInvoke);
			
			DefaultMethod ctor = new DefaultMethod(delegateType, ".ctor");
			ctor.EntityType = EntityType.Constructor;
			ctor.Accessibility = Accessibility.Public;
			ctor.IsSynthetic = true;
			ctor.Parameters.Add(delegateObjectParameter);
			ctor.Parameters.Add(delegateIntPtrMethodParameter);
			ctor.ReturnType = delegateType;
			ctor.Region = region;
			delegateType.Methods.Add(ctor);
		}
		#endregion
		
		#region Fields
		public override IEntity VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			bool isSingleField = fieldDeclaration.Variables.Count == 1;
			Modifiers modifiers = fieldDeclaration.Modifiers;
			DefaultField field = null;
			foreach (VariableInitializer vi in fieldDeclaration.Variables) {
				field = new DefaultField(currentTypeDefinition, vi.Name);
				
				field.Region = isSingleField ? MakeRegion(fieldDeclaration) : MakeRegion(vi);
				field.BodyRegion = MakeRegion(vi);
				ConvertAttributes(field.Attributes, fieldDeclaration.Attributes);
				
				ApplyModifiers(field, modifiers);
				field.IsVolatile = (modifiers & Modifiers.Volatile) != 0;
				field.IsReadOnly = (modifiers & Modifiers.Readonly) != 0;
				
				field.ReturnType = ConvertType(fieldDeclaration.ReturnType);
				
				if ((modifiers & Modifiers.Const) != 0) {
					field.ConstantValue = ConvertConstantValue(field.ReturnType, vi.Initializer);
				}
				
				currentTypeDefinition.Fields.Add(field);
				if (interningProvider != null) {
					field.ApplyInterningProvider(interningProvider);
				}
			}
			return isSingleField ? field : null;
		}
		
		public override IEntity VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration, object data)
		{
			// TODO: add support for fixed fields
			return base.VisitFixedFieldDeclaration(fixedFieldDeclaration, data);
		}
		
		public override IEntity VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration, object data)
		{
			DefaultField field = new DefaultField(currentTypeDefinition, enumMemberDeclaration.Name);
			field.Region = field.BodyRegion = MakeRegion(enumMemberDeclaration);
			ConvertAttributes(field.Attributes, enumMemberDeclaration.Attributes);
			
			field.ReturnType = currentTypeDefinition;
			field.Accessibility = Accessibility.Public;
			field.IsStatic = true;
			if (!enumMemberDeclaration.Initializer.IsNull) {
				field.ConstantValue = ConvertConstantValue(currentTypeDefinition, enumMemberDeclaration.Initializer);
			} else {
				IField prevField = currentTypeDefinition.Fields.LastOrDefault();
				if (prevField == null || prevField.ConstantValue == null) {
					field.ConstantValue = ConvertConstantValue(currentTypeDefinition, new PrimitiveExpression(0));
				} else {
					field.ConstantValue = new IncrementConstantValue(prevField.ConstantValue);
				}
			}
			
			currentTypeDefinition.Fields.Add(field);
			if (interningProvider != null) {
				field.ApplyInterningProvider(interningProvider);
			}
			return field;
		}
		#endregion
		
		#region Methods
		public override IEntity VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			DefaultMethod m = new DefaultMethod(currentTypeDefinition, methodDeclaration.Name);
			currentMethod = m; // required for resolving type parameters
			m.Region = MakeRegion(methodDeclaration);
			m.BodyRegion = MakeRegion(methodDeclaration.Body);
			
			if (InheritsConstraints(methodDeclaration) && methodDeclaration.Constraints.Count == 0) {
				int index = 0;
				foreach (TypeParameterDeclaration tpDecl in methodDeclaration.TypeParameters) {
					var tp = new TypeParameterWithInheritedConstraints(m, index++, tpDecl.Name);
					tp.Region = MakeRegion(tpDecl);
					ConvertAttributes(tp.Attributes, tpDecl.Attributes);
					tp.Variance = tpDecl.Variance;
					m.TypeParameters.Add(tp);
				}
			} else {
				ConvertTypeParameters(m.TypeParameters, methodDeclaration.TypeParameters, methodDeclaration.Constraints, EntityType.Method);
			}
			m.ReturnType = ConvertType(methodDeclaration.ReturnType);
			ConvertAttributes(m.Attributes, methodDeclaration.Attributes.Where(s => s.AttributeTarget != "return"));
			ConvertAttributes(m.ReturnTypeAttributes, methodDeclaration.Attributes.Where(s => s.AttributeTarget == "return"));
			
			ApplyModifiers(m, methodDeclaration.Modifiers);
			m.IsExtensionMethod = methodDeclaration.IsExtensionMethod;
			
			ConvertParameters(m.Parameters, methodDeclaration.Parameters);
			if (!methodDeclaration.PrivateImplementationType.IsNull) {
				m.Accessibility = Accessibility.None;
				m.InterfaceImplementations.Add(ConvertInterfaceImplementation(methodDeclaration.PrivateImplementationType, m.Name));
			}
			
			currentTypeDefinition.Methods.Add(m);
			currentMethod = null;
			if (interningProvider != null) {
				m.ApplyInterningProvider(interningProvider);
			}
			return m;
		}
		
		bool InheritsConstraints(MethodDeclaration methodDeclaration)
		{
			// overrides and explicit interface implementations inherit constraints
			if ((methodDeclaration.Modifiers & Modifiers.Override) == Modifiers.Override)
				return true;
			return !methodDeclaration.PrivateImplementationType.IsNull;
		}
		
		void ConvertTypeParameters(IList<ITypeParameter> output, AstNodeCollection<TypeParameterDeclaration> typeParameters, AstNodeCollection<Constraint> constraints, EntityType ownerType)
		{
			// output might be non-empty when type parameters were copied from an outer class
			int index = output.Count;
			List<DefaultTypeParameter> list = new List<DefaultTypeParameter>();
			foreach (TypeParameterDeclaration tpDecl in typeParameters) {
				DefaultTypeParameter tp = new DefaultTypeParameter(ownerType, index++, tpDecl.Name);
				tp.Region = MakeRegion(tpDecl);
				ConvertAttributes(tp.Attributes, tpDecl.Attributes);
				tp.Variance = tpDecl.Variance;
				list.Add(tp);
				output.Add(tp); // tp must be added to list here so that it can be referenced by constraints
			}
			foreach (Constraint c in constraints) {
				foreach (var tp in list) {
					if (tp.Name == c.TypeParameter) {
						foreach (AstType type in c.BaseTypes) {
							PrimitiveType primType = type as PrimitiveType;
							if (primType != null) {
								if (primType.Keyword == "new") {
									tp.HasDefaultConstructorConstraint = true;
									continue;
								} else if (primType.Keyword == "class") {
									tp.HasReferenceTypeConstraint = true;
									continue;
								} else if (primType.Keyword == "struct") {
									tp.HasValueTypeConstraint = true;
									continue;
								}
							}
							tp.Constraints.Add(ConvertType(type));
						}
						break;
					}
				}
			}
		}
		
		DefaultExplicitInterfaceImplementation ConvertInterfaceImplementation(AstType interfaceType, string memberName)
		{
			return new DefaultExplicitInterfaceImplementation(ConvertType(interfaceType), memberName);
		}
		#endregion
		
		#region Operators
		public override IEntity VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			DefaultMethod m = new DefaultMethod(currentTypeDefinition, operatorDeclaration.Name);
			m.EntityType = EntityType.Operator;
			m.Region = MakeRegion(operatorDeclaration);
			m.BodyRegion = MakeRegion(operatorDeclaration.Body);
			
			m.ReturnType = ConvertType(operatorDeclaration.ReturnType);
			ConvertAttributes(m.Attributes, operatorDeclaration.Attributes.Where(s => s.AttributeTarget != "return"));
			ConvertAttributes(m.ReturnTypeAttributes, operatorDeclaration.Attributes.Where(s => s.AttributeTarget == "return"));
			
			ApplyModifiers(m, operatorDeclaration.Modifiers);
			
			ConvertParameters(m.Parameters, operatorDeclaration.Parameters);
			
			currentTypeDefinition.Methods.Add(m);
			if (interningProvider != null) {
				m.ApplyInterningProvider(interningProvider);
			}
			return m;
		}
		#endregion
		
		#region Constructors
		public override IEntity VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			Modifiers modifiers = constructorDeclaration.Modifiers;
			bool isStatic = (modifiers & Modifiers.Static) != 0;
			DefaultMethod ctor = new DefaultMethod(currentTypeDefinition, isStatic ? ".cctor" : ".ctor");
			ctor.EntityType = EntityType.Constructor;
			ctor.Region = MakeRegion(constructorDeclaration);
			if (!constructorDeclaration.Initializer.IsNull) {
				ctor.BodyRegion = MakeRegion(constructorDeclaration.Initializer.StartLocation, constructorDeclaration.EndLocation);
			} else {
				ctor.BodyRegion = MakeRegion(constructorDeclaration.Body);
			}
			ctor.ReturnType = currentTypeDefinition;
			
			ConvertAttributes(ctor.Attributes, constructorDeclaration.Attributes);
			ConvertParameters(ctor.Parameters, constructorDeclaration.Parameters);
			
			if (isStatic)
				ctor.IsStatic = true;
			else
				ApplyModifiers(ctor, modifiers);
			
			currentTypeDefinition.Methods.Add(ctor);
			if (interningProvider != null) {
				ctor.ApplyInterningProvider(interningProvider);
			}
			return ctor;
		}
		#endregion
		
		#region Destructors
		public override IEntity VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			DefaultMethod dtor = new DefaultMethod(currentTypeDefinition, "Finalize");
			dtor.EntityType = EntityType.Destructor;
			dtor.Region = MakeRegion(destructorDeclaration);
			dtor.BodyRegion = MakeRegion(destructorDeclaration.Body);
			dtor.Accessibility = Accessibility.Protected;
			dtor.IsOverride = true;
			dtor.ReturnType = KnownTypeReference.Void;
			
			ConvertAttributes(dtor.Attributes, destructorDeclaration.Attributes);
			
			currentTypeDefinition.Methods.Add(dtor);
			if (interningProvider != null) {
				dtor.ApplyInterningProvider(interningProvider);
			}
			return dtor;
		}
		#endregion
		
		#region Properties / Indexers
		public override IEntity VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			DefaultProperty p = new DefaultProperty(currentTypeDefinition, propertyDeclaration.Name);
			p.Region = MakeRegion(propertyDeclaration);
			p.BodyRegion = MakeBraceRegion(propertyDeclaration);
			ApplyModifiers(p, propertyDeclaration.Modifiers);
			p.ReturnType = ConvertType(propertyDeclaration.ReturnType);
			ConvertAttributes(p.Attributes, propertyDeclaration.Attributes);
			if (!propertyDeclaration.PrivateImplementationType.IsNull) {
				p.Accessibility = Accessibility.None;
				p.InterfaceImplementations.Add(ConvertInterfaceImplementation(propertyDeclaration.PrivateImplementationType, p.Name));
			}
			p.Getter = ConvertAccessor(propertyDeclaration.Getter, p.Accessibility);
			p.Setter = ConvertAccessor(propertyDeclaration.Setter, p.Accessibility);
			currentTypeDefinition.Properties.Add(p);
			if (interningProvider != null) {
				p.ApplyInterningProvider(interningProvider);
			}
			return p;
		}
		
		public override IEntity VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			DefaultProperty p = new DefaultProperty(currentTypeDefinition, "Item");
			p.EntityType = EntityType.Indexer;
			p.Region = MakeRegion(indexerDeclaration);
			p.BodyRegion = MakeBraceRegion(indexerDeclaration);
			ApplyModifiers(p, indexerDeclaration.Modifiers);
			p.ReturnType = ConvertType(indexerDeclaration.ReturnType);
			ConvertAttributes(p.Attributes, indexerDeclaration.Attributes);
			if (!indexerDeclaration.PrivateImplementationType.IsNull) {
				p.Accessibility = Accessibility.None;
				p.InterfaceImplementations.Add(ConvertInterfaceImplementation(indexerDeclaration.PrivateImplementationType, p.Name));
			}
			p.Getter = ConvertAccessor(indexerDeclaration.Getter, p.Accessibility);
			p.Setter = ConvertAccessor(indexerDeclaration.Setter, p.Accessibility);
			ConvertParameters(p.Parameters, indexerDeclaration.Parameters);
			currentTypeDefinition.Properties.Add(p);
			if (interningProvider != null) {
				p.ApplyInterningProvider(interningProvider);
			}
			return p;
		}
		
		IAccessor ConvertAccessor(Accessor accessor, Accessibility defaultAccessibility)
		{
			if (accessor.IsNull)
				return null;
			DefaultAccessor a = new DefaultAccessor();
			a.Accessibility = GetAccessibility(accessor.Modifiers) ?? defaultAccessibility;
			a.Region = MakeRegion(accessor);
			foreach (AttributeSection section in accessor.Attributes) {
				if (section.AttributeTarget == "return") {
					ConvertAttributes(a.ReturnTypeAttributes, section);
				} else if (section.AttributeTarget != "param") {
					ConvertAttributes(a.Attributes, section);
				}
			}
			return a;
		}
		#endregion
		
		#region Events
		public override IEntity VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			bool isSingleEvent = eventDeclaration.Variables.Count == 1;
			Modifiers modifiers = eventDeclaration.Modifiers;
			DefaultEvent ev = null;
			foreach (VariableInitializer vi in eventDeclaration.Variables) {
				ev = new DefaultEvent(currentTypeDefinition, vi.Name);
				
				ev.Region = isSingleEvent ? MakeRegion(eventDeclaration) : MakeRegion(vi);
				ev.BodyRegion = MakeRegion(vi);
				
				ApplyModifiers(ev, modifiers);
				
				ev.ReturnType = ConvertType(eventDeclaration.ReturnType);
				
				if (eventDeclaration.Attributes.Any(a => a.AttributeTarget == "method")) {
					ev.AddAccessor = ev.RemoveAccessor = new DefaultAccessor { Accessibility = ev.Accessibility };
				} else {
					// if there's no attributes on the accessors, we can re-use the shared accessor instance
					ev.AddAccessor = ev.RemoveAccessor = DefaultAccessor.GetFromAccessibility(ev.Accessibility);
				}
				foreach (AttributeSection section in eventDeclaration.Attributes) {
					if (section.AttributeTarget == "method") {
						// as we use the same instance for AddAccessor and RemoveAccessor, we only need to add the attribute once
						ConvertAttributes(ev.AddAccessor.Attributes, section);
					} else if (section.AttributeTarget != "field") {
						ConvertAttributes(ev.Attributes, section);
					}
				}
				
				currentTypeDefinition.Events.Add(ev);
				if (interningProvider != null) {
					ev.ApplyInterningProvider(interningProvider);
				}
			}
			return isSingleEvent ? ev : null;
		}
		
		public override IEntity VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration, object data)
		{
			DefaultEvent e = new DefaultEvent(currentTypeDefinition, eventDeclaration.Name);
			e.Region = MakeRegion(eventDeclaration);
			e.BodyRegion = MakeBraceRegion(eventDeclaration);
			ApplyModifiers(e, eventDeclaration.Modifiers);
			e.ReturnType = ConvertType(eventDeclaration.ReturnType);
			ConvertAttributes(e.Attributes, eventDeclaration.Attributes);
			
			if (!eventDeclaration.PrivateImplementationType.IsNull) {
				e.Accessibility = Accessibility.None;
				e.InterfaceImplementations.Add(ConvertInterfaceImplementation(eventDeclaration.PrivateImplementationType, e.Name));
			}
			
			e.AddAccessor = ConvertAccessor(eventDeclaration.AddAccessor, e.Accessibility);
			e.RemoveAccessor = ConvertAccessor(eventDeclaration.RemoveAccessor, e.Accessibility);
			
			currentTypeDefinition.Events.Add(e);
			if (interningProvider != null) {
				e.ApplyInterningProvider(interningProvider);
			}
			return e;
		}
		#endregion
		
		#region Modifiers
		static void ApplyModifiers(DefaultTypeDefinition td, Modifiers modifiers)
		{
			td.Accessibility = GetAccessibility(modifiers) ?? (td.DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
			td.IsAbstract = (modifiers & (Modifiers.Abstract | Modifiers.Static)) != 0;
			td.IsSealed = (modifiers & (Modifiers.Sealed | Modifiers.Static)) != 0;
			td.IsShadowing = (modifiers & Modifiers.New) != 0;
		}
		
		static void ApplyModifiers(TypeSystem.Implementation.AbstractMember m, Modifiers modifiers)
		{
			// members from interfaces are always Public+Abstract.
			if (m.DeclaringTypeDefinition.Kind == TypeKind.Interface) {
				m.Accessibility = Accessibility.Public;
				m.IsAbstract = true;
				return;
			}
			m.Accessibility = GetAccessibility(modifiers) ?? Accessibility.Private;
			m.IsAbstract = (modifiers & Modifiers.Abstract) != 0;
			m.IsOverride = (modifiers & Modifiers.Override) != 0;
			m.IsSealed = (modifiers & Modifiers.Sealed) != 0;
			m.IsShadowing = (modifiers & Modifiers.New) != 0;
			m.IsStatic = (modifiers & Modifiers.Static) != 0;
			m.IsVirtual = (modifiers & Modifiers.Virtual) != 0;
			m.IsPartial = (modifiers & Modifiers.Partial) != 0;
			
			
		}
		
		static Accessibility? GetAccessibility(Modifiers modifiers)
		{
			switch (modifiers & Modifiers.VisibilityMask) {
				case Modifiers.Private:
					return Accessibility.Private;
				case Modifiers.Internal:
					return Accessibility.Internal;
				case Modifiers.Protected | Modifiers.Internal:
					return Accessibility.ProtectedOrInternal;
				case Modifiers.Protected:
					return Accessibility.Protected;
				case Modifiers.Public:
					return Accessibility.Public;
				default:
					return null;
			}
		}
		#endregion
		
		#region Attributes
		public override IEntity VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			// non-assembly attributes are handled by their parent entity
			if (attributeSection.AttributeTarget == "assembly") {
				ConvertAttributes(parsedFile.AssemblyAttributes, attributeSection);
			} else if (attributeSection.AttributeTarget == "module") {
				ConvertAttributes(parsedFile.ModuleAttributes, attributeSection);
			}
			return null;
		}
		
		void ConvertAttributes(IList<IAttribute> outputList, IEnumerable<AttributeSection> attributes)
		{
			foreach (AttributeSection section in attributes) {
				ConvertAttributes(outputList, section);
			}
		}
		
		void ConvertAttributes(IList<IAttribute> outputList, AttributeSection attributeSection)
		{
			foreach (CSharp.Attribute attr in attributeSection.Attributes) {
				outputList.Add(ConvertAttribute(attr));
			}
		}
		
		internal static ITypeReference ConvertAttributeType(AstType type, ITypeDefinition parentTypeDefinition, IMethod parentMethodDefinition, UsingScope parentUsingScope)
		{
			ITypeReference tr = ConvertType(type, parentTypeDefinition, parentMethodDefinition, parentUsingScope, SimpleNameLookupMode.Type);
			if (!type.GetChildByRole(AstNode.Roles.Identifier).IsVerbatim) {
				// Try to add "Attribute" suffix, but only if the identifier
				// (=last identifier in fully qualified name) isn't a verbatim identifier.
				SimpleTypeOrNamespaceReference st = tr as SimpleTypeOrNamespaceReference;
				MemberTypeOrNamespaceReference mt = tr as MemberTypeOrNamespaceReference;
				if (st != null)
					return new AttributeTypeReference(st, st.AddSuffix("Attribute"));
				else if (mt != null)
					return new AttributeTypeReference(mt, mt.AddSuffix("Attribute"));
			}
			return tr;
		}
		
		IAttribute ConvertAttribute(CSharp.Attribute attr)
		{
			DomRegion region = MakeRegion(attr);
			ITypeReference type = ConvertAttributeType(attr.Type, currentTypeDefinition, currentMethod, usingScope);
			List<IConstantValue> positionalArguments = null;
			List<KeyValuePair<string, IConstantValue>> namedCtorArguments = null;
			List<KeyValuePair<string, IConstantValue>> namedArguments = null;
			foreach (Expression expr in attr.Arguments) {
				NamedArgumentExpression nae = expr as NamedArgumentExpression;
				if (nae != null) {
					if (namedCtorArguments == null)
						namedCtorArguments = new List<KeyValuePair<string, IConstantValue>>();
					namedCtorArguments.Add(new KeyValuePair<string, IConstantValue>(nae.Identifier, ConvertAttributeArgument(nae.Expression)));
				} else {
					NamedExpression namedExpression = expr as NamedExpression;
					if (namedExpression != null) {
						string name = namedExpression.Identifier;
						if (namedArguments == null)
							namedArguments = new List<KeyValuePair<string, IConstantValue>>();
						namedArguments.Add(new KeyValuePair<string, IConstantValue>(name, ConvertAttributeArgument(namedExpression.Expression)));
					} else {
						if (positionalArguments == null)
							positionalArguments = new List<IConstantValue>();
						positionalArguments.Add(ConvertAttributeArgument(expr));
					}
				}
			}
			return new CSharpAttribute(type, region, positionalArguments, namedCtorArguments, namedArguments);
		}
		#endregion
		
		#region Types
		ITypeReference ConvertType(AstType type, SimpleNameLookupMode lookupMode = SimpleNameLookupMode.Type)
		{
			return ConvertType(type, currentTypeDefinition, currentMethod, usingScope, lookupMode);
		}
		
		internal static ITypeReference ConvertType(AstType type, ITypeDefinition parentTypeDefinition, IMethod parentMethodDefinition, UsingScope parentUsingScope, SimpleNameLookupMode lookupMode)
		{
			SimpleType s = type as SimpleType;
			if (s != null) {
				List<ITypeReference> typeArguments = new List<ITypeReference>();
				foreach (var ta in s.TypeArguments) {
					typeArguments.Add(ConvertType(ta, parentTypeDefinition, parentMethodDefinition, parentUsingScope, lookupMode));
				}
				if (typeArguments.Count == 0 && parentMethodDefinition != null) {
					// SimpleTypeOrNamespaceReference doesn't have a 'current method' context
					// so we have to handle method type parameters here.
					foreach (ITypeParameter tp in parentMethodDefinition.TypeParameters) {
						if (tp.Name == s.Identifier)
							return tp;
					}
				}
				if (typeArguments.Count == 0 && string.IsNullOrEmpty(s.Identifier)) {
					// empty SimpleType is used for typeof(List<>).
					return SharedTypes.UnboundTypeArgument;
				}
				return new SimpleTypeOrNamespaceReference(s.Identifier, typeArguments, parentTypeDefinition, parentUsingScope, lookupMode);
			}
			
			PrimitiveType p = type as PrimitiveType;
			if (p != null) {
				switch (p.Keyword) {
					case "string":
						return KnownTypeReference.String;
					case "int":
						return KnownTypeReference.Int32;
					case "uint":
						return KnownTypeReference.UInt32;
					case "object":
						return KnownTypeReference.Object;
					case "bool":
						return KnownTypeReference.Boolean;
					case "sbyte":
						return KnownTypeReference.SByte;
					case "byte":
						return KnownTypeReference.Byte;
					case "short":
						return KnownTypeReference.Int16;
					case "ushort":
						return KnownTypeReference.UInt16;
					case "long":
						return KnownTypeReference.Int64;
					case "ulong":
						return KnownTypeReference.UInt64;
					case "float":
						return KnownTypeReference.Single;
					case "double":
						return KnownTypeReference.Double;
					case "decimal":
						return KnownTypeReference.Decimal;
					case "char":
						return KnownTypeReference.Char;
					case "void":
						return KnownTypeReference.Void;
					default:
						return SharedTypes.UnknownType;
				}
			}
			MemberType m = type as MemberType;
			if (m != null) {
				ITypeOrNamespaceReference t;
				if (m.IsDoubleColon) {
					SimpleType st = m.Target as SimpleType;
					if (st != null) {
						t = new AliasNamespaceReference(st.Identifier, parentUsingScope);
					} else {
						t = null;
					}
				} else {
					t = ConvertType(m.Target, parentTypeDefinition, parentMethodDefinition, parentUsingScope, lookupMode) as ITypeOrNamespaceReference;
				}
				if (t == null)
					return SharedTypes.UnknownType;
				List<ITypeReference> typeArguments = new List<ITypeReference>();
				foreach (var ta in m.TypeArguments) {
					typeArguments.Add(ConvertType(ta, parentTypeDefinition, parentMethodDefinition, parentUsingScope, lookupMode));
				}
				return new MemberTypeOrNamespaceReference(t, m.MemberName, typeArguments, parentTypeDefinition, parentUsingScope);
			}
			ComposedType c = type as ComposedType;
			if (c != null) {
				ITypeReference t = ConvertType(c.BaseType, parentTypeDefinition, parentMethodDefinition, parentUsingScope, lookupMode);
				if (c.HasNullableSpecifier) {
					t = NullableType.Create(t);
				}
				for (int i = 0; i < c.PointerRank; i++) {
					t = PointerTypeReference.Create(t);
				}
				foreach (var a in c.ArraySpecifiers.Reverse()) {
					t = ArrayTypeReference.Create(t, a.Dimensions);
				}
				return t;
			}
			if (!type.IsNull)
				Debug.WriteLine("Unknown node used as type: " + type);
			return SharedTypes.UnknownType;
		}
		#endregion
		
		#region Constant Values
		IConstantValue ConvertConstantValue(ITypeReference targetType, AstNode expression)
		{
			return ConvertConstantValue(targetType, expression, currentTypeDefinition, currentMethod, usingScope);
		}
		
		internal static IConstantValue ConvertConstantValue(ITypeReference targetType, AstNode expression,
		                                                    ITypeDefinition parentTypeDefinition, IMethod parentMethodDefinition, UsingScope parentUsingScope)
		{
			ConstantValueBuilder b = new ConstantValueBuilder(parentTypeDefinition, parentMethodDefinition, parentUsingScope, false);
			ConstantExpression c = expression.AcceptVisitor(b, null);
			if (c == null)
				return new ErrorConstantValue(targetType);
			PrimitiveConstantExpression pc = c as PrimitiveConstantExpression;
			if (pc != null && pc.Type == targetType) {
				// Save memory by directly using a SimpleConstantValue.
				return new SimpleConstantValue(targetType, pc.Value);
			}
			// cast to the desired type
			return new CSharpConstantValue(new ConstantCast(targetType, c), parentUsingScope, parentTypeDefinition);
		}
		
		IConstantValue ConvertAttributeArgument(Expression expression)
		{
			ConstantValueBuilder b = new ConstantValueBuilder(currentTypeDefinition, currentMethod, usingScope, true);
			ConstantExpression c = expression.AcceptVisitor(b, null);
			if (c == null)
				return null;
			PrimitiveConstantExpression pc = c as PrimitiveConstantExpression;
			if (pc != null) {
				// Save memory by directly using a SimpleConstantValue.
				return new SimpleConstantValue(pc.Type, pc.Value);
			} else {
				return new CSharpConstantValue(c, usingScope, currentTypeDefinition);
			}
		}
		
		sealed class ConstantValueBuilder : DepthFirstAstVisitor<object, ConstantExpression>
		{
			readonly ITypeDefinition currentTypeDefinition;
			readonly IMethod currentMethod;
			readonly UsingScope usingScope;
			readonly bool isAttributeArgument;
			
			public ConstantValueBuilder(ITypeDefinition currentTypeDefinition, IMethod currentMethod, UsingScope usingScope, bool isAttributeArgument)
			{
				this.currentTypeDefinition = currentTypeDefinition;
				this.currentMethod = currentMethod;
				this.usingScope = usingScope;
				this.isAttributeArgument = isAttributeArgument;
			}
			
			ITypeReference ConvertType(AstType type)
			{
				return TypeSystemConvertVisitor.ConvertType(type, currentTypeDefinition, currentMethod, usingScope, SimpleNameLookupMode.Type);
			}
			
			protected override ConstantExpression VisitChildren(AstNode node, object data)
			{
				return null;
			}
			
			public override ConstantExpression VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, object data)
			{
				return new PrimitiveConstantExpression(KnownTypeReference.Object, null);
			}
			
			public override ConstantExpression VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
			{
				TypeCode typeCode = Type.GetTypeCode(primitiveExpression.Value.GetType());
				return new PrimitiveConstantExpression(typeCode.ToTypeReference(), primitiveExpression.Value);
			}
			
			IList<ITypeReference> ConvertTypeArguments(AstNodeCollection<AstType> types)
			{
				int count = types.Count;
				if (count == 0)
					return null;
				ITypeReference[] result = new ITypeReference[count];
				int pos = 0;
				foreach (AstType type in types) {
					result[pos++] = ConvertType(type);
				}
				return result;
			}
			
			public override ConstantExpression VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
			{
				return new ConstantIdentifierReference(identifierExpression.Identifier, ConvertTypeArguments(identifierExpression.TypeArguments));
			}
			
			public override ConstantExpression VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
			{
				TypeReferenceExpression tre = memberReferenceExpression.Target as TypeReferenceExpression;
				if (tre != null) {
					// handle "int.MaxValue"
					return new ConstantMemberReference(
						ConvertType(tre.Type),
						memberReferenceExpression.MemberName,
						ConvertTypeArguments(memberReferenceExpression.TypeArguments));
				}
				ConstantExpression v = memberReferenceExpression.Target.AcceptVisitor(this, data);
				if (v == null)
					return null;
				return new ConstantMemberReference(
					v, memberReferenceExpression.MemberName,
					ConvertTypeArguments(memberReferenceExpression.TypeArguments));
			}
			
			public override ConstantExpression VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
			{
				return parenthesizedExpression.Expression.AcceptVisitor(this, data);
			}
			
			public override ConstantExpression VisitCastExpression(CastExpression castExpression, object data)
			{
				ConstantExpression v = castExpression.Expression.AcceptVisitor(this, data);
				if (v == null)
					return null;
				return new ConstantCast(ConvertType(castExpression.Type), v);
			}
			
			public override ConstantExpression VisitCheckedExpression(CheckedExpression checkedExpression, object data)
			{
				ConstantExpression v = checkedExpression.Expression.AcceptVisitor(this, data);
				if (v != null)
					return new ConstantCheckedExpression(true, v);
				else
					return null;
			}
			
			public override ConstantExpression VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
			{
				ConstantExpression v = uncheckedExpression.Expression.AcceptVisitor(this, data);
				if (v != null)
					return new ConstantCheckedExpression(false, v);
				else
					return null;
			}
			
			public override ConstantExpression VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
			{
				return new ConstantDefaultValue(ConvertType(defaultValueExpression.Type));
			}
			
			public override ConstantExpression VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
			{
				ConstantExpression v = unaryOperatorExpression.Expression.AcceptVisitor(this, data);
				if (v == null)
					return null;
				switch (unaryOperatorExpression.Operator) {
					case UnaryOperatorType.Not:
					case UnaryOperatorType.BitNot:
					case UnaryOperatorType.Minus:
					case UnaryOperatorType.Plus:
						return new ConstantUnaryOperator(unaryOperatorExpression.Operator, v);
					default:
						return null;
				}
			}
			
			public override ConstantExpression VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
			{
				ConstantExpression left = binaryOperatorExpression.Left.AcceptVisitor(this, data);
				ConstantExpression right = binaryOperatorExpression.Right.AcceptVisitor(this, data);
				if (left == null || right == null)
					return null;
				return new ConstantBinaryOperator(left, binaryOperatorExpression.Operator, right);
			}
			
			public override ConstantExpression VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
			{
				if (isAttributeArgument) {
					return new PrimitiveConstantExpression(KnownTypeReference.Type, ConvertType(typeOfExpression.Type));
				} else {
					return null;
				}
			}
			
			public override ConstantExpression VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
			{
				var initializer = arrayCreateExpression.Initializer;
				// Attributes only allow one-dimensional arrays
				if (isAttributeArgument && !initializer.IsNull && arrayCreateExpression.Arguments.Count < 2) {
					ITypeReference type;
					if (arrayCreateExpression.Type.IsNull) {
						type = null;
					} else {
						type = ConvertType(arrayCreateExpression.Type);
						foreach (var spec in arrayCreateExpression.AdditionalArraySpecifiers.Reverse()) {
							type = ArrayTypeReference.Create(type, spec.Dimensions);
						}
					}
					ConstantExpression[] elements = new ConstantExpression[initializer.Elements.Count];
					int pos = 0;
					foreach (Expression expr in initializer.Elements) {
						ConstantExpression c = expr.AcceptVisitor(this, data);
						if (c == null)
							return null;
						elements[pos++] = c;
					}
					return new ConstantArrayCreation(type, elements);
				} else {
					return null;
				}
			}
		}
		#endregion
		
		#region Parameters
		void ConvertParameters(IList<IParameter> outputList, IEnumerable<ParameterDeclaration> parameters)
		{
			foreach (ParameterDeclaration pd in parameters) {
				DefaultParameter p = new DefaultParameter(ConvertType(pd.Type), pd.Name);
				p.Region = MakeRegion(pd);
				ConvertAttributes(p.Attributes, pd.Attributes);
				switch (pd.ParameterModifier) {
					case ParameterModifier.Ref:
						p.IsRef = true;
						p.Type = ByReferenceTypeReference.Create(p.Type);
						break;
					case ParameterModifier.Out:
						p.IsOut = true;
						p.Type = ByReferenceTypeReference.Create(p.Type);
						break;
					case ParameterModifier.Params:
						p.IsParams = true;
						break;
				}
				if (!pd.DefaultExpression.IsNull)
					p.DefaultValue = ConvertConstantValue(p.Type, pd.DefaultExpression);
				outputList.Add(p);
			}
		}
		#endregion
	}
}
