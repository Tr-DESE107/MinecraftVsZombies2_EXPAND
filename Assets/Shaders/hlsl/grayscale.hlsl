half4 Grayscale(half4 color, half3 factor)
{
    //获取灰度值
    half _texGray = dot(color.rgb, factor);
    //将灰度值分别赋予rgb三色通道
    return half4(_texGray, _texGray, _texGray, color.a);
}