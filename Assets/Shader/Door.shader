Shader "Custom/DoorComposite" {
    Properties {
        _MainTex ("Wall Texture", 2D) = "white" {}
        _DoorTex ("Door Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DoorTex;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                if (i.uv.x < 0.5) {
                    float2 wallUV = float2(i.uv.x * 2.0, i.uv.y);
                    return tex2D(_MainTex, wallUV);
                } else {
                    float2 doorUV = float2((i.uv.x - 0.5) * 2.0, i.uv.y);
                    return tex2D(_DoorTex, doorUV);
                }
            }
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Diffuse"
}