Shader "MinecraftVSZombies2/Legacy/Lit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [Enum(None, 0, Front, 1, Back, 2)]
		_Cull("Cull", Int) = 1

		[Header(Lighting)]
		[Toggle(LIT)]
		_LIT("Lit", Int) = 1
		[HideInInspector] _LightDisabled("Light Disabled", Int) = 0
		[Toggle] _BackgroundLit("Lit by Background", Int) = 0
        _LightMapSpot("Light Map Spot", 2D) = "black" {}
        _LightMapST ("Light Map ST", Vector) = (14, 10.2, 0, 0)
    }
    SubShader
    {
        Tags
        { 
            "Queue" = "Transparent"
            "RenderType"="Transparent" 
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull [_Cull]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ LIT
            #include "hlsl/lighting.hlsl"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                #if LIT
                float2 lightUV : TEXCOORD1;
                #endif
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #if LIT
                o.lightUV = GetLightUV(v.vertex);
                #endif
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
                #if LIT
                col = ApplyLight(col, i.lightUV);
                #endif
                return col;
            }
            ENDHLSL
        }
    }
}