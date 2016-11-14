using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using CoreEngine;

namespace CoreEditor
{
	[ExtendedInspectorDrawer( typeof( NonObsoleteAttribute ) )]
	public sealed class NonObsoleteDrawerAttribute : Attribute, IDrawer
	{
		bool mTreatWarningAsError;
		public NonObsoleteDrawerAttribute( NonObsoleteAttribute attribute )
		{
			mTreatWarningAsError = attribute.TreatWarningAsError;
		}
		
		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			object value = null;


			var enumValueRaw = property.enumNames[property.enumValueIndex];

			var enumValue = Enum.Parse( field.FieldType, enumValueRaw );

			value = enumValue;
			
			var obsolete = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute),true);
			if ( IsValid( value ) == false )
				return ExtendedInspectorDrawerAttribute.DrawProperty( false,label, ()=> EditorGUILayout.PropertyField (property, label), Color.red, GetError( value ) );
			else if ( obsolete.Length > 0 )
				return ExtendedInspectorDrawerAttribute.DrawProperty( false,label, ()=> EditorGUILayout.PropertyField (property, label), Color.yellow, GetError( value ) );
			else
				return ExtendedInspectorDrawerAttribute.DrawProperty( true,label, ()=> EditorGUILayout.PropertyField (property, label) );
		}

		public bool IsValid( object value )
		{
			var obsolete = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute),true);
			if ( obsolete.Length > 0 )
			{
				if ( mTreatWarningAsError || (obsolete[0] as ObsoleteAttribute).IsError )
					return false;
			}
			return true;
		}

		public string GetError( object value )
		{
			var obsolete = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute),true);
			if ( obsolete.Length > 0 )
			{
				return (obsolete[0] as ObsoleteAttribute).Message;
			}
			return string.Empty;
		}
		
		public string Tooltip
		{
			get {
				return "NonObsolete";
			}
		}
	}
}