using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CoreEngine
{
	public static class JsonObjectHelper
	{
		public static string Serialize( object obj )
		{
			object jsonObject = null;

			if ( typeof( IEnumerable ).IsInstanceOfType( obj ) )
			{
				var list = new List<object>();
				var type = obj.GetType();
				if ( type.IsGenericType )
				{
					var genericType = type.GetGenericArguments() [0];
					if ( genericType.IsDefined( typeof(JsonSerializableAttribute),true ) )
					{
						foreach ( object element in obj as IEnumerable )
							list.Add( SerializeObject( element ) );
					}
					else if ( genericType.IsPrimitive || genericType == typeof( string ) )
					{
						foreach ( object element in obj as IEnumerable )
							list.Add( element );
					}
				}
				else
				{
					foreach ( object element in obj as IEnumerable )
						list.Add( element );
				}
				jsonObject = list;
			}
			else if ( obj.GetType().IsDefined( typeof(JsonSerializableAttribute), true ) )
			{
				jsonObject = SerializeObject( obj );
			}

			return MiniJson.Json.Serialize( jsonObject );
		}

		public static object SerializeObject( object obj )
		{
			var properties = obj.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

			var data = new Dictionary<string,object>();

			foreach ( var property in properties )
			{
				foreach ( JsonValueAttribute jsonAttribute in property.GetCustomAttributes( typeof(JsonValueAttribute), true ) )
				{
					var type = property.PropertyType;
					var value = property.GetValue( obj, new object[]{} );
					object jsonValue = null;
					if ( type.IsPrimitive || value is string  )
					{
						if ( jsonAttribute.OriginalType != null )
							jsonValue = Convert.ChangeType( value, jsonAttribute.OriginalType );
						else
							jsonValue = value;
					}
					else if ( value == null )
					{
						jsonValue = null;
					}
					else if ( typeof( IDictionary ).IsInstanceOfType( value ) )
					{
						var dico = new Dictionary<string,object>();
						if ( type.IsGenericType )
						{
							var genericType = type.GetGenericArguments()[1];
							if ( genericType.IsDefined(typeof(JsonSerializableAttribute), true ) )
							{
								var enumerator = (value as IDictionary).GetEnumerator();
								enumerator.Reset();
								
								while ( enumerator.MoveNext() )
									dico.Add( (string)enumerator.Key, SerializeObject(enumerator.Value) );
							}
							else if ( genericType.IsPrimitive || genericType == typeof( string ) )
							{
								var enumerator = (value as IDictionary).GetEnumerator();
								enumerator.Reset();

								while ( enumerator.MoveNext() )
									dico.Add( (string)enumerator.Key, enumerator.Value );								
							}
						}
						else
						{
							Debug.LogError("Type [" + type + "]not supported.");
						}
						jsonValue = dico;
					}
					else if ( typeof(IEnumerable).IsInstanceOfType( value ) )
					{
						var list = new List<object>();
						if ( type.IsGenericType )
						{
							var genericType = type.GetGenericArguments() [0];
							if ( genericType.IsDefined(typeof(JsonSerializableAttribute), true ) )
							{
								foreach ( object element in value as IEnumerable )
									list.Add( SerializeObject( element ) );
							}
							else if ( genericType.IsPrimitive || genericType == typeof( string ) )
							{
								foreach ( object element in value as IEnumerable )
									list.Add( element );
							}
						}
						else
						{
							foreach ( object element in value as IEnumerable )
								list.Add( element );
						}
						jsonValue = list;
					}
					else if ( value.GetType().IsDefined( typeof(JsonSerializableAttribute), true )  )
					{
						jsonValue = SerializeObject( value );
					}
					
					data.Add( jsonAttribute.JsonValueName, jsonValue );
				}
			}
			return data;
		}

		public static List<T> DeserializeJsonObjects<T>( List<object> jsonObjectList ) where T : new()
		{
			var outList = new List<T>();
			foreach ( var obj in jsonObjectList )
			{
				var subObject = new T();
				Deserialize( obj as Dictionary<string,object>, subObject );
				outList.Add( subObject );
			}
			return outList;
		}

		public static Dictionary<string,T> DeserializeDictionary<T>( Dictionary<string,object> jsonDictionary ) where T : new()
		{
			var outDictionary = new Dictionary<string,T>();
			foreach ( var obj in jsonDictionary )
			{
				var subObject = new T();
				Deserialize( obj.Value as Dictionary<string,object>, subObject );
				outDictionary.Add( obj.Key , subObject );
			}
			return outDictionary;
		}
		
		public static T Deserialize<T>( IDictionary<string,object> jsonDictionary) where T: new()
		{
			var obj = new T();
			JsonObjectHelper.Deserialize( jsonDictionary as IDictionary<string,object>, obj );
			return obj;
		}

		public static void Deserialize( IDictionary<string,object> jsonDictionary , object obj )
		{
			if ( jsonDictionary == null )
			{
				Debug.LogError( "Invalid data" );
				return;
			}
			var properties = obj.GetType().GetProperties( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			foreach ( var property in properties )
			{
				DeserializeProperty( property, jsonDictionary, obj );
			}
		}

		static void DeserializeProperty( PropertyInfo property, IDictionary<string, object> jsonDictionary, object obj )
		{
			foreach( JsonValueAttribute jsonProperty in property.GetCustomAttributes( typeof(JsonValueAttribute), true ) )
			{
				var name = jsonProperty.JsonValueName;
				object value = null;
				if ( jsonDictionary.TryGetValue( name, out value ) )
				{
					if ( value == null )
					{
						return;
					}
					else if ( property.PropertyType.IsInstanceOfType( value ) )
					{
						property.SetValue( obj, value, null );
					}
					else if ( value is string || property.PropertyType.IsPrimitive )
					{
						try
						{
							object convertedType = null;
							if ( value is string && string.IsNullOrEmpty( (string)value ) )
								convertedType = null;
							else
								convertedType = Convert.ChangeType( value, property.PropertyType );

							property.SetValue( obj, convertedType, null );
							if ( jsonProperty.IgnoreConversionWarning == false && ( jsonProperty.OriginalType == null || jsonProperty.OriginalType != value.GetType() ) )
								Debug.LogWarning( string.Format( "Property {0} of class {1} is of different type type {2} vs {3}.", name, obj.GetType(), property.PropertyType.Name, value.GetType().Name ) );
						}
						catch ( FormatException )
						{
							Debug.LogError( string.Format( "Property {0} of class {1} is of incompatible type {2} vs {3}.", name, obj.GetType(), property.PropertyType.Name, value.GetType().Name ) );
						}
					}
					else if ( value is List<object> )
					{
						var jsonList = value as List<object>;
						var constructor = property.PropertyType.GetConstructor( new Type[] { } );
						var listObject = constructor.Invoke( new object[] { } );
						var addMethod = listObject.GetType().GetMethod( "Add" );
						var elementType = property.PropertyType.GetGenericArguments() [0];
						if ( elementType.IsPrimitive || elementType == typeof( string ) )
						{
							foreach ( var jsonObject in jsonList )
								addMethod.Invoke( listObject, new object[] { jsonObject } );
						}
						else
						{
							foreach ( var jsonObject in jsonList )
							{
								var subObject = Instantiate( elementType, jsonObject as IDictionary<string,object> );
								if ( subObject != null )
									addMethod.Invoke( listObject, new object[] { subObject } );
							}
						}
						property.SetValue( obj, listObject, null );
					}
					else if ( value is IDictionary<string,object> )
					{
						var subObject = Instantiate( property.PropertyType, value as IDictionary<string,object> );
						property.SetValue( obj, subObject, null );
					}
					else
					{
						Debug.LogError( string.Format( "Property {0} of class {1} is of incompatible type {2} vs {3}.", name, obj.GetType(), property.PropertyType.Name, value.GetType().Name ) );
					}
				}
			}
		}

		static object Instantiate( Type type, IDictionary<string,object> jsonObject )
		{
			var elementConstructor = type.GetConstructor( new Type[]{} );
			
			if ( elementConstructor != null )
			{
				var subObject = elementConstructor.Invoke( new object[]{} );
				Deserialize( jsonObject, subObject );
				return subObject;
			}
			else
			{
				Debug.LogError( "Unable to instantiate class since it has no default constructor: " + type.Name );
				return null;
			}
		}

	}
}