using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CoreEngine.UI
{
	[AddComponentMenu("Core.UI/Vertex Effects/SpriteUV1")]
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
	public class SpriteUV1 : BaseMeshEffect
#else
	public class SpriteUV1 : BaseVertexEffect
#endif
	{
		public
#if !UNITY_5_2 && !UNITY_5_3_OR_NEWER
		override
#endif
		void ModifyVertices (List<UIVertex> verts)
		{
			if (!IsActive ())
				return;

			var image = GetComponent<RectTransform>();

			var width = image.rect.width;
			var height = image.rect.height;

			if ( width == 0 || height == 0 )
				return;

			for (int index = 0; index < verts.Count; index++)
			{
				var uiVertex = verts[index];
				var normalizedU = (uiVertex.position.x / width) + 0.5f;
				var normalizedV = (uiVertex.position.y / height) + 0.5f;
				uiVertex.uv1 = new Vector2( normalizedU, normalizedV );
				verts[index] = uiVertex;
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
