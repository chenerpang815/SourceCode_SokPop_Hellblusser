// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/xBR-sharper" 
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {
            }
        _TextureSizeVector ("Texture Size", Vector) = (0,0,0,0) 
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off Fog {
                Mode off }
 
            CGPROGRAM
            
            #pragma exclude_renderers gles
 
            #pragma vertex vert
            #pragma fragment frag

            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #pragma target 3.0

			#define WINDOW_SINC 0.42
			#define SINC 0.92
			#define AR_STRENGTH 0.0

            struct v2f
            {
                float4 pos      : POSITION0;
                float2 tex0       : TEXCOORD0;
            }
;
            uniform sampler2D _MainTex;
            uniform float4 _TextureSizeVector;
            #define halfpi  1.5707963267948966192313216916398
			#define pi    3.1415926535897932384626433832795
			#define wa    (WINDOW_SINC*pi)
			#define wb    (SINC*pi)

			const static float3 Y = float3(0.299, 0.587, 0.114);
            float df(float A, float B)
			{
                return abs(A-B);
            }

			float4 resampler(float4 x)
			{
                float4 res;
                res = (x==float4(0.0, 0.0, 0.0, 0.0)) ?  float4(wa*wb,wa*wb,wa*wb,wa*wb)  :  sin(x*wa)*sin(x*wb)/(x*x);
                return res;
            }

			float d(float2 pt1, float2 pt2)
			{
                float2 v = pt2 - pt1;
                return sqrt(dot(v,v));
            }

			float3 min4(float3 a, float3 b, float3 c, float3 d)
			{
                return min(a, min(b, min(c, d)));
            }

			float3 max4(float3 a, float3 b, float3 c, float3 d)
			{
                return max(a, max(b, max(c, d)));
            }

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.tex0 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
                return o;
            }
 
            half4 frag(v2f input): COLOR
            {
                float3 color;
                float4x4 weights;
                float2 dx = float2(1.0, 0.0);
                float2 dy = float2(0.0, 1.0);
                float2 pc = input.tex0*_TextureSizeVector.xy;
                float2 tc = (floor(pc-float2(0.5,0.5))+float2(0.5,0.5));
                weights[0] = resampler(float4(d(pc, tc    -dx    -dy), d(pc, tc           -dy), d(pc, tc    +dx    -dy), d(pc, tc+2.0*dx    -dy)));
                weights[1] = resampler(float4(d(pc, tc    -dx       ), d(pc, tc              ), d(pc, tc    +dx       ), d(pc, tc+2.0*dx       )));
                weights[2] = resampler(float4(d(pc, tc    -dx    +dy), d(pc, tc           +dy), d(pc, tc    +dx    +dy), d(pc, tc+2.0*dx    +dy)));
                weights[3] = resampler(float4(d(pc, tc    -dx+2.0*dy), d(pc, tc       +2.0*dy), d(pc, tc    +dx+2.0*dy), d(pc, tc+2.0*dx+2.0*dy)));
                dx = dx/_TextureSizeVector.xy;
                dy = dy/_TextureSizeVector.xy;
                tc = tc/_TextureSizeVector.xy;
                float3 c00 = tex2D(_MainTex, tc    -dx    -dy).xyz;
                float3 c10 = tex2D(_MainTex, tc           -dy).xyz;
                float3 c20 = tex2D(_MainTex, tc    +dx    -dy).xyz;
                float3 c30 = tex2D(_MainTex, tc+2.0*dx    -dy).xyz;
                float3 c01 = tex2D(_MainTex, tc    -dx       ).xyz;
                float3 c11 = tex2D(_MainTex, tc              ).xyz;
                float3 c21 = tex2D(_MainTex, tc    +dx       ).xyz;
                float3 c31 = tex2D(_MainTex, tc+2.0*dx       ).xyz;
                float3 c02 = tex2D(_MainTex, tc    -dx    +dy).xyz;
                float3 c12 = tex2D(_MainTex, tc           +dy).xyz;
                float3 c22 = tex2D(_MainTex, tc    +dx    +dy).xyz;
                float3 c32 = tex2D(_MainTex, tc+2.0*dx    +dy).xyz;
                float3 c03 = tex2D(_MainTex, tc    -dx+2.0*dy).xyz;
                float3 c13 = tex2D(_MainTex, tc       +2.0*dy).xyz;
                float3 c23 = tex2D(_MainTex, tc    +dx+2.0*dy).xyz;
                float3 c33 = tex2D(_MainTex, tc+2.0*dx+2.0*dy).xyz;
                color = mul(weights[0], float4x3(c00, c10, c20, c30));
                color+= mul(weights[1], float4x3(c01, c11, c21, c31));
                color+= mul(weights[2], float4x3(c02, c12, c22, c32));
                color+= mul(weights[3], float4x3(c03, c13, c23, c33));
                color = color/(dot(mul(weights, float4(1,1,1,1)), 1));
                pc = input.tex0;
                c00 = tex2D(_MainTex, pc              ).xyz;
                c11 = tex2D(_MainTex, pc    +dx       ).xyz;
                c21 = tex2D(_MainTex, pc    -dx       ).xyz;
                c12 = tex2D(_MainTex, pc           +dy).xyz;
                c22 = tex2D(_MainTex, pc           -dy).xyz;
                float3 min_sample = min4(c11, c21, c12, c22);
                float3 max_sample = max4(c11, c21, c12, c22);
                min_sample = min(min_sample, c00);
                max_sample = max(max_sample, c00);
                float3 aux = color;
                color = clamp(color, min_sample, max_sample);
                color = lerp(aux, color, AR_STRENGTH);
                return float4(color, 1);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
