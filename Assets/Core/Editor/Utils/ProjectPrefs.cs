using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

namespace CoreEditor.Utils
{
	public abstract class ProjectPref<T>
	{
		static Func<string,T,T> GetWithDefaultValue;
		static Action<string,T> SetValue;

		string mKey;
		T mValue = default(T);

		public ProjectPref( string key, T defaultValue )
		{
			mKey = Directory.GetCurrentDirectory().GetHashCode().ToString() + "." + key;
			mValue = GetWithDefaultValue( mKey, defaultValue );
		}

		protected static void SetAccessors( Func<string,T,T> getWithDefaultValue, Action<string,T> setValue )
		{
			GetWithDefaultValue = getWithDefaultValue;
			SetValue = setValue;
		}

		public T Value
		{
			get{return mValue;}
			set{
				if ( !EqualityComparer<T>.Default.Equals( value , mValue ) )
				{
					mValue = value;
					SetValue( mKey, value );
				}
			}
		}
	}

	public class BoolProjectPref: ProjectPref<bool>
	{
		static BoolProjectPref()
		{
			SetAccessors( EditorPrefs.GetBool, EditorPrefs.SetBool );
		}
		public BoolProjectPref( string key, bool defaultValue ): base( key, defaultValue )
		{
		}
	}

	public class StringProjectPref: ProjectPref<string>
	{
		static StringProjectPref()
		{
			SetAccessors( EditorPrefs.GetString, EditorPrefs.SetString );
		}
		public StringProjectPref( string key, string defaultValue ): base( key, defaultValue )
		{
		}
	}

	public class FloatProjectPref: ProjectPref<float>
	{
		static FloatProjectPref()
		{
			SetAccessors( EditorPrefs.GetFloat, EditorPrefs.SetFloat );
		}
		public FloatProjectPref( string key, float defaultValue ): base( key, defaultValue )
		{
		}
	}

	public class IntProjectPref: ProjectPref<int>
	{
		static IntProjectPref()
		{
			SetAccessors( EditorPrefs.GetInt, EditorPrefs.SetInt );
		}
		public IntProjectPref( string key, int defaultValue ): base( key, defaultValue )
		{
		}
	}
}