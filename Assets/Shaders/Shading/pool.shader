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

            Pass
            {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_instancing
                #pragma multi_compile_local _ PIXELSNAP_ON
                #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
                #include "UnitySprites.cginc"

                float2 _WarpTile;
                float _CausticTime;
                float _CausticAlpha;
                fixed _WarpTime;
                float _Warp;
                sampler2D _ShadeTex;
                sampler2D _CausticTex;
                float4 _CausticTex1_ST;
                float4 _CausticTex2_ST;
                int _Caustic1_Speed;
                int _Caustic2_Speed;

                float GetModifier(fixed2 uv)
                {
                    float2 value, modifierVec;
                    value.x = 1 - abs(0.5 - uv.x) * 2;
                    value.y = 1 - abs(0.5 - uv.y) * 2;
                    modifierVec = -(pow(value - 1, 2)) + 1;
                    return modifierVec.x * modifierVec.y;
                }

                float GetCausticModifier(fixed2 uv)
                {
                    float2 value, modifierVec;
                    value.x = 1 - abs(0.5 - uv.x) * 2;
                    value.y = 1 - abs(0.5 - uv.y) * 2;
                    modifierVec = -(pow(value - 1, 4)) + 1;
                    return modifierVec.x * modifierVec.y;
                }

                float2 PoolModifyVertex(fixed2 uv, float2 vertex, float time)
                {
                    const float pi = 3.14159;
                    
                    float modifier = GetModifier(uv);

                    vertex.x += cos(uv.y * _WarpTile.x + time * 2 * pi) * _Warp * modifier;
                    vertex.y += sin(uv.x * _WarpTile.x + time * 2 * pi) * _Warp * modifier;

                    return vertex;
                }

                

                fixed2 PoolModifyUV(fixed2 uv, float time)
                {
                    const float pi = 3.14159;
                    float modifier = GetModifier(uv);
                    uv.x += cos(uv.y * _WarpTile.x + time * 2 * pi) * _Warp * modifier;
                    uv.y += sin(uv.x * _WarpTile.y + time * 2 * pi) * _Warp * modifier;

                    return uv;
                }

                v2f vert(appdata_t IN)
                {
                    v2f OUT;

                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                    OUT.texcoord = IN.texcoord;

                    OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                    OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                    
                    OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
                    

                    return OUT;
                }

                fixed4 Screen(fixed4 src, fixed4 dest)
                {
                    src.rgb *= src.a;
                    dest.rgb = 1 - (1 - src.rgb) * (1 - dest.rgb) * 2;
                    return dest;
                }

                fixed3 SampleCaustic(sampler2D tex, fixed2 uv, float time, fixed2 poolUV)
                {
                    fixed2 firstUV = uv;
                    firstUV.xy *= _CausticTex1_ST.xy;
                    firstUV.y += time * _Caustic1_Speed;

                    fixed2 secondUV = uv;
                    secondUV.xy *= _CausticTex2_ST.xy;
                    secondUV.y -= time * _Caustic2_Speed;
                    
                    
                    fixed3 result = tex2D(tex, firstUV).rgb;
                    fixed3 secondColor = tex2D(tex, secondUV).rgb;
                    result = min(secondColor, result);

                    result *= GetCausticModifier(poolUV);// * _CausticAlpha;
                    result -= _CausticAlpha;

                    return result;
                }

                fixed4 PoolBlend(fixed4 poolColor, fixed4 shadeColor, fixed3 causticColor)
                {
                    fixed4 result;
                    result = poolColor;
                    shadeColor.rgb = shadeColor.a * shadeColor.rgb + 1 - shadeColor.a;
                    result.rgb = shadeColor.a * shadeColor.rgb + (1 - shadeColor.a) * result.rgb;
                    
                    

                    fixed3 caustic = causticColor;

                    result.rgb += caustic;
                    //result.rgb = shadeColor.rgb * result.rgb;

                    return result;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    float time = _WarpTime;
                    fixed2 uv = PoolModifyUV(IN.texcoord, time);
                    fixed4 poolColor = SampleSpriteTexture(uv);
                    fixed4 shadeColor = tex2D(_ShadeTex, uv);
                    fixed3 causticColor = SampleCaustic(_CausticTex, uv, _CausticTime, IN.texcoord);
                    
                    fixed4 result = PoolBlend(poolColor, shadeColor, causticColor) * IN.color;
                    return result;
                }
            ENDCG
            }
        }
}
