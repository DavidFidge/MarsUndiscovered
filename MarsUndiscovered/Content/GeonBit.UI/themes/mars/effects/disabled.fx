Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
    float value = (color.r + color.g + color.b) / 3;
    color.r = color.g = color.b = value;
    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile ps_4_1
        PixelShaderFunction();
    }
};
