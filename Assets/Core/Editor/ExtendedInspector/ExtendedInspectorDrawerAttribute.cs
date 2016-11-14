using System;
using UnityEngine;

namespace CoreEditor
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExtendedInspectorDrawerAttribute : Attribute
	{
		public Type Type{get;private set;}
		public ExtendedInspectorDrawerAttribute( Type type )
		{
			Type = type;
		}
		
		public static bool DrawProperty(bool success, GUIContent content, Func<bool> propertyDrawReturningChildrenAreExpanded, string error= null )
		{
			return DrawProperty( success, content, propertyDrawReturningChildrenAreExpanded, Color.red, error );
		}

		public static bool DrawProperty(bool userDefaultColor,GUIContent content, Func<bool> propertyDrawReturningChildrenAreExpanded, Color color, string error = null )
		{
			var previousColor = GUI.color;			
			if ( userDefaultColor == false )
			{
				GUI.color = color;
				if ( string.IsNullOrEmpty( error ) == false )
					content.tooltip += string.Format( "\n\nError:\n{0}", error );
			}

			var childrenAreVisible = propertyDrawReturningChildrenAreExpanded();
			GUI.color = previousColor;
			return childrenAreVisible;
		}
	}
}
