Shader "MinecraftVSZombies2/Nightmare Smile"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _WarpTile("Warp Tile", Vector) = (0,0,0,0)
        _WarpTime("Warp Time", Range(0,1)) = 0
        _Warp("Warp", Float) = 0
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
                fixed _WarpTime;
                float _Warp;
                

                fixed2 PoolModifyUV(fixed2 uv, float time)
                {
                    const float pi = 3.14159;
                    uv.x += cos((uv.y * _WarpTile.x + time) * 2 * pi) * _Warp;
                    uv.y += sin((uv.x * _WarpTile.y + time) * 2 * pi) * _Warp;

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
                fixed4 frag(v2f IN) : SV_Target
                {
                    float time = _WarpTime;
                    fixed2 uv = PoolModifyUV(IN.texcoord, time);
                    fixed4 poolColor = SampleSpriteTexture(uv);
                    
                    fixed4 result = poolColor * IN.color;
                    return result;
                }
            ENDCG
            }
        }
}
