float4x4 World;
float4x4 View;
float4x4 Projection;
Texture2D Texture;
float4 Colour;

bool AlphaEnabled = true;
bool AlphaTestGreater = true;
float AlphaTestValue = 0.5f;

sampler2D texSampler = sampler_state 
{
    texture = <Texture>;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 position = input.Position;

    output.Position = mul(position, mul(World, mul(View, Projection)));
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColour = tex2D(texSampler, input.TextureCoordinates);

    if (AlphaEnabled)
        clip((textureColour.a - AlphaTestValue) * (AlphaTestGreater ? 1 : -1));

    return float4(
        Colour.r * textureColour.a,
        Colour.g * textureColour.a,
        Colour.b * textureColour.a,
        textureColour.a
    );
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_4_1 VertexShaderFunction();
        PixelShader = compile ps_4_1 PixelShaderFunction();
    }
}
