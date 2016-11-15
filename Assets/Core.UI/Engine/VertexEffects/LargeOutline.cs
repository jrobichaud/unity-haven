using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreEngine.UI
{
	[AddComponentMenu("UI/Effects/Large Outline",20)]
	public class LargeOutline : BaseMeshEffect
	{
		public float m_Duplications = 8;
		public float m_Radius = 3;
		public Color32 m_Color = Color.white;
		Color32 m_OldColor = Color.white;
		float m_OldRadius = 0f;
		List<UIVertex> m_BaseBuffer = new List<UIVertex>();
		List<UIVertex> m_UpdatedBuffer = new List<UIVertex>();

		public void Update()
		{
			if ( !Color32.Equals( m_Color, m_OldColor) || m_OldRadius != m_Radius )
				this.graphic.SetVerticesDirty ();

			m_OldColor = m_Color;
			m_OldRadius = m_Radius;
		}
		public override void ModifyMesh(VertexHelper vh)
		{
			if ( !IsActive() )
				return;

			vh.GetUIVertexStream( m_BaseBuffer );

			var wishedCapacity = m_BaseBuffer.Count * ( Mathf.Clamp( Mathf.RoundToInt( m_Duplications ), 0, int.MaxValue ) + 1 );
			if ( m_UpdatedBuffer.Capacity < wishedCapacity )
				m_UpdatedBuffer.Capacity = wishedCapacity;

			if ( m_Duplications > 0 && m_Radius > 0 )
			{
				for( var i = 0; i < m_Duplications; ++i )
				{
					float theta = i* 2f * Mathf.PI / m_Duplications;
					var sinTheta = Mathf.Sin( theta );
					var cosTheta =  Mathf.Cos( theta ); 

					var offset = new Vector3( m_Radius * cosTheta, m_Radius * sinTheta, 1 );

					for( var vi = 0; vi < m_BaseBuffer.Count; ++vi )
					{
						var vert = m_BaseBuffer[vi];
						vert.position = vert.position + offset;
						vert.uv1 = new Vector2(1,1);
						vert.color = m_Color;
						m_UpdatedBuffer.Add( vert );
					}					
				}
			}

			for( var vi = 0; vi < m_BaseBuffer.Count; ++vi )
			{
				var vert = m_BaseBuffer[vi];
				vert.uv1 = new Vector2(0,0);
				m_UpdatedBuffer.Add( vert );
			}
			vh.Clear();
			vh.AddUIVertexTriangleStream( m_UpdatedBuffer );
			m_UpdatedBuffer.Clear();
			m_BaseBuffer.Clear();
		}
	}
}