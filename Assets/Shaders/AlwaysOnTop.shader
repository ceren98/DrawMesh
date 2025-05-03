Shader "Custom/AlwaysOnTop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Radius ("Corner Radius", Range(0, 0.5)) = 0.1
        _Smooth ("Edge Smoothness", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Radius;
            float _Smooth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 d = abs(uv - 0.5);
                float2 rect = 0.5 - _Radius;

                float alphaMask = 1.0;

                if (d.x > rect.x && d.y > rect.y)
                {
                    float2 delta = d - rect;
                    float dist = length(delta);
                    alphaMask = 1.0 - smoothstep(_Radius - _Smooth, _Radius, dist);
                }

                fixed4 col = tex2D(_MainTex, uv) * _Color;
                col.a *= alphaMask;

                if (col.a <= 0.01) discard;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
