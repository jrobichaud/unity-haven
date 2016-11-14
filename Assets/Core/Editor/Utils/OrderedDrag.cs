using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Utils
{
	public class OrderedDrag : EditorWindow
	{
		MessageType type = MessageType.None;
		public void OnEnable()
		{
			titleContent.text = ObjectNames.NicifyVariableName( GetType().Name );
		}	
		public void OnGUI()
		{
			EditorGUILayout.HelpBox( "Drag mouse out of this window to create an ordeded DragAndDrop from selection.", MessageType.Info, true );

			if ( Event.current.type == EventType.MouseDrag )
			{
				if ( Selection.objects.Length > 1 )
				{
					DragAndDrop.PrepareStartDrag();
					var objects = Selection.objects.OrderBy( o=>o.name ).ToArray();
					DragAndDrop.objectReferences = objects;
					DragAndDrop.StartDrag( "Ordered Drag: <Multiple>" );
					Event.current.Use();
					Repaint();
					type = MessageType.None;
				}
				else
				{
					Repaint();
					type = MessageType.Error;
				}
			}
			if ( Event.current.type == EventType.MouseDown )
			{
				Repaint();
				type = MessageType.None;
			}
			EditorGUILayout.HelpBox( type == MessageType.Error? "More than one selected object is required": string.Empty, type, true );
		}

		[MenuItem("Window/Ordered Drag")]
		public static void Open()
		{
			var window = EditorWindow.GetWindow( typeof(OrderedDrag) );
			window.Show();
		}
	}
}