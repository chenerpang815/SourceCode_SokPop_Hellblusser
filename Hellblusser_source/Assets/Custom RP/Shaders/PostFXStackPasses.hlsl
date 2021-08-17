#ifndef CUSTOM_POST_FX_PASSES_INCLUDED
#define CUSTOM_POST_FX_PASSES_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

TEXTURE2D(_PostFXSource);
TEXTURE2D(_PostFXSource2);

SAMPLER(sampler_PostFXSource);

TEXTURE2D(_Decal);
uniform sampler2D decal : TEXUNIT0;

float4 _PostFXSource_TexelSize;

float4x4 _ClipToView;

float2 _RipplePosition;
float _RippleAmount;

float4 GetSourceTexelSize ()
{
	return _PostFXSource_TexelSize;
}

float4 GetSource(float2 screenUV)
{
	return SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_linear_clamp, screenUV, 0);
}

float4 GetSourceBicubic (float2 screenUV) {
	return SampleTexture2DBicubic(
		TEXTURE2D_ARGS(_PostFXSource, sampler_linear_clamp), screenUV,
		_PostFXSource_TexelSize.zwxy, 1.0, 0.0
	);
}

float4 GetSource2(float2 screenUV)
{
	return SAMPLE_TEXTURE2D_LOD(_PostFXSource2, sampler_linear_clamp, screenUV, 0);
}

struct Varyings
{
	float4 positionCS : SV_POSITION;
	float2 screenUV : VAR_SCREEN_UV;
};

Varyings DefaultPassVertex (uint vertexID : SV_VertexID)
{
	Varyings output;
	output.positionCS = float4(vertexID <= 1 ? -1.0 : 3.0,vertexID == 1 ? 3.0 : -1.0,0.0, 1.0);
	output.screenUV = float2(vertexID <= 1 ? 0.0 : 2.0,vertexID == 1 ? 2.0 : 0.0);
	if (_ProjectionParams.x < 0.0)
	{
		output.screenUV.y = 1.0 - output.screenUV.y;
	}
	return output;
}

Varyings OutlinePassVertex (uint vertexID : SV_VertexID)
{
	Varyings output;
	output.positionCS = float4(vertexID <= 1 ? -1.0 : 3.0,vertexID == 1 ? 3.0 : -1.0,0.0, 1.0);
	output.screenUV = float2(vertexID <= 1 ? 0.0 : 2.0,vertexID == 1 ? 2.0 : 0.0);
	if (_ProjectionParams.x < 0.0)
	{
		output.screenUV.y = 1.0 - output.screenUV.y;
	}
	return output;
}

bool _BloomBicubicUpsampling;
float _BloomIntensity;

float4 BloomAddPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float4 highRes = GetSource2(input.screenUV);
	return float4(lowRes * _BloomIntensity + highRes.rgb, highRes.a);
}

float4 BloomHorizontalPassFragment (Varyings input) : SV_TARGET {
	float3 color = 0.0;
	float offsets[] = {
		-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0
	};
	float weights[] = {
		0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
		0.19459459, 0.12162162, 0.05405405, 0.01621622
	};
	for (int i = 0; i < 9; i++) {
		float offset = offsets[i] * 2.0 * GetSourceTexelSize().x;
		color += GetSource(input.screenUV + float2(offset, 0.0)).rgb * weights[i];
	}
	return float4(color, 1.0);
}

float4 _BloomThreshold;

float3 ApplyBloomThreshold (float3 color) {
	float brightness = Max3(color.r, color.g, color.b);
	float soft = brightness + _BloomThreshold.y;
	soft = clamp(soft, 0.0, _BloomThreshold.z);
	soft = soft * soft * _BloomThreshold.w;
	float contribution = max(soft, brightness - _BloomThreshold.x);
	contribution /= max(brightness, 0.00001);
	return color * contribution;
}

float4 BloomPrefilterPassFragment (Varyings input) : SV_TARGET {
	float3 color = ApplyBloomThreshold(GetSource(input.screenUV).rgb);
	return float4(color, 1.0);
}

float4 BloomPrefilterFirefliesPassFragment (Varyings input) : SV_TARGET {
	float3 color = 0.0;
	float weightSum = 0.0;
	float2 offsets[] = {
		float2(0.0, 0.0),
		float2(-1.0, -1.0), float2(-1.0, 1.0), float2(1.0, -1.0), float2(1.0, 1.0)
	};
	for (int i = 0; i < 5; i++) {
		float3 c =
			GetSource(input.screenUV + offsets[i] * GetSourceTexelSize().xy * 2.0).rgb;
		c = ApplyBloomThreshold(c);
		float w = 1.0 / (Luminance(c) + 1.0);
		color += c * w;
		weightSum += w;
	}
	color /= weightSum;
	return float4(color, 1.0);
}

float4 BloomScatterPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float3 highRes = GetSource2(input.screenUV).rgb;
	return float4(lerp(highRes, lowRes, _BloomIntensity), 1.0);
}

float4 BloomScatterFinalPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float4 highRes = GetSource2(input.screenUV);
	lowRes += highRes.rgb - ApplyBloomThreshold(highRes.rgb);
	return float4(lerp(highRes.rgb, lowRes, _BloomIntensity), highRes.a);
}

float4 BloomVerticalPassFragment (Varyings input) : SV_TARGET {
	float3 color = 0.0;
	float offsets[] = {
		-3.23076923, -1.38461538, 0.0, 1.38461538, 3.23076923
	};
	float weights[] = {
		0.07027027, 0.31621622, 0.22702703, 0.31621622, 0.07027027
	};
	for (int i = 0; i < 5; i++) {
		float offset = offsets[i] * GetSourceTexelSize().y;
		color += GetSource(input.screenUV + float2(0.0, offset)).rgb * weights[i];
	}
	return float4(color, 1.0);
}

float4 CopyPassFragment (Varyings input) : SV_TARGET {
	return GetSource(input.screenUV);
}

float4 _ColorAdjustments;
float4 _ColorFilter;
float4 _WhiteBalance;
float4 _SplitToningShadows, _SplitToningHighlights;
float4 _ChannelMixerRed, _ChannelMixerGreen, _ChannelMixerBlue;
float4 _SMHShadows, _SMHMidtones, _SMHHighlights, _SMHRange;

float Luminance (float3 color, bool useACES) {
	return useACES ? AcesLuminance(color) : Luminance(color);
}

float3 ColorGradePostExposure (float3 color) {
	return color * _ColorAdjustments.x;
}

float3 ColorGradeWhiteBalance (float3 color) {
	color = LinearToLMS(color);
	color *= _WhiteBalance.rgb;
	return LMSToLinear(color);
}

float3 ColorGradingContrast (float3 color, bool useACES) {
	color = useACES ? ACES_to_ACEScc(unity_to_ACES(color)) : LinearToLogC(color);
	color = (color - ACEScc_MIDGRAY) * _ColorAdjustments.y + ACEScc_MIDGRAY;
	return useACES ? ACES_to_ACEScg(ACEScc_to_ACES(color)) : LogCToLinear(color);
}

float3 ColorGradeColorFilter (float3 color) {
	return color * _ColorFilter.rgb;
}

float3 ColorGradingHueShift (float3 color) {
	color = RgbToHsv(color);
	float hue = color.x + _ColorAdjustments.z;
	color.x = RotateHue(hue, 0.0, 1.0);
	return HsvToRgb(color);
}

float3 ColorGradingSaturation (float3 color, bool useACES) {
	float luminance = Luminance(color, useACES);
	return (color - luminance) * _ColorAdjustments.w + luminance;
}

float3 ColorGradeSplitToning (float3 color, bool useACES) {
	color = PositivePow(color, 1.0 / 2.2);
	float t = saturate(Luminance(saturate(color), useACES) + _SplitToningShadows.w);
	float3 shadows = lerp(0.5, _SplitToningShadows.rgb, 1.0 - t);
	float3 highlights = lerp(0.5, _SplitToningHighlights.rgb, t);
	color = SoftLight(color, shadows);
	color = SoftLight(color, highlights);
	return PositivePow(color, 2.2);
}

float3 ColorGradingChannelMixer (float3 color) {
	return mul(
		float3x3(_ChannelMixerRed.rgb, _ChannelMixerGreen.rgb, _ChannelMixerBlue.rgb),
		color
	);
}

float3 ColorGradingShadowsMidtonesHighlights (float3 color, bool useACES) {
	float luminance = Luminance(color, useACES);
	float shadowsWeight = 1.0 - smoothstep(_SMHRange.x, _SMHRange.y, luminance);
	float highlightsWeight = smoothstep(_SMHRange.z, _SMHRange.w, luminance);
	float midtonesWeight = 1.0 - shadowsWeight - highlightsWeight;
	return
		color * _SMHShadows.rgb * shadowsWeight +
		color * _SMHMidtones.rgb * midtonesWeight +
		color * _SMHHighlights.rgb * highlightsWeight;
}

float3 ColorGrade (float3 color, bool useACES = false) {
	color = ColorGradePostExposure(color);
	color = ColorGradeWhiteBalance(color);
	color = ColorGradingContrast(color, useACES);
	color = ColorGradeColorFilter(color);
	color = max(color, 0.0);
	color =	ColorGradeSplitToning(color, useACES);
	color = ColorGradingChannelMixer(color);
	color = max(color, 0.0);
	color = ColorGradingShadowsMidtonesHighlights(color, useACES);
	color = ColorGradingHueShift(color);
	color = ColorGradingSaturation(color, useACES);
	return max(useACES ? ACEScg_to_ACES(color) : color, 0.0);
}

float4 _ColorGradingLUTParameters;

bool _ColorGradingLUTInLogC;

float3 GetColorGradedLUT (float2 uv, bool useACES = false) {
	float3 color = GetLutStripValue(uv, _ColorGradingLUTParameters);
	return ColorGrade(_ColorGradingLUTInLogC ? LogCToLinear(color) : color, useACES);
}

float4 ColorGradingNonePassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	return float4(color, 1.0);
}

float4 ColorGradingACESPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV, true);
	color = AcesTonemap(color);
	return float4(color, 1.0);
}

float4 ColorGradingNeutralPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	color = NeutralTonemap(color);
	return float4(color, 1.0);
}

float4 ColorGradingReinhardPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	color /= color + 1.0;
	return float4(color, 1.0);
}

TEXTURE2D(_ColorGradingLUT);

float3 ApplyColorGradingLUT (float3 color) {
	return ApplyLut2D(
		TEXTURE2D_ARGS(_ColorGradingLUT, sampler_linear_clamp),
		saturate(_ColorGradingLUTInLogC ? LinearToLogC(color) : color),
		_ColorGradingLUTParameters.xyz
	);
}

float4 ApplyColorGradingPassFragment (Varyings input) : SV_TARGET {
	float4 color = GetSource(input.screenUV);
	color.rgb = ApplyColorGradingLUT(color.rgb);
	return color;
}

float4 ApplyColorGradingWithLumaPassFragment (Varyings input) : SV_TARGET {
	float4 color = GetSource(input.screenUV);
	color.rgb = ApplyColorGradingLUT(color.rgb);
	color.a = sqrt(Luminance(color.rgb));
	return color;
}

bool _CopyBicubic;
float4 FinalPassFragmentRescale (Varyings input) : SV_TARGET {
	if (_CopyBicubic)
	{
		return GetSourceBicubic(input.screenUV);
	}
	else
	{
		float2 uv = input.screenUV;
		float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

		float4 color = GetSource(pointUV);
		return color;
	}
}

int _ColorClampEnabled;
int _ColorClampColorCount;
float4 ColorClampPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float4 color = GetSource(pointUV);

	if ( _ColorClampEnabled == 1 )
	{
		float h = _ColorClampColorCount;
		color.rgb = ceil(color.rgb * h) / h;
	}

	return color;
}

float4 CameraNormalsPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float4 color = GetSource(pointUV);

	return color;
}

int _EndlessFactor;
int _EndlessFilterEnabled;
float4 RippleEffectPassFragment (Varyings input) : SV_TARGET
{
	float2 uv0 = input.screenUV;

	if ( _EndlessFactor == 1 && _EndlessFilterEnabled )
	{
		float t0 = _Time * 2;
		float f0 = .005;
		float f1 = 10;
		uv0.x += sin(t0 + (uv0.y * f1)) * f0;
		uv0.y += cos(t0 + (uv0.x * f1)) * f0;
	}

	float waveamt = 20;
	float wavespeed = 400;
	float amt = _RippleAmount / 1000;
 
    float2 center = float2(_RipplePosition.x / _ScreenParams.x, _RipplePosition.y / _ScreenParams.y);
    float time = _Time * wavespeed;
    
    float2 uv = center.xy - uv0;
    uv.x *= _ScreenParams.x / _ScreenParams.y;
 
    float dist = sqrt(dot(uv,uv));
    float ang = dist * waveamt - time;
    uv = uv0 + normalize(uv) * sin(ang) * amt;

	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;
	float4 color = GetSource(pointUV);

	return color;
}

float _HurtFactor;
float _BurnFactor;
float4 HurtEffectPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float4 color = GetSource(pointUV);

	float f = _HurtFactor; 
	color.r *= 1 / f;
	color.g *= f;
	color.b *= f;

	color.b *= 1 - _BurnFactor;

	color.r = clamp(color.r,0,1);
	color.g = clamp(color.g,0,1);
	color.b = clamp(color.b,0,1);

	return color;
}

float3 ContrastSaturationBrightness ( float3 color, float brt, float sat, float con )
{
	//RGB Color Channels
	float AvgLumR = .5;
	float AvgLumG = .5;
	float AvgLumB = .5;
				
	//Luminace Coefficients for brightness of image
	float3 LuminaceCoeff = float3(.2125,.7154,.0721);
				
	//Brigntess calculations
	float3 AvgLumin = float3(AvgLumR,AvgLumG,AvgLumB);
	float3 brtColor = color * brt;
	float intensityf = dot(brtColor,LuminaceCoeff);
	float3 intensity = float3(intensityf,intensityf,intensityf);
				
	//Saturation calculation
	float3 satColor = lerp(intensity,brtColor,sat);
				
	//Contrast calculations
	float3 conColor = lerp(AvgLumin,satColor,con);

	return conColor;
}

int _PausedFactor;
float _SaturationAmount;
float _ContrastAmount;
float _BrightnessAmount;
float4 ContrastEtcPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float4 color = GetSource(pointUV);

	if ( _PausedFactor == 1 )
	{
		color.rgb = ContrastSaturationBrightness(color.rgb,_BrightnessAmount,_SaturationAmount,_ContrastAmount);
	}

	return color;
}

float _TransitionFactor;
float4 TransitionPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;

	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float asp = _ScreenParams.y / _ScreenParams.x;

	float2 offset = float2(0,0);
	float2 circlePos = pointUV + offset;
	circlePos.y *= asp;

	float4 color = GetSource(pointUV);
	float4 transitionCol = float4(0,0,0,1);

	float circleSize = _TransitionFactor;
	float circleDelta = .001;

	float dist = distance(circlePos, float2(.5, .5 * asp));
	float f = (1 - smoothstep(circleSize * asp, circleSize * asp + circleDelta * asp, dist));
	color = (f > 0) ? color : transitionCol;
	if ( circleSize <= .0125 )
	{
		color = transitionCol;
	}

	return color;
}

int _OutlineEnabled;
float _OutlineScale;
float4 _OutlineColor;
float4 OutlinePassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;
	float4 color = GetSource(pointUV);
	float4 colFinal = color;

	if ( _OutlineEnabled == 1 )
	{
		float scale = _OutlineScale;

		float halfScaleFloor = floor(scale * 0.5);
		float halfScaleCeil = ceil(scale * 0.5);

		float2 bottomLeftUV = pointUV - float2(_PostFXSource_TexelSize.x, _PostFXSource_TexelSize.y) * halfScaleFloor;
		float2 topRightUV = pointUV+ float2(_PostFXSource_TexelSize.x, _PostFXSource_TexelSize.y) * halfScaleCeil;  
		float2 bottomRightUV = pointUV + float2(_PostFXSource_TexelSize.x * halfScaleCeil, -_PostFXSource_TexelSize.y * halfScaleFloor);
		float2 topLeftUV = pointUV + float2(-_PostFXSource_TexelSize.x * halfScaleFloor, _PostFXSource_TexelSize.y * halfScaleCeil);

		// Add to the fragment shader, just below float2 topLeftUV.
		float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
		float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
		float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
		float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;

		float depthFiniteDifference0 = depth1 - depth0;
		float depthFiniteDifference1 = depth3 - depth2;

		float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;

		float depthThreshold = 30 * depth0;
		edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

		float3 normal0 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomLeftUV).rgb;
		float3 normal1 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topRightUV).rgb;
		float3 normal2 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomRightUV).rgb;
		float3 normal3 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topLeftUV).rgb;

		float3 normalFiniteDifference0 = normal1 - normal0;
		float3 normalFiniteDifference1 = normal3 - normal2;

		float normalThreshold = .05;

		float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
		edgeNormal = edgeNormal > normalThreshold ? 1 : 0;

		float edge = max(edgeDepth, edgeNormal);

		float4 outlineColor = _OutlineColor;
		if ( edge >= 1 )
		{
			colFinal = outlineColor;
		}
		else
		{
			colFinal = color;
		}
	}

	return colFinal;
}

uniform float2 _TextureSize;

float4 df(float4 A, float4 B)
{
  return float4(abs(A.x-B.x), abs(A.y-B.y), abs(A.z-B.z), abs(A.w-B.w));
}

bool float3Equal(float3 A, float3 B)
{
  return (A.x == B.x && A.y == B.y && A.z == B.z);
}

float4 weighted_distance(float4 a, float4 b, float4 c, float4 d, float4 e, float4 f, float4 g, float4 h)
{
  return (df(a,b) + df(a,c) + df(d,e) + df(d,f) + 4.0*df(g,h));
}

struct VaryingsXbr
{
	float4 positionCS : SV_POSITION;
	float2 screenUV : VAR_SCREEN_UV;
	half2 t0		 : TEXCOORD0;
	float4 t1        : TEXCOORD1;
};

VaryingsXbr xbrPassVertex ( uint vertexID : SV_VertexID )
{
	VaryingsXbr output;
	output.positionCS = float4(vertexID <= 1 ? -1.0 : 3.0,vertexID == 1 ? 3.0 : -1.0,0.0, 1.0);
	output.screenUV = float2(vertexID <= 1 ? 0.0 : 2.0,vertexID == 1 ? 2.0 : 0.0);
	if (_ProjectionParams.x < 0.0)
	{
		output.screenUV.y = 1.0 - output.screenUV.y;
	}

	float2 texel_size = _PostFXSource_TexelSize;

	half2 ps = float2(1.0/_TextureSize.x, 1.0/_TextureSize.y);
	half dx = ps.x;
	half dy = ps.y;

	float2 texCoord = output.screenUV + float2(0.0000001, 0.0000001);

    output.t0 = texCoord.xy;
	output.t1.xy = half2(dx,0);
	output.t1.zw = half2(0,dy);

	return output;
}

int _XBREnabled;
int _ContrastEtcEnabled;
float4 xbrPassFragment (VaryingsXbr input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;
	float4 color = GetSource(pointUV);

	if ( _XBREnabled == 1 )
	{
	  float coef           = 2.0;
	  half3 yuv_weighted  = half3(14.352, 28.176, 5.472);

	  bool4 edr, edr_left, edr_up, px;
	  bool4 ir_lv1, ir_lv2_left, ir_lv2_up;
	  bool4 nc;
	  bool4 fx, fx_left, fx_up;

	  float2 fp = frac(input.t0.xy * _TextureSize.xy);

	  float2 dx = input.t1.xy;
	  float2 dy = input.t1.zw;

	  half3 A = tex2D(decal,input.t0 -dx -dy).xyz;
	  half3 B = tex2D(decal,input.t0     -dy).xyz;
	  half3 C = tex2D(decal,input.t0 +dx -dy).xyz;
	  half3 D = tex2D(decal,input.t0 -dx    ).xyz;
	  half3 E = tex2D(decal,input.t0        ).xyz;
	  half3 F = tex2D(decal,input.t0 +dx    ).xyz;
	  half3 G = tex2D(decal,input.t0 -dx +dy).xyz;
	  half3 H = tex2D(decal,input.t0     +dy).xyz;
	  half3 I = tex2D(decal,input.t0 +dx +dy).xyz;

	  half3  A1 = tex2D(decal,input.t0     -dx -2.0*dy).xyz;
	  half3  C1 = tex2D(decal,input.t0     +dx -2.0*dy).xyz;
	  half3  A0 = tex2D(decal,input.t0 -2.0*dx     -dy).xyz;
	  half3  G0 = tex2D(decal,input.t0 -2.0*dx     +dy).xyz;
	  half3  C4 = tex2D(decal,input.t0 +2.0*dx     -dy).xyz;
	  half3  I4 = tex2D(decal,input.t0 +2.0*dx     +dy).xyz;
	  half3  G5 = tex2D(decal,input.t0     -dx +2.0*dy).xyz;
	  half3  I5 = tex2D(decal,input.t0     +dx +2.0*dy).xyz;
	  half3  B1 = tex2D(decal,input.t0         -2.0*dy).xyz;
	  half3  D0 = tex2D(decal,input.t0 -2.0*dx        ).xyz;
	  half3  H5 = tex2D(decal,input.t0         +2.0*dy).xyz;
	  half3  F4 = tex2D(decal,input.t0 +2.0*dx        ).xyz;

	  float4 b = mul( half4x3(B, D, H, F), yuv_weighted );
	  float4 c = mul( half4x3(C, A, G, I), yuv_weighted );
	  float4 e = mul( half4x3(E, E, E, E), yuv_weighted );
	  float4 d = b.yzwx;
	  float4 f = b.wxyz;
	  float4 g = c.zwxy;
	  float4 h = b.zwxy;
	  float4 i = c.wxyz;

	  float4 i4 = mul( half4x3(I4, C1, A0, G5), yuv_weighted );
	  float4 i5 = mul( half4x3(I5, C4, A1, G0), yuv_weighted );
	  float4 h5 = mul( half4x3(H5, F4, B1, D0), yuv_weighted );
	  float4 f4 = h5.yzwx;

	  float4 Ao = float4( 1.0, -1.0, -1.0, 1.0 );
	  float4 Bo = float4( 1.0,  1.0, -1.0,-1.0 );
	  float4 Co = float4( 1.5,  0.5, -0.5, 0.5 );
	  float4 Ax = float4( 1.0, -1.0, -1.0, 1.0 );
	  float4 Bx = float4( 0.5,  2.0, -0.5,-2.0 );
	  float4 Cx = float4( 1.0,  1.0, -0.5, 0.0 );
	  float4 Ay = float4( 1.0, -1.0, -1.0, 1.0 );
	  float4 By = float4( 2.0,  0.5, -2.0,-0.5 );
	  float4 Cy = float4( 2.0,  0.0, -1.0, 0.5 );

	  // These inequations define the line below which interpolation occurs.
	  fx.x      = (Ao.x*fp.y+Bo.x*fp.x > Co.x); 
	  fx_left.x = (Ax.x*fp.y+Bx.x*fp.x > Cx.x);
	  fx_up.x   = (Ay.x*fp.y+By.x*fp.x > Cy.x);

	  fx.y      = (Ao.y*fp.y+Bo.y*fp.x > Co.y); 
	  fx_left.y = (Ax.y*fp.y+Bx.y*fp.x > Cx.y);
	  fx_up.y   = (Ay.y*fp.y+By.y*fp.x > Cy.y);

	  fx.z      = (Ao.z*fp.y+Bo.z*fp.x > Co.z); 
	  fx_left.z = (Ax.z*fp.y+Bx.z*fp.x > Cx.z);
	  fx_up.z   = (Ay.z*fp.y+By.z*fp.x > Cy.z);

	  fx.w      = (Ao.w*fp.y+Bo.w*fp.x > Co.w); 
	  fx_left.w = (Ax.w*fp.y+Bx.w*fp.x > Cx.w);
	  fx_up.w   = (Ay.w*fp.y+By.w*fp.x > Cy.w);

	  ir_lv1.x      = ((e.x!=f.x) && (e.x!=h.x));
	  ir_lv2_left.x = ((e.x!=g.x) && (d.x!=g.x));
	  ir_lv2_up.x   = ((e.x!=c.x) && (b.x!=c.x));

	  ir_lv1.y      = ((e.y!=f.y) && (e.y!=h.y));
	  ir_lv2_left.y = ((e.y!=g.y) && (d.y!=g.y));
	  ir_lv2_up.y   = ((e.y!=c.y) && (b.y!=c.y));

	  ir_lv1.z      = ((e.z!=f.z) && (e.z!=h.z));
	  ir_lv2_left.z = ((e.z!=g.z) && (d.z!=g.z));
	  ir_lv2_up.z   = ((e.z!=c.z) && (b.z!=c.z));

	  ir_lv1.w      = ((e.w!=f.w) && (e.w!=h.w));
	  ir_lv2_left.w = ((e.w!=g.w) && (d.w!=g.w));
	  ir_lv2_up.w   = ((e.w!=c.w) && (b.w!=c.w));

	  float4 w1 = weighted_distance( e, c, g, i, h5, f4, h, f);
	  float4 w2 = weighted_distance( h, d, i5, f, i4, b, e, i);

	  float4 t1 = (coef*df(f,g));
	  float4 t2 = df(h,c);
	  float4 t3 = df(f,g);
	  float4 t4 = (coef*df(h,c));

	  edr      = bool4((w1.x<w2.x) && ir_lv1.x, (w1.y<w2.y) && ir_lv1.y, (w1.z<w2.z) && ir_lv1.z, (w1.w<w2.w) && ir_lv1.w);
	  edr_left = bool4((t1.x<=t2.x) && ir_lv2_left.x, (t1.y<=t2.y) && ir_lv2_left.y, (t1.z<=t2.z) && ir_lv2_left.z, (t1.w<=t2.w) && ir_lv2_left.w);
	  edr_up   = bool4((t4.x<=t3.x) && ir_lv2_up.x, (t4.y<=t3.y) && ir_lv2_up.y, (t4.z<=t3.z) && ir_lv2_up.z, (t4.w<=t3.w) && ir_lv2_up.w);

	  nc.x = ( edr.x && (fx.x || edr_left.x && fx_left.x || edr_up.x && fx_up.x) );
	  nc.y = ( edr.y && (fx.y || edr_left.y && fx_left.y || edr_up.y && fx_up.y) );
	  nc.z = ( edr.z && (fx.z || edr_left.z && fx_left.z || edr_up.z && fx_up.z) );
	  nc.w = ( edr.w && (fx.w || edr_left.w && fx_left.w || edr_up.w && fx_up.w) );

	  t1 = df(e,f);
	  t2 = df(e,h);

	  px = bool4(t1.x<=t2.x, t1.y<=t2.y, t1.z<=t2.z, t1.w<=t2.w);

	  half3 res = nc.x ? px.x ? F : H : nc.y ? px.y ? B : F : nc.z ? px.z ? D : B : nc.w ? px.w ? H : D : E;

	  color = half4(res.x, res.y, res.z, (float3Equal(res.xyz,float3(1,0,0))) ? 0 : 1);
	}

	if ( _ContrastEtcEnabled == 1 && _PausedFactor == 1 )
	{
		color.rgb = ContrastSaturationBrightness(color.rgb,_BrightnessAmount,_SaturationAmount,_ContrastAmount);
	}

	float f = _HurtFactor; 
	color.r *= (1 / f);
	color.g *= f;
	color.b *= f;

	color.r += (_BurnFactor * .25);
	color.g -= (_BurnFactor * .25);
	color.b -= _BurnFactor;

	if ( _EndlessFactor == 1 )
	{
		color.r *= .9;
		color.g *= .5;
		color.b *= 1.3;
	}

	return color;
}

int _ChromaticEnabled;
float _ChromaticRDir, _ChromaticGDir, _ChromaticBDir;
float _ChromaticRAmount, _ChromaticGAmount, _ChromaticBAmount;
float _ChromaticBlendFactor;
float4 ChromaticAberrationPassFragment (Varyings input) : SV_TARGET
{
	float2 uv = input.screenUV;
	float2 pointUV = (floor(uv * _PostFXSource_TexelSize.zw) + .5) * _PostFXSource_TexelSize.xy;

	float4 color = GetSource(pointUV);

	if ( _ChromaticEnabled == 1 )
	{
		float rFac = (_ChromaticRAmount * _ChromaticRDir);
		float gFac = (_ChromaticGAmount * _ChromaticGDir);
		float bFac = (_ChromaticBAmount * _ChromaticBDir);

		float colR = tex2D(decal, float2(uv.x + rFac, uv.y + rFac)).r;
		float colG = tex2D(decal, float2(uv.x + gFac, uv.y + gFac)).g;
		float colB = tex2D(decal, float2(uv.x + bFac, uv.y + bFac)).b;

		float3 targetCol = float3(colR,colG,colB);
		float3 res = lerp(color.rgb,targetCol.rgb,_ChromaticBlendFactor);
		return float4(res,color.a);
	}

	return color;
}
#endif