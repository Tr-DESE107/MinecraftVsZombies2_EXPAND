Shader "MinecraftVSZombies2/Geometry"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }
    CGINCLUDE
    
    #pragma target 2.0
    #pragma multi_compile_instancing
    #pragma multi_compile_local _ PIXELSNAP_ON
    #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
    #include "UnitySprites.cginc"
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

    fixed4 frag(v2f IN) : SV_Target
    {
        fixed4 result = SampleSpriteTexture(IN.texcoord) * IN.color;
        return result;
    }
    ENDCG

    SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
                "Queue" = "Geometry"
            }
            
            Lighting Off
            
            Blend SrcAlpha OneMinusSrcAlpha

            Pass{
                Tags{"LightMode" = "ForwardBase"}
                Cull Front
                ZWrite On//开启深度写入
                ColorMask 0//当ColorMask设为0时，意味着该pass不写任何颜色通道，即不会输出任何颜色。这正是我们需要的-该Pass只需要写入深度缓冲即可
            }

            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                Cull Back
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                ENDCG
            }
        }
}
