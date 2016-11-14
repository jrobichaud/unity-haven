using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using CoreEngine;

namespace CoreEditor
{
	[ExtendedInspectorDrawer( typeof( SliderAttribute ) )]
	public sealed class SliderDrawerAttribute : Attribute, IDrawer
	{
		float min;
		float max;
		public SliderDrawerAttribute( SliderAttribute slider )
		{
			this.min = slider.Min;
			this.max = slider.Max;
		}
		
		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			if ( property.propertyType == SerializedPropertyType.Float )
			{
	        	EditorGUILayout.Slider (property,min, max, label);
				return false;
			}
			else if ( property.propertyType == SerializedPropertyType.Integer )
			{
	        	EditorGUILayout.IntSlider (property,(int)min, (int)max, label);
				return false;
			}
			
			return EditorGUILayout.PropertyField( property, label );
		}

		public bool IsValid( object value )
		{
			return true;
		}

		public string GetError( object value )
		{
			return string.Empty;
		}
		
		public string Tooltip
		{
			get {
				return string.Format("Range: [{0}, {1}]",min,max);
			}
		}
	}
}