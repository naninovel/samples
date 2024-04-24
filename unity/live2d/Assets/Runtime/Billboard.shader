Shader "Billboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "IGNOREPROJECTOR" = "true"
            "RenderType" = "Transparent"
            "Queue" = "Transparent+100"
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;

                float4x4 view = UNITY_MATRIX_V;
                float3 right = normalize(view._m00_m01_m02);
                float3 up = float3(0, 1, 0);
                float3 forward = normalize(view._m20_m21_m22);
                float4x4 rotation = float4x4(right, 0, up, 0, forward, 0, 0, 0, 0, 1);
                float4x4 invserse = transpose(rotation);

                float4 pos = v.vertex;
                pos = mul(invserse, pos);
                pos = mul(UNITY_MATRIX_M, pos);
                pos = mul(view, pos);
                pos = mul(UNITY_MATRIX_P, pos);

                o.vertex = pos;
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(const v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
