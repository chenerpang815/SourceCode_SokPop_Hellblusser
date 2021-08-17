Shader "Postprocess/SokpopIntro"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_CircleSize("Circle Size", float) = .5
		_CircleDelta("Circle Delta", float) = .1
		_XOffset("X Offset", float) = 0
		_YOffset("Y Offset", float) = 0
		_Darkness("Darkness", float) = 0
	}
	
		SubShader
	{
		// No culling or depth
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float _CircleSize;
			float _CircleDelta;
			float _XOffset;
			float _YOffset;
			float _Darkness;

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float3 offset = float3(_XOffset, _YOffset, 0.);
				float3 pos = i.uv + offset;

				fixed4 col = tex2D(_MainTex, pos.xy);

				float asp = _ScreenParams.y / _ScreenParams.x;
				pos.y *= asp;

				float2 circlePos = i.uv + offset;
				circlePos.y *= asp;

				float dist = distance(circlePos, float2(.5, .5 * asp));
				col = col * (1. - smoothstep(_CircleSize * asp, _CircleSize * asp + _CircleDelta * asp, dist));
				col = lerp(col, float4(0.,0.,0.,0.), _Darkness);

				return col;
			}

			ENDCG
		}
	}
}
