Shader "MinecraftVSZombies2/Entity/Entity Unlit"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

		[Header(Color)]
		[Space(10)]
        _Color("Tint", Color) = (1,1,1,1)
		_ColorOffset("Color Offset", Color) = (0,0,0,0)

		[Header(Burn)]
		[Toggle(BURN_ON)]
		_Burn("Can Burn", Int) = 0
		_BurnNoise("Noise Tex", 2D) = "white"{}
		_BurnValue("Value", Range(0, 1)) = 0
		_BurnEdgeColor("Edge Color", Color) = (0,0.5,0.5,1)
		_BurnEdgeThreshold("Edge Threshold", Float) = 0.2
		_BurnFireColor("Fire Color", Color) = (0,1,1,1)
        
		[Header(Circle Tiled)]
		[Toggle(CIRCLE_TILED)]
		_CircleTiled("Circle Tiled", Int) = 0
		_CircleFill("Fill", Range(0, 1)) = 1
		[KeywordEnum(Right, Down, Left, Up)]
		_CircleStart("Start", Range(0, 3)) = 0
		[Toggle]
		_CircleClockwise("Clockwise", Int) = 0
		
		[Header(HSV)]
		[Toggle(HSV_TINT)]
		_HSVTint("HSVTint", Int) = 0

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
			"PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ HSV_TINT
            #pragma multi_compile _ CIRCLE_TILED

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

                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif

                #if BURN_ON
                float2  noiseUV        : TEXCOORD3;
                #endif

                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            float4 _Color;
            half4 _ColorOffset;
            half4 _RendererColor;
            #if BURN_ON
            TEXTURE2D(_BurnNoise);
            SAMPLER(sampler_BurnNoise);
            half _BurnValue;
            #endif

            #if CIRCLE_TILED
            half _CircleFill;
            #endif


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)

            float4 _MainTex_ST;

            #if BURN_ON
            float4 _BurnNoise_ST;
            half4 _BurnEdgeColor;
            half4 _BurnFireColor;
            float _BurnEdgeThreshold;
            #endif
        
            #if CIRCLE_TILED
            int _CircleTiled;
            int _CircleStart;
            int _CircleClockwise;
            #endif
            CBUFFER_END

            #if BURN_ON

            half4 Burn(half4 col, float2 noiseUV, float2 uv)
            {
                if (_BurnValue)
                {
                    half noise = SAMPLE_TEXTURE2D(_BurnNoise, sampler_BurnNoise, noiseUV).r - _BurnValue;

                    clip(noise);
                    if (noise <= _BurnEdgeThreshold + uv.y)
                    {
                        col.rgb = lerp(_BurnEdgeColor.rgb, col.rgb, saturate(noise / _BurnEdgeThreshold));
                    }

                    if (noise <= _BurnEdgeThreshold * 0.023)
                    {
                        col.rgb = _BurnFireColor.rgb;
                    }
                }
                return col;
            }
            #endif

            #if CIRCLE_TILED
            void CircleTile(half4 col, float2 uv)
            {
                float2 pos = uv - float2(0.5f, 0.5f);

                float clockwise = -1;
                if (_CircleClockwise)
                {
                    clockwise = 1;
                }

                float2 angles[4];
                angles[0] = float2(pos.y * clockwise, -pos.x);
                angles[1] = float2(-pos.x * clockwise, pos.y);
                angles[2] = float2(-pos.y * clockwise, pos.x);
                angles[3] = float2(-pos.x * clockwise, -pos.y);

                float ang = degrees(atan2(angles[_CircleStart].x, angles[_CircleStart].y)) + 180;
                clip(_CircleFill * 360 - ang);
            }
            #endif

            #if HSV_TINT
            float3 RGB2HSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }
            float3 HSV2RGB(float3 c)
            {
                float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6) - 3.0) - 1.0, 0, 1);
                rgb = rgb * rgb * (3.0 - 2.0 * rgb);
                return c.z * lerp(float3(1, 1, 1), rgb, c.y);
            }
            half4 Tint(half4 color, half4 tint)
            {
                float3 hsv = RGB2HSV(color.rgb);
                float3 tintHsv = RGB2HSV(tint.rgb);
                float t = hsv.y * hsv.z;
                float3 tintedHsv = float3(hsv.x + tintHsv.x, hsv.y * tintHsv.y, hsv.z * tintHsv.z);
                return half4(HSV2RGB(tintedHsv), color.a);
            }
            #else
            half4 Tint(half4 color, half4 tint)
            {
                return color * tint;
            }
            #endif

            Varyings UnlitVertex(Attributes v)
            {
                Varyings o = (Varyings) 0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
            #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
            #endif
                            
            #if BURN_ON
                o.noiseUV = TRANSFORM_TEX(v.uv, _BurnNoise);
            #endif

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 UnlitFragment(Varyings i) : SV_Target
            {
                half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                main = Tint(main, _Color);
                main = Tint(main, i.color);
                
                main.rgb = main.rgb + _ColorOffset.rgb;
                            
                #if CIRCLE_TILED
                CircleTile(main, i.uv);
                #endif

                #if BURN_ON
                main = Burn(main, i.noiseUV, i.uv);
                #endif

                return main;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
