using UnityEngine;

namespace CoreEngine
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class RendererSortingLayer : MonoBehaviour
	{
		[SortingLayer]
		public string m_Layer = "Default";
		public int m_OrderInLayer = 0;

		Renderer m_Renderer;

		public void Awake()
		{
			m_Renderer = GetComponent<Renderer>();
		}

		public void Start()
		{
			Update();
			if ( Application.isEditor == false )
				enabled = false;
		}

		public void Update()
		{
			m_Renderer.sortingLayerName = m_Layer;
			m_Renderer.sortingOrder = m_OrderInLayer;
		}
	}
}
