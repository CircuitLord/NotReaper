Shader "Custom/FishEye"
{
	Properties
	{
		_MainTex("", 2D) = "white" {}
	_Distortion("Distortion", Range(-3, 3)) = -1
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f
	{
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	//Our Vertex Shader 
	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
		return o;
	}

	sampler2D _MainTex;
	float _Distortion;

	//Our Fragment Shader
	float4 frag(v2f i) : COLOR
	{
		// lens distortion coefficient
		float k = -0.15;

	float r2 = (i.uv.x - 0.5) * (i.uv.x - 0.5) + (i.uv.y - 0.5) * (i.uv.y - 0.5);
	float f = 0;

	//only compute the cubic distortion if necessary
	if (_Distortion == 0.0)
	{
		f = 1 + r2 * k;
	}
	else
	{
		f = 1 + r2 * (k + _Distortion * sqrt(r2));
	};

	// get the right pixel for the current position
	float x = f*(i.uv.x - 0.5) + 0.5;
	float y = f*(i.uv.y - 0.5) + 0.5;
	float3 inputDistord = tex2D(_MainTex,float2(x,y));

	return float4(inputDistord.r,inputDistord.g,inputDistord.b,1);
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}