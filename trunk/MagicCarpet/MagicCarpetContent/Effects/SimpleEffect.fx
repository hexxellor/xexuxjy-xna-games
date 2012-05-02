#include "Common.fx"

texture Texture;


uniform sampler TextureSampler = sampler_state
{
    Texture   = (Texture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


// TODO: add effect parameters here.

struct VertexShaderInput
{
	/*
	float4 pos : POSITION0;
    float3 normal : NORMAL;
    float4 uv : TEXCOORD0;
	*/
	float4 pos : SV_Position;
    float3 normal   : NORMAL;
    float2 uv : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.pos, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	output.pos3d = input.pos;
	output.normal = input.normal;
	output.uv = input.uv;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TextureSampler, input.uv);

	float dotResult = dot(-LightDirection, input.normal);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLight * dotResult;
	float4 light = float4(directionalComponent + AmbientLight,1);

	result *= light;

	float distanceFromCamera = length(input.pos3d - CameraPosition);
	float fogFactor = ComputeFogFactor(distanceFromCamera,input.pos3d);
	result.rgb = lerp(result.rgb,FogColor,fogFactor);

	//result = float4(1,1,0,1);


    return result;
}

technique SimpleTechnique
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}