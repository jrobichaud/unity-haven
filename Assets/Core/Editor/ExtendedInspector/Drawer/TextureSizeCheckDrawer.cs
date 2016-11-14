using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using CoreEngine;

namespace CoreEditor
{
	[ExtendedInspectorDrawer(typeof( TextureSizeCheckAttribute))]
	public sealed class TextureSizeCheckDrawerAttribute : Attribute, IDrawer
	{
		int width;
		int height; 
		public TextureSizeCheckDrawerAttribute( TextureSizeCheckAttribute attribute )
		{
			this.width = attribute.Width;
			this.height = attribute.Height;
		}

		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			if ( property.propertyType == SerializedPropertyType.ObjectReference )
			{
				var texture = property.objectReferenceValue as Texture2D;

				return ExtendedInspectorDrawerAttribute.DrawProperty(IsValid(texture),label, ()=> EditorGUILayout.PropertyField (property, label), GetError( texture ) );
			}
			return EditorGUILayout.PropertyField( property, label );
		}

		public bool IsValid( object value )
		{
			var texture = (Texture)value;
			return texture != null && texture.width == width && texture.height == height;
		}
		public string GetError( object value )
		{
			var texture = value as Texture;
			if ( texture == null )
				return "Texture is null";
			if ( texture.width != width || texture.height != height )
				return string.Format( "Size: {0}x{1} does not match {2}x{3}", texture.width, texture.height, width, height );
			return string.Empty;
		}

		public string Tooltip
		{
			get {
				return "Dimensions:\n" + width + "x" + height;
			}
		}
	}
}
