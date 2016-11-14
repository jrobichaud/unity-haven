using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using CoreEngine;

namespace CoreEditor
{
	[ExtendedInspectorDrawer( typeof( NonNullAttribute ) )]
	public sealed class NonNullDrawerAttribute : Attribute, IDrawer
	{
		public NonNullDrawerAttribute( NonNullAttribute attribute )
		{
		}
		
		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			object value = null;

			if ( property.propertyType == SerializedPropertyType.String )
				value = property.stringValue;
			else if ( property.propertyType == SerializedPropertyType.ObjectReference )
				value = property.objectReferenceValue;

			
			return ExtendedInspectorDrawerAttribute.DrawProperty(IsValid(value),label, ()=> EditorGUILayout.PropertyField (property, label), GetError( value ) );
		}

		public bool IsValid( object value )
		{
			return ( value == null || value.Equals( null ) ) == false;
		}

		public string GetError( object value){ return "Value is null."; }
		
		public string Tooltip
		{
			get {
				return "NonNull";
			}
		}
	}
}