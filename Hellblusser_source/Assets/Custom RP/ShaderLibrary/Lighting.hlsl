#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 IncomingLight (Surface surface, Light light, float4 screenPos)
{
	if ( light.attenuation <= 0 )
	{
		//float4 screenSpace = TransformWorldToHClip(float4(surface.position,1));

		light.attenuation += abs(sin(((screenPos.x - screenPos.y) * .5) * 1.5)) * .01;
	}
	return saturate(dot(surface.normal, light.direction) * light.attenuation) * light.color;
}

float3 GetLighting (Surface surface, BRDF brdf, Light light, float4 screenPos)
{
	return IncomingLight(surface, light,screenPos) * DirectBRDF(surface, brdf, light);
}

bool RenderingLayersOverlap (Surface surface, Light light)
{
	return (surface.renderingLayerMask & light.renderingLayerMask) != 0;
}

float3 GetLighting (Surface surfaceWS, BRDF brdf, GI gi,float4 screenPos)
{
	ShadowData shadowData = GetShadowData(surfaceWS);
	shadowData.shadowMask = gi.shadowMask;
	
	float3 color = IndirectBRDF(surfaceWS, brdf, gi.diffuse, gi.specular);
	float3 colorAdd = 0;

	for (int i = 0; i < GetDirectionalLightCount(); i++)
	{
		Light light = GetDirectionalLight(i, surfaceWS, shadowData);
		if (RenderingLayersOverlap(surfaceWS, light))
		{
			colorAdd += GetLighting(surfaceWS, brdf, light,screenPos);
		}
	}
	
	#if defined(_LIGHTS_PER_OBJECT)
		for (int j = 0; j < min(unity_LightData.y, 8); j++)
		{
			int lightIndex = unity_LightIndices[(uint)j / 4][(uint)j % 4];
			Light light = GetOtherLight(lightIndex, surfaceWS, shadowData);
			if (RenderingLayersOverlap(surfaceWS, light))
			{
				colorAdd += GetLighting(surfaceWS, brdf, light,screenPos);
			}
		}
	#else
		for (int j = 0; j < GetOtherLightCount(); j++)
		{
			Light light = GetOtherLight(j, surfaceWS, shadowData);
			if (RenderingLayersOverlap(surfaceWS, light))
			{
				colorAdd += GetLighting(surfaceWS, brdf, light,screenPos);
			}
		}
	#endif

	color += colorAdd;

	return color;
}

#endif