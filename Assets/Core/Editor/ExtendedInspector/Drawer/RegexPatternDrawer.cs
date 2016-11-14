using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using CoreEngine;

namespace CoreEditor
{
	[ExtendedInspectorDrawer(typeof(RegexPatternAttribute))]
	public sealed class RegexPatternDrawerAttribute: IDrawer
	{
		string regexString;
		Regex regex; 
		public RegexPatternDrawerAttribute( RegexPatternAttribute attribute )
		{
			this.regexString = attribute.regexString;
			try
			{
				regex = new Regex( regexString );
			}
			catch (Exception e)
			{
				regex = new Regex(".*");
				Debug.LogError( e.Message );
			}
		}
		
		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			if ( property.propertyType == SerializedPropertyType.String )
				return ExtendedInspectorDrawerAttribute.DrawProperty(IsValid(property.stringValue),label, ()=> EditorGUILayout.PropertyField (property, label),GetError(property.stringValue) );
			return EditorGUILayout.PropertyField( property, label );
		}

		public bool IsValid( object value )
		{
			return regex.IsMatch( (string)value );
		}

		public string GetError( object value )
		{
			return string.Format( @"""{0}"" does not match regex ""{1}"".", (string)value, regexString );
		}
		
		public string Tooltip
		{
			get {
				return "Regex Pattern:\n" + regexString;
			}
		}

	}
}
