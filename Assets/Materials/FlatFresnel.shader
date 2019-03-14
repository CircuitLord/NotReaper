Shader "Custom/GradientFresnel"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 1
		_FadeThreshold("Fade In Distance", Float) = 1
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Opaque"
	}
		Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency 
		ZWrite Off
		Cull Back

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		half3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		float fresnel : TEXCOORD1;
		float3 worldPos : TEXCOORD2;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _FadeThreshold;
	fixed4 _Color;
	fixed4 _FresnelColor;
	fixed _FresnelBias;
	fixed _FresnelScale;
	fixed _FresnelPower;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.pos);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.worldPos = mul(unity_ObjectToWorld, v.pos);
		float3 i = normalize(float3(0,0,1));
		o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(i, v.normal), _FresnelPower);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 c = tex2D(_MainTex, i.uv) * _Color;
	float a = clamp((_FadeThreshold - abs(i.worldPos.z)) / _FadeThreshold, 0, 1);
	c.a *= a;
	_FresnelColor.a *= a;
	return lerp(c, _FresnelColor, 1 - i.fresnel);
	}
		ENDCG
	}
	}
}