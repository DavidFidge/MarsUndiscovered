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
    color.r = 1 * color.a * input.Color.r;
    color.g = 1 * color.a * input.Color.g;
    color.b = 1 * color.a * input.Color.b;
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
