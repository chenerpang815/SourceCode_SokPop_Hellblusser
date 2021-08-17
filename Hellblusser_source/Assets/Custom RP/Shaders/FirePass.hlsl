#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

struct Attributes
{
	float3 positionOS : POSITION;
	float4 color : COLOR;
	#if defined(_FLIPBOOK_BLENDING)
		float4 baseUV : TEXCOORD0;
		float flipbookBlend : TEXCOORD1;
	#else
		float2 baseUV : TEXCOORD0;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float4 positionCS_SS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 positionOS : VAR_POSITION1;
	#if defined(_VERTEX_COLORS)
		float4 color : VAR_COLOR;
	#endif
	float2 baseUV : VAR_BASE_UV;
	#if defined(_FLIPBOOK_BLENDING)
		float3 flipbookUVB : VAR_FLIPBOOK;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings FirePassVertex (Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);

	float t0 = _Time * 150;
	float f0 = .375;
	float s0 = sin(t0 + (input.positionOS.y * 2)) * f0;
	input.positionOS.z += s0 * input.positionOS.y;
	input.positionOS.z += -.5 * input.positionOS.y;

	output.positionWS = TransformObjectToWorld(input.positionOS);
	output.positionCS_SS = TransformWorldToHClip(output.positionWS);
	output.positionOS = input.positionOS;

	#if defined(_VERTEX_COLORS)
		output.color = input.color;
	#endif
	output.baseUV.xy = TransformBaseUV(input.baseUV.xy);
	#if defined(_FLIPBOOK_BLENDING)
		output.flipbookUVB.xy = TransformBaseUV(input.baseUV.zw);
		output.flipbookUVB.z = input.flipbookBlend;
	#endif
	return output;
}

float4 FirePassFragment (Varyings input) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(input);
	InputConfig config = GetInputConfig(input.positionCS_SS, input.baseUV);
	
	#if defined(_VERTEX_COLORS)
		config.color = input.color;
	#endif

	#if defined(_FLIPBOOK_BLENDING)
		config.flipbookUVB = input.flipbookUVB;
		config.flipbookBlending = true;
	#endif

	#if defined(_NEAR_FADE)
		config.nearFade = true;
	#endif

	#if defined(_SOFT_PARTICLES)
		config.softParticles = true;
	#endif

	float4 base = GetBase(config);

	#if defined(_CLIPPING)
		clip(base.a - GetCutoff(config));
	#endif

	#if defined(_DISTORTION)
		float2 distortion = GetDistortion(config) * base.a;
		base.rgb = lerp(
			GetBufferColor(config.fragment, distortion).rgb, base.rgb,
			saturate(base.a - GetDistortionBlend(config))
		);
	#endif

	float off = (input.positionWS.xyz / 3) * 200;

	float3 uv0 = input.positionOS;
	float3 uv1 = input.positionOS;

	float t0 = (_Time * 100);
	float f0 = .125;
	uv0.x -= (t0 * f0);

	uv0.y += t0 * -(f0 * 1.5);

	float t1 = (_Time * 100);
	float f1 = .125;
	uv1.x -= t1 * f1;
	uv1.y += t1 * -(f1 * .5);

	uv1.x += cos((_Time * .5) + (uv1.y * .5)) * 10;

	float alphaUse = base.a;

	float4 noiseMap0 = GetNoiseMap(config,float4(uv0.xyz,1) * 1);
	float4 noiseMap1 = GetNoiseMap(config,float4(uv1.xyz,1) * 2);

	float x = 1 - abs(input.positionOS.x);
	float y = 1 - abs(input.positionOS.y);

	float value0 = ((noiseMap0.x + noiseMap0.y + noiseMap0.z) / 3) - (input.positionOS.y - 1) * 1;
	float value1 = ((noiseMap1.x + noiseMap1.y + noiseMap1.z) / 3) - (input.positionOS.y - .5) * 1;

	value0 *= x * .275;

	if ( value0 < .25 )
	{
		base.rgb = float3(1,1,1);
		alphaUse = 0;
	}

	if ( value0 > .325 )
	{
		float c0 = .125;
		base.rgb += float3(c0,c0,c0);
	}

	float4 c = float4(base.rgb,alphaUse);
	return c;
}

#endif