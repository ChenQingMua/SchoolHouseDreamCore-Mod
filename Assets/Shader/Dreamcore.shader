Shader "Custom/DreamcoreShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Chromatic ("Chromatic", Range(0, 0.02)) = 0.004
        _Noise ("Noise", Range(0, 0.2)) = 0.15
        _NoiseSize ("Noise Size", Range(50, 2000)) = 1000
        _Darkness ("Darkness", Range(0, 1)) = 0.95
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Chromatic;
            float _Noise;
            float _NoiseSize;
            float _Darkness; 

            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                
                fixed r = tex2D(_MainTex, uv + float2(_Chromatic, 0)).r;
                fixed g = tex2D(_MainTex, uv).g;
                fixed b = tex2D(_MainTex, uv - float2(_Chromatic, 0)).b;
                fixed4 col = fixed4(r, g, b, 1);

                float2 noiseUV = floor(uv * _NoiseSize) / _NoiseSize;
                
                noiseUV += _Time.y; 

                float noise = frac(sin(dot(noiseUV, float2(12.9898, 78.233))) * 43758.5453);
                
                col.rgb += (noise - 0.5) * _Noise;

                col.rgb *= _Darkness;

                return col;
            }
            ENDCG
        }
    }
}