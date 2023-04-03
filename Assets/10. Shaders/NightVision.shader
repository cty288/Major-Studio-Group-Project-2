Shader "hw/hw10/NightVision"
{
	Properties 
	{
		_MainTex ("render texture", 2D) = "white"{}

		_blueNoisePattern("Blue Noise pattern", 2D) = "gray" {}
        _blueNoisePixel("Blue Noise Pixel",Range(0,500)) = 100

        _lineScale("LineScale", Range(0,500)) = 100

        _colorMultiplier("Color Multiplier", Color) = (1,1,1,1)

        _noiseScale("Noise Scale", Range(0,2)) = 1

        _contrast("Contrast",Float) = 1

        _lineSpeed("Line Speed", Range(0,1)) = 0.5


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
            sampler2D _blueNoisePattern; float4 _blueNoisePattern_TexelSize;
            float _lineScale;
            float3 _colorMultiplier;
            float _noiseScale;
            float _contrast;
            float _lineSpeed;
            int _blueNoisePixel;
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

            float lines(float color, float2 uv){
               
                //uv.y += _Time.y * _lineSpeed;
                //blue noise
                float2 blueNoiseUV = (uv / _blueNoisePattern_TexelSize.zw) * 
                _MainTex_TexelSize.zw; 
                blueNoiseUV = floor(blueNoiseUV * _blueNoisePixel) / _blueNoisePixel;

                float2 center = float2(0.5,0.5);
                float distanceToCenter = pow(distance(uv,center),1.1);

                float blueNoise = tex2D(_blueNoisePattern, blueNoiseUV).r;
                blueNoise = pow(blueNoise,1/(1-distanceToCenter));

                blueNoise += (value_noise(_Time.y * uv * 500) *2) * _noiseScale;
                

                color *= blueNoise;

                //line
                float lineValue = (abs(((uv.y + _Time.y * _lineSpeed) *_lineScale) % 2) -1);
                lineValue = pow(lineValue, 0.5* (1-color));

                return lineValue * color;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float3 color = 0;
                float2 uv = i.uv;

                color = tex2D(_MainTex, uv);

                float grayscale = dot(color, float3(0.299,0.587,0.114));

                
                grayscale *= 2;
                grayscale = lines(grayscale, uv);

             
                color += (grayscale) + _colorMultiplier;
                
                color = pow(color, _contrast);
                //color = saturate(color);
                return float4(color, 1.0);
            }
            ENDCG
		}
	}
}