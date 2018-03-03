// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Stencil/3DStandardMask"
{
	Properties
	{
		_Stencil ("Stencil ID", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{

			ZWrite off
			Stencil {
	            Ref [_Stencil]
	            Comp always
	            Pass replace
	        }

       		Colormask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = float4(1,0,0,1);
				return col;
			}
			ENDCG
		}
	}
}
