Shader "Hidden/EfficientBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        half4 _MainTex_ST;

        uniform half _Radius;

        struct v2f
        {
            half4 vertex : SV_POSITION;
            half4 texcoord : TEXCOORD0;
        };

        /*struct appdata
        {
            half4 vertex : POSITION;
            half2 texcoord: TEXCOORD0;
        }*/

        v2f vert(appdata_img v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            
            half4 offset = half2(-0.5h, 0.5h).xxyy; //-x, -y, x, y
            offset *= _MainTex_TexelSize.xyxy;
            offset *= _Radius;            
            o.texcoord = v.texcoord.xyxy + offset;

            return o;
        }

        half4 frag(v2f i) : SV_Target
        {
//            half4 o = 
//                 tex2D(_MainTex, i.texcoord.xw);
//            o += tex2D(_MainTex, i.texcoord.zw);
//            o += tex2D(_MainTex, i.texcoord.xy);
//            o += tex2D(_MainTex, i.texcoord.zy);
//            o /= 4.0;

            
            //Pray to the compiler god these will MAD
            half4 o = 
                 tex2D(_MainTex, i.texcoord.xw) / 4.0h;
            o += tex2D(_MainTex, i.texcoord.zw) / 4.0h;
            o += tex2D(_MainTex, i.texcoord.xy) / 4.0h;
            o += tex2D(_MainTex, i.texcoord.zy) / 4.0h;

            return o;
        }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag		
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
            //Crop before blur
            #pragma vertex vertCrop
            #pragma fragment frag
            
            half4 _CropRegion;
			
			half2 getNewUV(half2 oldUV)
			{
			    return lerp(_CropRegion.xy, _CropRegion.zw, oldUV);
			}
			
			v2f vertCrop(appdata_img v)
            {
                v2f o = vert(v);
                
                o.texcoord.xy = getNewUV(o.texcoord.xy);
                o.texcoord.zw = getNewUV(o.texcoord.zw);
    
                return o;
            }
            ENDCG
        }
    }

    FallBack Off
}
