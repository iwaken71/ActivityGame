Shader "Colored Font Stencil" {

	Properties {
	    _FontTex ("Font Texture", 2D)					= "" { }
	    _ForegroundTex ("Foreground Color Texture", 2D)	= "" { }
	    _BackgroundTex ("Background Color Texture", 2D)	= "" { }
	}

	SubShader {
	
		Cull		Back
		AlphaTest	Greater	0
	
		Pass {
	
CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _FontTex;
			sampler2D _ForegroundTex;
			sampler2D _BackgroundTex;

			struct v2f {
			    float4  pos			: SV_POSITION;
			    float2  font_uv		: TEXCOORD0;
			    float2  color_uv	: TEXCOORD1;
			};

			float4 _MainTex_ST;

			v2f vert (appdata_full v) {

			    v2f o;

			    o.pos 		= mul (UNITY_MATRIX_MVP, v.vertex);
			    o.font_uv	= v.texcoord;
			    o.color_uv	= v.texcoord1;

			    return o;

			}

			half4 frag (v2f i) : COLOR {

			    float pixel_front	= tex2D (_FontTex,			i.font_uv).r;
			    float pixel_back	= tex2D (_FontTex,			i.font_uv).g;
			    float pixel_alpha	= tex2D (_FontTex,			i.font_uv).b;
			    
			    half4 fg_color 		= tex2D (_ForegroundTex,	i.color_uv);
			    half4 bg_color 		= tex2D (_BackgroundTex,	i.color_uv);
			    
			    return pixel_alpha * ( pixel_front * fg_color + pixel_back * bg_color );
			
			}

ENDCG

	    }
	}

	Fallback "VertexLit"

}
