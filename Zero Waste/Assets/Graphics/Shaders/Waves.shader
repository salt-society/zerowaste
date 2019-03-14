Shader "Custom/Unlit/Waves"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		_Tiles ("Tiles", Int) = 1
		_OffsetScale ("Offset", Vector) = (2, 2, 0, 0)
		_Amplitude ("Amplitude", Vector) = (0.05, 0.1, 0, 0)
		_Speed ("Speed", Vector) = (1, 1, 0, 0)

		[NoScaleOffset] _DeformTex("Deform Texture", 2D) = "white" {}
		_DeformOffset ("Deform Offset", Vector) = (1, 1, 0, 0)
		_WavesSize ("Waves Size", Vector) = (0.1, 0.1, 0, 0)
	}

    SubShader
    {
		Tags 
		{
			"Queue" = "Transparent"
		}

        Pass
        {
			Cull Off ZWrite Off ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            };

			sampler2D _MainTex;
			sampler2D _DeformTex;

			float _Tiles;
			float2 _OffsetScale;
			float2 _Amplitude;
			float2 _Speed;

			float2 _DeformOffset;
			float2 _WavesSize;
			
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				// Big Waves
				// float2 tiledFactor = o.uv * _Tiles;

				// float2 wavesOffset;
				// wavesOffset.x = sin(_Time.x * _Speed.x + (tiledFactor.x + tiledFactor.y) * _OffsetScale.x);
				// wavesOffset.y = cos(_Time.y * _Speed.y + (tiledFactor.x + tiledFactor.y) * _OffsetScale.y);

				// o.uv = tiledFactor + wavesOffset * _Amplitude;

				// Dancing Waves
				float2 offsetTexUV = (o.uv * (_Tiles/ 4)) * _DeformOffset;
				// offsetTexUV += _Time * 0.05;

				o.uv = offsetTexUV;
				o.uv += _Time * 0.05;
                return o;
            }


            float4 frag (v2f i) : SV_Target
            {
				float2 texBasedOffset = tex2D(_DeformTex, i.uv).rg;
				texBasedOffset = texBasedOffset * 2.0 - 1.0;

				float4 col = tex2D(_MainTex, (i.uv * (_Tiles / 4)) + texBasedOffset * _WavesSize);
				col = float4(texBasedOffset, float2(0.0, 1.0));

				return col;
            }

            ENDCG
        }
    }
}
