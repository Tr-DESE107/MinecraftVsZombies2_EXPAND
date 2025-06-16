Shader "MinecraftVSZombies2/Legacy/Model"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ColorOffset ("Color Offset", Color) = (1,1,1,1)
		[Toggle]
		_Grayscale("Grayscale", Int) = 0
		_GrayscaleFactor("Grayscale Factor", Vector) = (0.5,0.5,0.5,0)
        
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
            "Queue"="Transparent"
        }
        Pass 
        {
            Tags 
            { 
                "RenderType"="TransparentCutout" 
            }
            ZWrite On
            
            HLSLPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;

            v2f vert(a2v IN)
            {
                v2f OUT;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                return OUT;
            }

            half4 frag(v2f IN) : SV_Target
            {
                half4 baseColor = tex2D(_MainTex, IN.uv) * _Color;
                clip(baseColor.a - 1);
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
        Pass
        {
            Name "Main"

            ZWrite Off
            ZTest LEqual
            Cull Off
            Tags
            { 
                "RenderType"="Transparent" 
                "LightMode" = "ForwardBase" 
            }

            HLSLPROGRAM
            #include "../hlsl/entity.hlsl"
            #pragma vertex EntityVert
            #pragma fragment EntityFrag
            #pragma multi_compile LIT _
            ENDHLSL
        }
    }
}