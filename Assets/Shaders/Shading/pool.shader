Shader "MinecraftVSZombies2/Pool"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _ShadeTex("Shade", 2D) = "white" {}
        _CausticTex("Caustic", 2D) = "white" {}
        _CausticTex1_ST ("Caustic 1 ST", Vector) = (0,0,0,0)
        _Caustic1_Speed("Caustic 1 Speed", Int) = 0
        _CausticTex2_ST ("Caustic 2 ST", Vector) = (0,0,0,0)
        _Caustic2_Speed("Caustic 2 Speed", Int) = 0
        _WarpTile("Warp Tile", Vector) = (0,0,0,0)
        _WarpTime("Warp Time", Range(0,1)) = 0
        _Warp("Warp", Float) = 0
        _CausticTime("Caustic Time", Float) = 0
        _CausticAlpha("Caustic Alpha", Float) = 0
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
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

        HLSLINCLUDE

        float GetModifier(half2 uv)
        {
            float2 value, modifierVec;
            value.x = 1 - abs(0.5 - uv.x) * 2;
            value.y = 1 - abs(0.5 - uv.y) * 2;
            modifierVec = -(pow(value - 1, 2)) + 1;
            return modifierVec.x * modifierVec.y;
        }

        float GetCausticModifier(half2 uv)
        {
            float2 value, modifierVec;
            value.x = 1 - abs(0.5 - uv.x) * 2;
            value.y = 1 - abs(0.5 - uv.y) * 2;
            modifierVec = -(pow(value - 1, 4)) + 1;
            return modifierVec.x * modifierVec.y;
        }

        half4 Screen(half4 src, half4 dest)
        {
            src.rgb *= src.a;
            dest.rgb = 1 - (1 - src.rgb) * (1 - dest.rgb) * 2;
            return dest;
        }

        half4 PoolBlend(half4 poolColor, half4 shadeColor, half3 causticColor)
        {
            half4 result;
            result = poolColor;
            shadeColor.rgb = shadeColor.a * shadeColor.rgb + 1 - shadeColor.a;
            result.rgb = shadeColor.a * shadeColor.rgb + (1 - shadeColor.a) * result.rgb;
                    
                    

            half3 caustic = causticColor;

            result.rgb += caustic;

            return result;
        }
        ENDHLSL
            
        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                half2 lightingUV : TEXCOORD1;

                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_ShadeTex);
            SAMPLER(sampler_ShadeTex);
            TEXTURE2D(_CausticTex);
            SAMPLER(sampler_CausticTex);

            CBUFFER_START(UnityPerMaterial)
            half4 _MainTex_ST;
            float4 _CausticTex1_ST;
            float4 _CausticTex2_ST;
            float _CausticAlpha;
            int _Caustic1_Speed;
            int _Caustic2_Speed;
            float2 _WarpTile;
            float _Warp;
            CBUFFER_END
            float _CausticTime;
            half _WarpTime;
            float4 _Color;
            half4 _RendererColor;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            float2 PoolModifyVertex(half2 uv, float2 vertex, float time)
            {
                const float pi = 3.14159;
                    
                float modifier = GetModifier(uv);

                vertex.x += cos(uv.y * _WarpTile.x + time * 2 * pi) * _Warp * modifier;
                vertex.y += sin(uv.x * _WarpTile.x + time * 2 * pi) * _Warp * modifier;

                return vertex;
            }

                

            half2 PoolModifyUV(half2 uv, float time)
            {
                const float pi = 3.14159;
                float modifier = GetModifier(uv);
                uv.x += cos(uv.y * _WarpTile.x + time * 2 * pi) * _Warp * modifier;
                uv.y += sin(uv.x * _WarpTile.y + time * 2 * pi) * _Warp * modifier;

                return uv;
            }

            half3 SampleCaustic(half2 uv, float time, half2 poolUV)
            {
                half2 firstUV = uv;
                firstUV.xy *= _CausticTex1_ST.xy;
                firstUV.y += time * _Caustic1_Speed;

                half2 secondUV = uv;
                secondUV.xy *= _CausticTex2_ST.xy;
                secondUV.y -= time * _Caustic2_Speed;
                    
                    
                half3 result = SAMPLE_TEXTURE2D(_CausticTex, sampler_CausticTex, firstUV).rgb;
                half3 secondColor =  SAMPLE_TEXTURE2D(_CausticTex, sampler_CausticTex,secondUV).rgb;
                result = min(secondColor, result);

                result *= GetCausticModifier(poolUV);// * _CausticAlpha;
                result -= _CausticAlpha;

                return result;
            }
            
            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings) 0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
                o.color = v.color * _Color * _RendererColor;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                    
                float time = _WarpTime;
				half2 uv = i.uv;
                half2 poolUv = PoolModifyUV(uv, time);
                const half4 poolColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 shadeColor = SAMPLE_TEXTURE2D(_ShadeTex, sampler_ShadeTex, poolUv);
                half3 causticColor = SampleCaustic(poolUv, _CausticTime, i.uv);
                    
                half4 result = PoolBlend(poolColor, shadeColor, causticColor) * i.color;
                    
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(result.rgb, result.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }
    }
}
