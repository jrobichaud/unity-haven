using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace CoreEditor
{
	public interface IDrawer 
	{
		bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label );
		bool IsValid( object value );
		string GetError( object value );
		string Tooltip{get;}
	}
}