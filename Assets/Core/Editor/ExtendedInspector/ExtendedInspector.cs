using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CoreEditor.Extensions;
using CoreEngine;
using System.IO;
using CoreEditor.Reflection;

namespace CoreEditor
{
	
[InitializeOnLoad]
public static class ExtendedInspector
{
	class Parent
	{
		const string Element = "Element ";
		public bool HasChildren{get{return ChildrenType!=null;}}
		public Type Type{get;private set;}
		public Type ChildrenType{get;private set;}
		int currentIndex = 0;
		public string ArrayElementName{ get{return Element+(currentIndex++).ToString();}}
		
		public Parent( Type type )
		{
			Type = type;
			
			if ( Type.IsGenericType )
				ChildrenType = Type.GetGenericArguments()[0];
			else if ( Type.IsArray )
				ChildrenType = Type.GetElementType();
		}
	}

	public static Dictionary<Type,Type> AttributeToDrawer{get;private set;}
	static ExtendedInspector()
	{
		AttributeToDrawer = new Dictionary<Type, Type>();
		var path = Directory.GetCurrentDirectory();
		
		var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where( a=>a.Location.StartsWith(path) );
		foreach( var assembly in assemblies )
		{
			foreach( var type in assembly.GetTypes() )
			{
				if ( typeof(IDrawer).IsAssignableFrom( type ) )
				{
					var attribute = type.GetCustomAttributes(true).FirstOrDefault( a=>a is ExtendedInspectorDrawerAttribute ) as ExtendedInspectorDrawerAttribute;
					if ( attribute != null && typeof( IDrawer ).IsAssignableFrom( type ) )
						AttributeToDrawer.Add( attribute.Type, type );
				}

			}
		}
	}

	internal const BindingFlags SerializeFieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static void ShowGenericEditor( SerializedObject serializedObject )
	{
		serializedObject.Update();
		ShowPropertiesWithToopTips( serializedObject );
		serializedObject.ApplyModifiedProperties();
		EditorGUI.indentLevel = 0;
	}

	internal static void ShowPropertiesWithToopTips( SerializedObject serializedObject )
	{
		SerializedProperty property = serializedObject.GetIterator();

		var rootType = serializedObject.targetObject.GetType();

		var descriptions = rootType.HelpBox();
		foreach ( var description in descriptions )
			EditorGUILayout.HelpBox( description.Value, description.Key );

		var parentTypes = new Stack<Parent>();
		parentTypes.Push( new Parent( rootType ) );
		EditorGUI.indentLevel = 0;

		bool childrenAreExpanded = true;
		while( property.NextVisible(childrenAreExpanded) )
		{
			PopParentIfNeeded( property, ref parentTypes );
			EditorGUI.indentLevel = property.depth;

			var field = parentTypes.Peek().Type.GetSerializedField( property.name, SerializeFieldBindingFlags );
			var label = CreateLabel( property, parentTypes, field );
			childrenAreExpanded = DisplayProperty( property, field, label );
			PushParentIfNeeded( property, field, ref parentTypes );
		}
	}

		
	static void AddDrawerTooltip( GUIContent label, IDrawer drawer )
	{
		var hasTooltip = string.IsNullOrEmpty(label.tooltip)==false;
		label.tooltip = (hasTooltip?(label.tooltip+"\n\n"):string.Empty)+drawer.Tooltip;
	}

	internal static bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
	{
		if ( field != null )
		{
			foreach( var attribute in field.GetCustomAttributes(true) )
			{
				Type drawerType;
				if ( AttributeToDrawer.TryGetValue( attribute.GetType(), out drawerType ) )
				{
					var drawer = Activator.CreateInstance( drawerType, new object[]{attribute}, new object[]{}) as IDrawer;
					AddDrawerTooltip(label, drawer);
					MarkIfToolTip( label );
					return drawer.DisplayProperty(property, field, label );
				}
			}
		}
		MarkIfToolTip( label );
		try
		{
			return EditorGUILayout.PropertyField( property, label );	
		}
		catch(ArgumentException)
		{
			return false;
		}
	}
	
	static void MarkIfToolTip( GUIContent label )
	{
		if ( string.IsNullOrEmpty( label.tooltip) == false )
			label.text += " *";
	}

	static void PopParentIfNeeded( SerializedProperty property, ref Stack<Parent> parentTypes )
	{
		while ( parentTypes.Count > property.depth + 1 )
			parentTypes.Pop();
	}

	static void PushParentIfNeeded( SerializedProperty property, FieldInfo field, ref Stack<Parent> parentTypes )
	{
		if ( property.hasChildren )
		{
			if ( field != null )
			{
				parentTypes.Push( new Parent( field.FieldType ) );
			}
			else
			{
				if ( parentTypes.Peek().HasChildren )
					parentTypes.Push( new Parent( parentTypes.Peek().ChildrenType ) );
				else
					parentTypes.Push( new Parent( typeof(object) ) );
			}
		}
	}

	static GUIContent CreateLabel( SerializedProperty property, Stack<Parent> parentTypes, FieldInfo field )
	{
		var label = new GUIContent( property.ReadableName() );
		if ( field != null )
		{
			label.tooltip = field.ToolTip();
		}
		else
		{
			if ( parentTypes.Peek().HasChildren && property.name == "data" )
				label.text = parentTypes.Peek().ArrayElementName;
		}
		return label;
	}


}


namespace Extensions
{
	static partial class Extensions
	{
		public static SortedDictionary<MessageType,string> HelpBox( this Type type )
		{
			var splitHelpBox = new SortedDictionary<MessageType,string>( );
			var helpBoxes = ( from description in type.GetCustomAttributes( typeof( HelpBoxAttribute ), true ).Reverse( )
			                 select ( description as HelpBoxAttribute ) );
				
			foreach ( ObsoleteAttribute obsolete in type.GetCustomAttributes( typeof( ObsoleteAttribute ), true ) )
			{
				var messageType = obsolete.IsError?MessageType.Error:MessageType.Warning;
				var message = string.IsNullOrEmpty( obsolete.Message )?"Obsolete":("Obsolete: " + obsolete.Message);
				MergeHelpBox( splitHelpBox, messageType, message );			
			}

			foreach ( var helpBox in helpBoxes )
			{
				MessageType messageType = (MessageType)helpBox.Icon;
				MergeHelpBox( splitHelpBox, messageType, helpBox.Description );
			}
			return splitHelpBox;
		}
		static void MergeHelpBox( SortedDictionary<MessageType,string> splitHelpBox,MessageType messageType, string message )
		{
			if ( splitHelpBox.ContainsKey( messageType ) == false )
				splitHelpBox.Add( messageType, message );
			else
				splitHelpBox[messageType] = splitHelpBox[messageType] + "\n\n" + message;				
		}
		
		public static string ToolTip(this FieldInfo field )
		{
			var fullDescription = (from description in field.GetCustomAttributes(typeof(ToolTipAttribute),true).Reverse()
			                       select (description as ToolTipAttribute).Description).ToArray();
			
			if ( fullDescription.Length == 0 )
				return null;
			else
				return string.Join( "\n", fullDescription );
		}

		static readonly Regex RemoveM = new Regex( "^M [A-Z]|^M[A-Z]" );
		public static string ReadableName( this SerializedProperty property )
		{
			return RemoveM.Replace( property.displayName, (m)=>m.Value[m.Value.Length-1].ToString());
		}
	}
}
}