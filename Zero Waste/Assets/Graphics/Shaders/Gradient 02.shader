Shader "Custom/Unlit/Gradient 01"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Red ("Red Intensity", Range(0, 10)) = 0
		_Green ("Green Intensity", Range(0, 10)) = 0
		_Blue ("Blue Intensity", Range(0, 10)) = 0
	}

	SubShader 
	{
		Tags 
		{
			"Queue" = "Transparent"
		}

		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D_half _MainTex;
			float _Red;
			float _Green;
			float _Blue;

			float4 frag(v2f i): SV_Target {
				float4 color = tex2D(_MainTex, i.uv) * float4(i.uv.x, i.uv.x, 0, 1);
				return color;
			}

			ENDCG
		}
	}
}
