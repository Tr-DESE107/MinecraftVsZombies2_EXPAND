#ifndef FLAGS
#include "flags.hlsl"
#endif
#define TERRAIN

sampler2D _TerrainTexture;
int _TerrainMask;
int _TerrainFlagReference;
int _TerrainReferenceNotEqual;

int GetTerrainFlags(float2 levelUV)
{
    half4 color = tex2D(_TerrainTexture, levelUV);
    return Color3ToFlags(color.rgb);
}
void ClipTerrain(float2 levelUV)
{
    int flags = GetTerrainFlags(levelUV) & _TerrainMask;
    if (_TerrainReferenceNotEqual > 0.5)
    {
        if (flags == _TerrainFlagReference)
        {
            discard;
        }
    }
    else
    {
        if (flags != _TerrainFlagReference)
        {
            discard;
        }
    }
}
