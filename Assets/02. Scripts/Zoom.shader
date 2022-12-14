Shader "Custom/Zoom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
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

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float2 _Pos;
            float _ZoomIntensity;
            float _EdgeIntensity;
            float _Size;
			float4 _Color;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			fixed noise (float2 uv){
			uv *= 10;
			uv = frac(uv);
			uv += dot(uv, uv + 19.19);
			return frac((uv.x + uv.y) * uv.y);
			
            }

			
			
         
			
            fixed4 frag (Interpolators i) : SV_Target
            {
                float2 scale = float2(_ScreenParams.x / _ScreenParams.y, 1);

                float2 dir = _Pos-i.uv;
                
                float dis = length(dir * scale);

                float atZoomArea = smoothstep(_Size + _EdgeIntensity,_Size,dis );
				
                fixed4 col = 0;
				
				fixed4 target = tex2D(_MainTex, i.uv + dir * _ZoomIntensity * atZoomArea );
                col = lerp(col, target, atZoomArea);
                col = pow(col, 1);
                col = col * _Color * dis * 5; 
                col = col * noise(i.uv+ _Time.x);
                return col;
            }
            ENDCG
        }
    }
}