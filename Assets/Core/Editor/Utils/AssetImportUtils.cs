using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CoreEditor.Utils
{
public static class AssetImportUtils
{
	static IDictionary<string, object> GetAllUserData( AssetImporter importer )
	{
		if ( string.IsNullOrEmpty( importer.userData ) == true )
			return new Dictionary<string, object>();
		
		var structuredData = CoreEngine.MiniJson.Json.Deserialize( importer.userData );
		
		var dico = structuredData as IDictionary<string, object>;
		if ( dico == null )
		{
			Debug.LogError( importer.assetPath + " has non-dictionary user data: " + importer.userData );
			return null;
		}
		
		return dico;
	}

	public static T GetUserData<T>( AssetImporter importer, string key )
	{
		var dico = GetAllUserData( importer );
		if ( dico.ContainsKey( key ) == false )
			return default(T);
		return (T) dico[key];
	}

	public static T GetUserData<T>( string assetPath, string key )
	{
		if ( string.IsNullOrEmpty( assetPath ) )
		{
			Debug.LogError( "Empty path for asset" );
			return default(T);
		}
		
		var importer = AssetImporter.GetAtPath( assetPath );
		return GetUserData<T>( importer, key );
	}

	public static T GetUserData<T>( Object assetObject, string key )
	{
		return GetUserData<T>( AssetDatabase.GetAssetPath( assetObject ), key );
	}
	
	public static T GetUserData<T>( int instanceID, string key )
	{
		return GetUserData<T>( AssetDatabase.GetAssetPath( instanceID ), key );
	}
	
	public static void SetUserData( AssetImporter importer, string key, object value )
	{
		var dico = GetAllUserData( importer );
		if ( dico == null )
		{
			Debug.LogError( "Cannot set user data" );
			return;
		}

		if (value == null)
			dico.Remove(key);
		else
			dico[key] = value;

		importer.userData = CoreEngine.MiniJson.Json.Serialize( dico );
	}

	public static void SetUserData( string assetPath, string key, object value )
	{
		if ( string.IsNullOrEmpty( assetPath ) )
		{
			Debug.LogError( "Empty path for asset" );
			return;
		}
		
		var importer = AssetImporter.GetAtPath( assetPath );
		SetUserData( importer, key, value );
	}

	public static void SetUserData( Object assetObject, string key, object value )
	{
		SetUserData( AssetDatabase.GetAssetPath( assetObject ), key, value );
	}

	public static void SetUserData( int instanceID, string key, object value )
	{
		SetUserData( AssetDatabase.GetAssetPath( instanceID ), key, value );
	}
}
}
