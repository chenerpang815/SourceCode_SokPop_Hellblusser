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

Varyings LavaPassVertex (Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);

	/*
	float t0 = _Time * 150;
	float f0 = .375;
	float s0 = sin(t0 + (input.positionOS.y * 2)) * f0;
	input.positionOS.z += s0 * input.positionOS.y;
	input.positionOS.z += -.5 * input.positionOS.y;
	*/

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

float4 LavaPassFragment (Varyings input) : SV_TARGET
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

	float t0 = (_Time * .5);
	float f0 = (.01 * _LavaFactor);
	float s0 = (t0 * f0);
	float3 off0 = float3(s0,-s0,0);

	float t1 = (_Time * .5);
	float f1 = (.005 * _LavaFactor);
	float s1 = (t1 * f1);
	float3 off1 = float3(-s1,-s1,0);

	float3 uv0 = input.positionOS;
	float3 uv1 = input.positionOS;

	float4 noiseMap0 = GetNoiseMap(config,float4(uv0.xyz + off0,1) * 100);
	float4 noiseMap1 = GetNoiseMap(config,float4(uv1.xyz + off1,1) * 40);

	float alphaUse = base.a;

	float value0 = ((noiseMap0.x + noiseMap0.y + noiseMap0.z) / 3);
	float value1 = ((noiseMap1.x + noiseMap1.y + noiseMap1.z) / 3);

	if ( value0 > .5 )
	{
		float c0 = (.125 * _LavaFactor);
		base.rgb += float3(c0,c0,c0);
	}

	if ( value1 > .5 )
	{
		float c1 = -(.05 * _LavaFactor);
		base.rgb += float3(c1,c1,c1);
	}

	float4 c = float4(base.rgb,alphaUse);
	return c;
}

#endif