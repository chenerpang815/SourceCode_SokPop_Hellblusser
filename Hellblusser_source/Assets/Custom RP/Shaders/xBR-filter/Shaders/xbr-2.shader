// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/xBR2" 
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

			#define XBR_EDGE_STR 0.0
			#define XBR_WEIGHT 0.5
 
            struct v2f
            {
                float4 pos      : POSITION0;
                float2 tex0       : TEXCOORD0;
            }
;
            uniform sampler2D _MainTex;
            uniform float4 _TextureSizeVector;
            #define wp1  2.0
			#define wp2  1.0
			#define wp3 -1.0
			#define wp4  4.0
			#define wp5 -1.0
			#define wp6  1.0

			#define weight1 (XBR_WEIGHT*1.29633/10.0)
			#define weight2 (XBR_WEIGHT*1.75068/10.0/2.0)

			const static float3 Y = float3(.2126, .7152, .0722);
            float RGBtoYUV(float3 color)
			{
                return dot(color, Y);
            }

			float df(float A, float B)
			{
                return abs(A-B);
            }

			float d_wd(float b0, float b1, float c0, float c1, float c2, float d0, float d1, float d2, float d3, float e1, float e2, float e3, float f2, float f3)
			{
                return (wp1*(df(c1,c2) + df(c1,c0) + df(e2,e1) + df(e2,e3)) + wp2*(df(d2,d3) + df(d0,d1)) + wp3*(df(d1,d3) + df(d0,d2)) + wp4*df(d1,d2) + wp5*(df(c0,c2) + df(e1,e3)) + wp6*(df(b0,b1) + df(f2,f3)));
            }

			float hv_wd(float i1, float i2, float i3, float i4, float e1, float e2, float e3, float e4)
			{
                return ( wp4*(df(i1,i2)+df(i3,i4)) + wp1*(df(i1,e1)+df(i2,e2)+df(i3,e3)+df(i4,e4)) + wp3*(df(i1,e2)+df(i3,e4)+df(e1,i2)+df(e3,i4)));
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
                float2 g1 = float2(1.0/_TextureSizeVector.xy.x, 0.0);
                float2 g2 = float2(0.0, 1.0/_TextureSizeVector.xy.y);
                float3 P0 = tex2D(_MainTex, input.tex0     -g1    -g2).xyz;
                float3 P1 = tex2D(_MainTex, input.tex0 +2.0*g1    -g2).xyz;
                float3 P2 = tex2D(_MainTex, input.tex0     -g1+2.0*g2).xyz;
                float3 P3 = tex2D(_MainTex, input.tex0 +2.0*g1+2.0*g2).xyz;
                float3  B = tex2D(_MainTex, input.tex0    -g2).xyz;
                float3  C = tex2D(_MainTex, input.tex0 +g1-g2).xyz;
                float3  D = tex2D(_MainTex, input.tex0 -g1   ).xyz;
                float3  E = tex2D(_MainTex, input.tex0       ).xyz;
                float3  F = tex2D(_MainTex, input.tex0 +g1   ).xyz;
                float3  G = tex2D(_MainTex, input.tex0 -g1+g2).xyz;
                float3  H = tex2D(_MainTex, input.tex0    +g2).xyz;
                float3  I = tex2D(_MainTex, input.tex0 +g1+g2).xyz;
                float3 F4 = tex2D(_MainTex,input.tex0    +2.0*g1   ).xyz;
                float3 I4 = tex2D(_MainTex,input.tex0 +g2+2.0*g1   ).xyz;
                float3 H5 = tex2D(_MainTex,input.tex0 +2.0*g2      ).xyz;
                float3 I5 = tex2D(_MainTex,input.tex0 +2.0*g2+g1   ).xyz;
                float b = RGBtoYUV( B );
                float c = RGBtoYUV( C );
                float d = RGBtoYUV( D );
                float e = RGBtoYUV( E );
                float f = RGBtoYUV( F );
                float g = RGBtoYUV( G );
                float h = RGBtoYUV( H );
                float i = RGBtoYUV( I );
                float i4 = RGBtoYUV( I4 );
                float p0 = RGBtoYUV( P0 );
                float i5 = RGBtoYUV( I5 );
                float p1 = RGBtoYUV( P1 );
                float h5 = RGBtoYUV( H5 );
                float p2 = RGBtoYUV( P2 );
                float f4 = RGBtoYUV( F4 );
                float p3 = RGBtoYUV( P3 );
                float d_edge  = (d_wd( d, b, g, e, c, p2, h, f, p1, h5, i, f4, i5, i4 ) - d_wd( c, f4, b, f, i4, p0, e, i, p3, d, h, i5, g, h5 ));
                float hv_edge = (hv_wd(f, i, e, h, c, i5, b, h5) - hv_wd(e, f, h, i, d, f4, g, i4));
                float limits = XBR_EDGE_STR + 0.000001;
                float edge_strength = smoothstep(0.0, limits, abs(d_edge));
                float4 w1 = float4(-weight1, weight1+0.5, weight1+0.5, -weight1);
                float4 w2 = float4(-weight2, weight2+0.25, weight2+0.25, -weight2);
                float3 c1 = mul(w1, float4x3(P2, H, F, P1));
                float3 c2 = mul(w1, float4x3(P0, E, I, P3));
                float3 c3 = mul(w2, float4x3( D, E, F, F4) + float4x3( G, H, I, I4));
                float3 c4 = mul(w2, float4x3( C, F, I, I5) + float4x3( B, E, H, H5));
                float3 color =  lerp(lerp(c1, c2, step(0.0, d_edge)), lerp(c3, c4, step(0.0, hv_edge)), 1 - edge_strength);
                float3 min_sample = min4(  E,   F,   H,   I);
                float3 max_sample = max4(  E,   F,   H,   I);
                color = clamp(color, min_sample, max_sample);
                return float4(color, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
