void MyFunc_float(UnityTexture2D tex, UnitySamplerState stex, float2 uv, out float4 Out)
{

    Out = SAMPLE_TEXTURE2D(tex, stex, uv);
}