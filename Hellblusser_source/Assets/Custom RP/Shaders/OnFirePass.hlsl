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

Varyings OnFirePassVertex (Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);

	float t0 = _Time * 200;
	float f0 = .1;
	float f1 = 10;
	float s0 = sin(t0 + (input.positionOS.y * f1)) * f0;
	float s1 = cos(t0 + (input.positionOS.y * f1)) * f0;
	input.positionOS.x += s0 * input.positionOS.y;
	input.positionOS.z += s1 * input.positionOS.y;

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

float4 OnFirePassFragment (Varyings input) : SV_TARGET
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

	float alphaUse = base.a;

	float onFire = GetOnFire(config);
	if ( onFire > 0 )
	{
		float tFac = 10;

		float t0 = (_Time * tFac);
		float f0 = 1;
		float s0 = (t0 * f0);
		float3 off0 = float3(s0,-s0,0);

		float t1 = (_Time * tFac);
		float f1 = 1;
		float s1 = (t1 * f1);
		float3 off1 = float3(s1 * .5,-s1,0);

		float3 uv0 = input.positionCS_SS.xyz;
		float3 uv1 = input.positionCS_SS.xyz;

		float scl = .005;
		float4 noiseMap0 = GetNoiseMap(config,float4((uv0.xyz * scl) + off0,1));
		float4 noiseMap1 = GetNoiseMap(config,float4((uv1.xyz * (scl * .5)) + off1,1));

		float value0 = ((noiseMap0.x + noiseMap0.y + noiseMap0.z) / 3);
		float value1 = ((noiseMap1.x + noiseMap1.y + noiseMap1.z) / 3);

		float cAdd = .0375;
		float threshold0 = .525;
		if ( value0 > threshold0 )
		{
			base.rgb = GetFireColorA(config);
		}
		if ( value1 > threshold0 )
		{
			base.rgb = GetFireColorB(config);
		}
	}

	float4 c = float4(base.rgb,alphaUse);
	return c;
}

#endif