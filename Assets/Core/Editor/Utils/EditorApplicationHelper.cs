using UnityEditor;
using UnityEngine;
using System;

namespace CoreEditor
{
	public class EditorApplicationHelper
	{
		class EditorUpdateRunner
		{
			Action mAction;
			EditorUpdateRunner( Action action )
			{
				mAction = action ?? delegate {};
				EditorApplication.update += EditorUpdate;
			}

			public static void Create( Action action )
			{
				new EditorUpdateRunner(action);
			}

			void EditorUpdate()
			{
				EditorApplication.update -= EditorUpdate;
				try
				{
					mAction();
				}
				catch( System.Exception e )
				{
					Debug.LogError( e );
				}
			}
		}

		public static void RunOnceOnEditorUpdate( Action action )
		{
			EditorUpdateRunner.Create( action );
		}
	}
}
