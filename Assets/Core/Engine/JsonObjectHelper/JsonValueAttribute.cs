using System;

namespace CoreEngine
{
	[AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
	sealed public class JsonValueAttribute : Attribute
	{
		public Type OriginalType{ get; private set; }
		public string JsonValueName{ get; private set;}
		public bool IgnoreConversionWarning{ get; private set;}
		public JsonValueAttribute( string jsonValueName ) : this( jsonValueName, null )
		{
		}

		public JsonValueAttribute( string jsonValueName, bool ignoreConversionWarning ) : this( jsonValueName, null )
		{
			IgnoreConversionWarning = ignoreConversionWarning;
		}

		public JsonValueAttribute( string jsonValueName, Type originalType )
		{
			JsonValueName = jsonValueName;
			OriginalType = originalType;
		}
	}
}