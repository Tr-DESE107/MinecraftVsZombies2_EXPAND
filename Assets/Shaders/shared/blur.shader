Shader "MinecraftVSZombies2/Blur"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Blur("Blur", Range(0, 1)) = 0
		_BlurSize("BlurSize", float) = 127
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        // Material Color.
        half4 _Color;
        half _Blur;
        half _BlurSize;

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            half4 color    : COLOR;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata_t IN)
        {
            v2f OUT;
            OUT.vertex = UnityObjectToClipPos(IN.vertex);
            OUT.color = IN.color * _Color;
            OUT.uv = IN.texcoord;
            return OUT;
        }
        half4 grabColor(half2 uv, half weight, half alphaWeight)
        {
            half4 color = tex2D(_MainTex, uv);
            color.rgb *= weight;
            color.a *= alphaWeight;
            return color;
        }
        half4 frag(v2f IN) : SV_Target
        {
            half2 texelSize = _MainTex_TexelSize;
            float blur = _Blur * _BlurSize;

            half4 color = half4(0.0, 0.0, 0.0, 0.0);
            const int xExtent = 2.0;
            const int yExtent = 2.0;
            const int width = xExtent * 2.0 + 1;
            const int height = yExtent * 2.0 + 1;
            const int totalGrids = width * height;
            const half alphaWeight = 1.0 / totalGrids;
            for (int x = -xExtent ; x <= xExtent; x++)
            {
                for (int y = -yExtent ; y <= yExtent; y++)
                {
                    half xDis = abs(x);
                    half yDis = abs(y);
                    half distance = sqrt(xDis * xDis + yDis * yDis);
                    half weight = distance / xExtent;
                    color += grabColor(IN.uv + half2(x * texelSize.x * blur, y * texelSize.y * blur), weight, alphaWeight);
                }
            }
            color *= IN.color;
            color *= _Color;
            return color;
        }
		ENDCG

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

        Pass
        {
            Name "BLUR"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
	}
	FallBack "Sprites/Default"
}
