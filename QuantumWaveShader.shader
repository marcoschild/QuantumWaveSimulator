Shader "Custom/QuantumWaveShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (0,0,1,1)
        _WaveTex ("Wave Function Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _WaveTex;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float waveIntensity = tex2D(_WaveTex, i.uv).r;
                return lerp(float4(0, 0, 1, 1), float4(1, 0, 0, 1), waveIntensity);
            }
            ENDCG
        }
    }
}
