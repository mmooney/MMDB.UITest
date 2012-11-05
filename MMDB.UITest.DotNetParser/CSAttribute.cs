using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSAttribute
	{
		public class CSAttributeArgument
		{
			public string ArgumentName { get; set; }
			public object ArguementValue { get; set; }
		}
		public string TypeName { get; set; }
		public string TypeNamespace { get; set; }
		public string TypeFullName
		{
			get
			{
				return DotNetParserHelper.BuildFullName(this.TypeNamespace, this.TypeName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.TypeName = className;
				this.TypeNamespace = namespaceName;
			}
		}
		public List<CSAttributeArgument> ArgumentList { get; set; }

		public CSAttribute()
		{
			this.ArgumentList = new List<CSAttributeArgument>();
		}

		public static CSAttribute Parse(ICSharpCode.NRefactory.CSharp.Attribute attributeNode)
		{
			var returnValue = new CSAttribute();
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(attributeNode.Type.ToString(), out typeName, out typeNamespace);
			returnValue.TypeName = typeName;
			returnValue.TypeNamespace = typeNamespace;

			foreach (var node in attributeNode.Children)
			{
				if(node is PrimitiveExpression)
				{
					var primitiveExpressionNode = (PrimitiveExpression)node;
					var argument = new CSAttributeArgument
					{
						ArguementValue = primitiveExpressionNode.Value
					};
					returnValue.ArgumentList.Add(argument);
				}
				if(node is NamedExpression)
				{
					var namedExpressionNode = (NamedExpression)node;
					if(namedExpressionNode.Expression is PrimitiveExpression)
					{
						var primitiveExpressionNode = (PrimitiveExpression)namedExpressionNode.Expression;
						var argument = new CSAttributeArgument
						{
							ArgumentName = namedExpressionNode.Identifier,
							ArguementValue = primitiveExpressionNode.Value
						};
						returnValue.ArgumentList.Add(argument);
					}
					else if(namedExpressionNode.Expression is MemberReferenceExpression)
					{
						var memberReferenceExpressionNode = (MemberReferenceExpression)namedExpressionNode.Expression;
					}
					else 
					{
						throw new Exception("Unrecognized expression type: " + namedExpressionNode.Expression.GetType().FullName);
					}
				}
				if(node is NamedArgumentExpression)
				{
					var namedArgumentExpressionNode = (NamedArgumentExpression)node;
					if (namedArgumentExpressionNode.Expression is PrimitiveExpression)
					{
						var primitiveExpressionNode = (PrimitiveExpression)namedArgumentExpressionNode.Expression;
						var argument = new CSAttributeArgument
						{
							ArgumentName = namedArgumentExpressionNode.Identifier,
							ArguementValue = primitiveExpressionNode.Value
						};
						returnValue.ArgumentList.Add(argument);
					}
					else if (namedArgumentExpressionNode.Expression is MemberReferenceExpression)
					{
						var memberReferenceExpressionNode = (MemberReferenceExpression)namedArgumentExpressionNode.Expression;
					}
					else
					{
						throw new Exception("Unrecognized expression type: " + namedArgumentExpressionNode.Expression.GetType().FullName);
					}
					
				}
			}
			return returnValue;
		}

		public object GetAttributeParameter(int ordinalPosition, string fieldName, bool required)
		{
			object returnValue = null;
			if(string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentNullException("fieldName required");
			}
			if(ordinalPosition < 0)
			{
				throw new ArgumentOutOfRangeException("ordinalPosition must be greater than 0");
			}
			else if (ordinalPosition >= this.ArgumentList.Count)
			{
				throw new ArgumentOutOfRangeException("ordinalPosition must be less than argument list count");
			}
			var argument = this.ArgumentList.SingleOrDefault(i=>i.ArgumentName == fieldName);
			if(argument != null)
			{
				returnValue = argument.ArguementValue;
			}
			else 
			{
				if(this.ArgumentList.Count > ordinalPosition)
				{
					bool anyPrecedingOrCurrentNamedArguments = this.ArgumentList.Take(ordinalPosition+1).Any(i=>!string.IsNullOrEmpty(i.ArgumentName));
					if (!anyPrecedingOrCurrentNamedArguments)
					{
						returnValue = this.ArgumentList[ordinalPosition].ArguementValue;
					}
					else if(required)
					{
						throw new ArgumentException(string.Format("Unable to load attribute parameter position \"{0}\" for attribute \"{1}\"", ordinalPosition, this.TypeName));
					}
				}
			}
			if(required && returnValue == null)
			{
				throw new ArgumentException(string.Format("Unable to load attribute parameter \"{0}\" for attribute \"{1}\"",fieldName,this.TypeName));
			}
			return returnValue;
		}
	}
}
