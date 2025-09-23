#include "UnityCG.cginc"

float4 _LevelMapST;
float2 GetLevelUV(float4 vertex)
{
    // 将模型空间坐标转换到世界空间
    float2 worldPos = mul(unity_ObjectToWorld, vertex);
    worldPos.y += 2.1;
    return (worldPos.xy + _LevelMapST.zw) / _LevelMapST.xy;
}