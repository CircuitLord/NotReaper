Shader "Unlit/DistanceScale"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_FadeThreshold("Fade In Distance", Float) = 1
		_ScaleFactor("ScaleFactor", Float) = 0.2

	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency 
			ZWrite Off
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float3 worldPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Tint;
				float _FadeThreshold;
				float _ScaleFactor;

				v2f vert(appdata v)
				{
					v2f o;
					
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
					float displacement = clamp(1-(_FadeThreshold - o.worldPos.z) / _FadeThreshold, 0, 1) * _ScaleFactor;
					float4 newVertexPos = float4(v.vertex * (1 + displacement));
					o.vertex = UnityObjectToClipPos(newVertexPos);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv) * _Tint;
				float a = clamp((_FadeThreshold - abs(i.worldPos.z) )/ _FadeThreshold, 0, 1);
					col.a *= a;
					return col;
				}
				ENDCG
			}
		}
}
