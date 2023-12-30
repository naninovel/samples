Shader "Naninovel/CustomTexture"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "black" {}
        _TransitionTex("Transition Texture", 2D) = "black" {}
        _CloudsTex("Clouds Texture", 2D) = "black" {}
        _DissolveTex("Dissolve Texture", 2D) = "black" {}
        _TransitionProgress("Transition Progress", Float) = 0
        _TransitionParams("Transition Parameters", Vector) = (1,1,1,1)
        _TintColor("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            
            #include "UnityCG.cginc"

            #pragma target 2.0
            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment
            #pragma multi_compile_local _ NANINOVEL_TRANSITION_CUSTOM1 NANINOVEL_TRANSITION_CUSTOM2

            sampler2D _MainTex, _TransitionTex, _DissolveTex, _CloudsTex;
            float4 _MainTex_ST, _TransitionTex_ST;
            float _TransitionProgress, _FlipMainX;
            float2 _RandomSeed;
            fixed4 _TintColor;
            float4 _TransitionParams;

            struct VertexInput
            {
                float4 Vertex : POSITION;
                float4 Color : COLOR;
                float2 MainTexCoord : TEXCOORD0;
                float2 TransitionTexCoord : TEXCOORD1;
            };

            struct VertexOutput
            {
                float4 Vertex : SV_POSITION;
                fixed4 Color : COLOR;
                float2 MainTexCoord : TEXCOORD0;
                float2 TransitionTexCoord : TEXCOORD1;
            };

            inline float IsPositionOutsideRect(float2 position, float4 rect)
            {
                float2 isInside = step(rect.xy, position.xy) * step(position.xy, rect.zw);
                return 1.0 - isInside.x * isInside.y;
            }

            inline fixed4 Tex2DClip01(sampler2D tex, float2 uvCoord, fixed4 clipColor)
            {
                const float4 UV_RANGE = float4(0, 0, 1, 1);
                float isUVOutOfRange = IsPositionOutsideRect(uvCoord, UV_RANGE);
                return lerp(tex2D(tex, uvCoord), clipColor, isUVOutOfRange);
            }

            fixed4 ApplyTransitionEffect(sampler2D mainTex, float2 mainUV, sampler2D transitionTex, float2 transitionUV, float progress,
                float4 params, float2 randomSeed, sampler2D cloudsTex, sampler2D customTex)
            {
                const fixed4 CLIP_COLOR = fixed4(0, 0, 0, 0);
                fixed4 mainColor = Tex2DClip01(mainTex, mainUV, CLIP_COLOR);
                fixed4 transitionColor = Tex2DClip01(transitionTex, transitionUV, CLIP_COLOR);

                #ifdef NANINOVEL_TRANSITION_CUSTOM1 // CUSTOM1 transition.
                return transitionUV.x > progress ? mainColor : lerp(mainColor / progress * .1, transitionColor, progress);
                #endif

                #ifdef NANINOVEL_TRANSITION_CUSTOM2 // CUSTOM2 transition.
                return lerp(mainColor * (1.0 - progress), transitionColor * progress, progress);
                #endif

                // When no transition keywords enabled default to crossfade.
                return lerp(mainColor, transitionColor, progress);
            }

            VertexOutput ComputeVertex(VertexInput vertexInput)
            {
                VertexOutput vertexOutput;
                vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
                vertexOutput.MainTexCoord = TRANSFORM_TEX(vertexInput.MainTexCoord, _MainTex);
                vertexOutput.TransitionTexCoord = TRANSFORM_TEX(vertexInput.TransitionTexCoord, _TransitionTex);
                vertexOutput.MainTexCoord.x = lerp(vertexOutput.MainTexCoord.x, 1 - vertexOutput.MainTexCoord.x, _FlipMainX);
                vertexOutput.Color = vertexInput.Color * _TintColor;
                return vertexOutput;
            }

            fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
            {
                fixed4 color = ApplyTransitionEffect(_MainTex, vertexOutput.MainTexCoord,
                                                     _TransitionTex, vertexOutput.TransitionTexCoord,
                                                     _TransitionProgress, _TransitionParams, _RandomSeed, _CloudsTex, _DissolveTex);
                color *= vertexOutput.Color;
                return color;
            }
            
            ENDCG
        }
    }
}
