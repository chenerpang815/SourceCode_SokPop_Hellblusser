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
	#if defined(_VERTEX_COLORS)
		float4 color : VAR_COLOR;
	#endif
	float2 baseUV : VAR_BASE_UV;
	#if defined(_FLIPBOOK_BLENDING)
		float3 flipbookUVB : VAR_FLIPBOOK;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings UnlitFoliagePassVertex (Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);

	//float t0 = _Time * 200;
	//float f0 = .025;
	//float s0 = sin(t0 + (input.positionOS.y * 4)) * f0;
	//input.positionOS.x += s0;

	output.positionWS = TransformObjectToWorld(input.positionOS);

	output.positionCS_SS = TransformWorldToHClip(output.positionWS);

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

float4 UnlitFoliagePassFragment (Varyings input) : SV_TARGET
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

	float4 c = float4(base.rgb, GetFinalAlpha(base.a));

	float r0 = 12;
	float h0 = sin(((input.positionWS.y) * r0));
	float t0 = 0;
	if ( h0 < t0 )
	{
		float c0 = .05;
		c.rgb += float3(c0,c0,c0);
	}

	for ( int i = 0; i < _DecalPositionLength; i ++ )
	{
		float l = length(_DecalPositions[i].xyz - input.positionWS.xyz);
		if ( l < _DecalPositions[i].w )
		{
			float c0 = .025;
			c.rgb = float3(c0,c0,c0);
		}
	}

	return c;
}

#endif