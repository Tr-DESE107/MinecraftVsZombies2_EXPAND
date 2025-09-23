#include "UnityCG.cginc"

float4 _LevelMapST;
float2 GetLevelUV(float4 vertex)
{
    // ��ģ�Ϳռ�����ת��������ռ�
    float2 worldPos = mul(unity_ObjectToWorld, vertex);
    worldPos.y += 2.1;
    return (worldPos.xy + _LevelMapST.zw) / _LevelMapST.xy;
}