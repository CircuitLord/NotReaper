Shader "Unlit/DistanceFade_Prefade"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FadeThreshold("Fade In Distance", Float) = 1.7
		_OpaqueDuration("Opaque Duration", Float) = 1
		_FadeOutThreshold("Fade Out Distance", Float) = 0.5
		_WorldPosOffset("World Position Offset", Float) = 0
		_Tint("Tint", Color) = (1,1,1,1)
		_GrayscaleStrength("Grayscale Strength", Float) = 0.5

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
				float _OpaqueDuration;
				float _FadeOutThreshold;
				float _WorldPosOffset;
				float _GrayscaleStrength;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv) * _Tint;

					float worldOffset = _WorldPosOffset - 1.5;
					float fadeThreshold = _FadeThreshold / 2;
					float opaqueDuration = 0;
					float fadeOutThreshold = _FadeOutThreshold;

					//if (_IsTimelineNote == 1f) return col;
					//if (_IsTimelineNote == 1) return col;

					float worldPos = i.worldPos.z + worldOffset;
					float fadeIn = (1.0 - smoothstep(0, fadeThreshold, worldPos)) * ((1.0 - step(fadeThreshold, worldPos)) - (1.0 - step(-opaqueDuration, worldPos)));
					float fadeOut = (1.0 - smoothstep(-opaqueDuration, -fadeOutThreshold - opaqueDuration, worldPos)) * (1.0 - step(-opaqueDuration, worldPos));

					float a = clamp(fadeIn + fadeOut, 0, 1);
					col.a *= a * 0.25;

					if (i.worldPos.z < -0.1) {
						col.rgb = lerp(col.rgb, dot(col.rgb, float3(0.3, 0.59, 0.11)), _GrayscaleStrength);
					}

					return col;
				}
				ENDCG
			}
		}
}
