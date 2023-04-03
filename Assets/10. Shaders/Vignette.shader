Shader "hw/hw10/Vignette"
{
	Properties 
	{
		_MainTex ("render texture", 2D) = "white"{}

	    _VignetteSize("Vignette Size", Float) = 1

        _VignetteStrength("Vignette Strength", Range(0,1)) = 1

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
            
            float _VignetteSize;
            float _VignetteStrength;
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

                color = tex2D(_MainTex, uv);
 
                float2 center = float2(0.5,0.5);
                float distanceToCenter = distance(uv,center) * _VignetteSize;
                

                color *=smoothstep(0.5, 0.5* (1-_VignetteStrength), distanceToCenter);//  max(0,(1-distanceToCenter));

                

                return float4(color, 1.0);
            }
            ENDCG
		}
	}
}