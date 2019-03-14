Shader "Custom/Image Effect/Distort Moving"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _DisplacementTex ("Displacement Texture", 2D) = "white" {}
		_Magnitude ("Magnitude", Vector) = (0, 0, 0, 0)
		_Speed ("Speed", Range(-20, 20)) = 1
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DisplacementTex;
			float2 _Magnitude;
			float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 disp = tex2D(_DisplacementTex, i.uv).rg;
				disp = ((disp * 2) - 1);

				float2 magnitude = _Magnitude;
				magnitude.x = cos(_Time.x * 0.5 + (i.uv.x + i.uv.y) * 0.5);
				magnitude.y = sin(_Time.y * 0.5 + (i.uv.y + i.uv.x) * 0.5);

				float4 col = tex2D(_MainTex, i.uv + (disp * magnitude));
				return col;
            }
            ENDCG
        }
    }
}