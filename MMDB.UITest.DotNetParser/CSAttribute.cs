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
			}
			return returnValue;
		}

		public string GetAttributeParameter(int ordinalPosition, string fieldName, bool required)
		{
			string returnValue = null;
			var argument = this.ArgumentList.SingleOrDefault(i=>i.ArgumentName == fieldName);
			if(argument != null)
			{
				returnValue = Convert.ToString(argument.ArguementValue);
			}
			else 
			{
				if(this.ArgumentList.Count > ordinalPosition)
				{
					bool anyPrecedingNamedArguments = this.ArgumentList.Take(ordinalPosition).Any(i=>!string.IsNullOrEmpty(i.ArgumentName));
					if(!anyPrecedingNamedArguments)
					{
						returnValue = Convert.ToString(this.ArgumentList[ordinalPosition].ArguementValue);
					}
				}
			}
			if(required && string.IsNullOrEmpty(returnValue))
			{
				throw new Exception(string.Format("Unable to load attribute parameter \"{0}\" for attribute \"{1}\"",fieldName,this.TypeName));
			}
			return returnValue;
		}
	}
}
