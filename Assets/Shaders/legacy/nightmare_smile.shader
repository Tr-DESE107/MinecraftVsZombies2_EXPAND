Shader "MinecraftVSZombies2/Legacy/Nightmare Smile"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _WarpTile("Warp Tile", Vector) = (0,0,0,0)
        _WarpTime("Warp Time", Range(0,1)) = 0
        _Warp("Warp", Float) = 0

		[Header(Lighting)]
		[HideInInspector] _LightDisabled("Light Disabled", Int) = 0
		[Toggle] _BackgroundLit("Lit by Background", Int) = 0
        _LightMapSpot("Light Map Spot", 2D) = "black" {}
        _LightMapST ("Light Map ST", Vector) = (14, 10.2, 0, 0)

        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
            
        Pass
        {
            Name "Main"

            HLSLPROGRAM
            #include "UnityCG.cginc"
            #include "../hlsl/lighting.hlsl"
            #pragma vertex vert
            #pragma fragment frag
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                half2 lightUV : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            half4 _MainTex_ST;

            float2 _WarpTile;
            float _Warp;
            half _WarpTime;
            float4 _Color;
            half4 _RendererColor;

            half2 PoolModifyUV(half2 uv, float time)
            {
                const float pi = 3.14159;
                uv.x += cos((uv.y * _WarpTile.x + time) * 2 * pi) * _Warp;
                uv.y += sin((uv.x * _WarpTile.y + time) * 2 * pi) * _Warp;

                return uv;
            }
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings) 0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightUV = GetLightUV(v.vertex);
                o.color = v.color * _Color * _RendererColor;
                return o;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                    
                float time = _WarpTime;
				half2 uv = i.uv;
                half2 poolUv = PoolModifyUV(uv, time);
                const half4 poolColor = tex2D(_MainTex, poolUv);
                    
                half4 result = poolColor * i.color;
                    
                return ApplyLight(result, i.lightUV);
            }
            ENDHLSL
        }
    }
}
