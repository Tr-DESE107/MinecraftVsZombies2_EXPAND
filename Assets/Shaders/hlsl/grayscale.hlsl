half4 Grayscale(half4 color, half3 factor)
{
    //��ȡ�Ҷ�ֵ
    half _texGray = dot(color.rgb, factor);
    //���Ҷ�ֵ�ֱ���rgb��ɫͨ��
    return half4(_texGray, _texGray, _texGray, color.a);
}