Shader "Test/Warp"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle]_Horizontal("Horizontal", Int) = 1
        _Direction("Direction", Range(-1, 1)) = 0
        _Side("Side", Range(-1, 1)) = 0
        _Slope("Slope", Float) = 0
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
            Blend One OneMinusSrcAlpha

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

                float4 _MainTex_TexelSize;
                int _Horizontal;
                float _Direction;
                float _Side;
                float _Slope;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;

                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                    OUT.texcoord = IN.texcoord;

                    OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);

                    float2 uv = OUT.texcoord;
                    float4 vertex = OUT.vertex;
                    if (_Horizontal == 0)
                    {
                        float expansion = _Slope * _MainTex_TexelSize.z * 0.01;
                        float y = uv.y - (_Direction + 1) / 2;
                        float x = lerp(1 - uv.x, uv.x, (_Side + 1) / 2);
                        vertex.y += x * y * expansion;
                    }
                    else
                    {
                        float expansion = _Slope * _MainTex_TexelSize.w * 0.01;
                        float y = lerp(1 - uv.y, uv.y, (_Side + 1) / 2);
                        float x = uv.x - (_Direction + 1) / 2;
                        vertex.x += x * y * expansion;
                    }
                    
                    

                    OUT.vertex = UnityObjectToClipPos(vertex);
                    
                    OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
                    

                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed2 uv = IN.texcoord;

                    clip(uv.x);
                    clip(1- uv.x);

                    fixed4 c = SampleSpriteTexture(uv) * IN.color;
                    clip(c.a - 0.01);
                    return c;
                }
            ENDCG
            }
        }
}
