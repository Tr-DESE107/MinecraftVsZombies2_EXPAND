#define FLAGS

int Color3ToFlags(half3 color)
{
    int r = round(color.r * 255);
    int g = round(color.g * 255);
    int b = round(color.b * 255);
    return b * 65536 + g * 256 + r;
}

half3 FlagsToColor3(int flags)
{
    half r = (flags & 255) / 255.0;
    half g = ((flags >> 8) & 255) / 255.0;
    half b = ((flags >> 16) & 255) / 255.0;
    return half3(r, g, b);
}