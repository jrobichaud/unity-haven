using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreEngine.UI
{
	[AddComponentMenu("Core.UI/Vertex Effects/ShearUI")]
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
	public class ShearUI : BaseMeshEffect
#else
	public class ShearUI : BaseVertexEffect
#endif
	{
		public Vector2 m_Shear = Vector2.zero;

		public
#if !UNITY_5_2 && !UNITY_5_3_OR_NEWER
		override
#endif
		void ModifyVertices (List<UIVertex> verts)
		{
			var shearMatrix = Matrix4x4.identity;
			shearMatrix[0,1] = m_Shear.x;
			shearMatrix[1,0] = m_Shear.y;

			for (int i = 0; i < verts.Count; ++i )
			{
				var vert = verts[i];
				vert.position = shearMatrix.MultiplyPoint( vert.position );
				verts[i] = vert;
			}
		}

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
		public override void ModifyMesh(VertexHelper vh)
		{
			if ( !IsActive() )
				return;
			
			var list = new List<UIVertex>();
			vh.GetUIVertexStream( list );
			
			ModifyVertices( list );
			
			vh.Clear();
			vh.AddUIVertexTriangleStream( list );
		}
#endif
	}
}