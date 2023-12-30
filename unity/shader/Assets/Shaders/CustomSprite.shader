Shader "Naninovel/CustomSprite"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        CGPROGRAM
        
        #pragma surface ComputeSurface Simple

        sampler2D _MainTex;
        uniform fixed _Emission = 0;

        struct Input
        {
            float2 uv_MainTex;
        };

        void ComputeSurface(Input input, inout SurfaceOutput output)
        {
            fixed4 color = tex2D(_MainTex, input.uv_MainTex);
            // Control brightness of the self-illuminated regions by the current emission level.
            output.Albedo = lerp(color.rgb, color.rgb * _Emission, color.a);
            output.Alpha = color.a;
            output.Emission = color.rgb * color.a * _Emission;
        }

        half4 LightingSimple(SurfaceOutput output, half3 direction, half attenuation)
        {
            half4 color;
            // Make the light only affect regions of the texture that are not self-illuminated.
            color.rgb = lerp(output.Albedo * _LightColor0.rgb, output.Albedo, output.Alpha);
            color.a = output.Alpha;
            return color;
        }
        
        ENDCG
    }
}
