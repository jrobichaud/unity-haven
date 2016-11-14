using System.Collections.Generic;
using System.Reflection;
using System;

namespace CoreEditor.Reflection
{
	public static class TypeExtensions
	{
		public static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
		{
			if(type == typeof(Object)) return new List<FieldInfo>();
			
			var list = type.BaseType.GetAllFields(flags);
			// in order to avoid duplicates, force BindingFlags.DeclaredOnly
			list.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
			return list;
		}

		
		public static FieldInfo GetSerializedField(this Type type, string fieldName, BindingFlags flags)
		{
			if(type == typeof(Object)) return null;

			var field = type.GetField( fieldName, flags );
			if ( field != null && ( field.IsPublic  || field.IsDefined( typeof(UnityEngine.SerializeField), true ) ) )
				return field;
			else
				return type.BaseType.GetSerializedField(fieldName,flags);
		}
	}
}