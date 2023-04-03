Shader "hw/hw10/Glitch"
{
	Properties 
	{
		_MainTex ("render texture", 2D) = "white"{}

	    _NightVisionColor("Night Vision color", Color) = (1,1,1,1)

        _GlitchBlockSize("Glitch Block size", Range(10,30))= 10 

		_Intensity("block glitching intensity", Range(0,1)) = 1

        _RGBGlichingIntensity("RGB Gliching Intensity", Range(0,1)) = 0.5
	}
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex; float4 _MainTex_TexelSize;
            float3 _NightVisionColor;
            int _GlitchBlockSize;
            float _Intensity;
            float _RGBGlichingIntensity;
            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;  
            };
            
            float rand (float2 uv) {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }


            float value_noise (float2 uv) {
                float2 ipos = floor(uv);
                float2 fpos = frac(uv); 
                
                float o  = rand(ipos);
                float x  = rand(ipos + float2(1, 0));
                float y  = rand(ipos + float2(0, 1));
                float xy = rand(ipos + float2(1, 1));

                float2 smooth = smoothstep(0, 1, fpos);
                return lerp( lerp(o,  x, smooth.x), 
                             lerp(y, xy, smooth.x), smooth.y);
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

           

            float4 frag (Interpolators i) : SV_Target
            {
                float3 color = 0;
                float2 uv = i.uv;

                float glitchNoise = rand(floor(i.uv * _GlitchBlockSize) + _Time.y * 0.00001);

                glitchNoise = pow(glitchNoise, 8) * _Intensity * rand(floor(i.uv * 10));

                float Offset =  _RGBGlichingIntensity * rand(float2(_Time.y,213));

                float r = tex2D(_MainTex, uv + glitchNoise + Offset).r;
                float g = tex2D(_MainTex, uv).g;
                float b = tex2D(_MainTex, uv - glitchNoise - Offset).b;

              
                color = float3(r,g,b);
                //color -= _NightVisionColor;
                
                return float4(color, 1.0);
            }
            ENDCG
		}
	}
}