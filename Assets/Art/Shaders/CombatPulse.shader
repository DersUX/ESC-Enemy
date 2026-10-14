Shader "EcsEnemy/Combat Pulse"
{
    Properties
    {
        [Enum(Attack,0,Impact,1)] _Impact ("Effect", Float) = 0
        _Duration ("Duration", Float) = 0.36
        _Tint ("Tint", Color) = (0.2, 1, 0.8, 1)
        _HighlightTint ("Highlight", Color) = (0.85, 1, 0.96, 1)
        _BoundsPadding ("Bounds: radius multiplier / extra world units", Vector) = (1.35, 0.25, 0, 0)
        [HideInInspector] _StartTime ("Start Time", Float) = 0
        [HideInInspector] _Radius ("Radius", Float) = 1
        [HideInInspector] _Extent ("Quad Extent", Float) = 1.6
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "CombatPulse"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float4 _HighlightTint;
                float4 _BoundsPadding;
                float _Impact;
                float _Duration;
                float _StartTime;
                float _Radius;
                float _Extent;
            CBUFFER_END

            struct Attributes { float3 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 position : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.position = input.positionOS.xy * _Extent;
                return output;
            }

            // Signed-distance masks keep thin lines smooth at different camera scales.
            float Coverage(float distance)
            {
                float aa = max(fwidth(distance), 0.0001);
                return 1.0 - smoothstep(-0.5 * aa, 0.5 * aa, distance);
            }

            void Over(inout float4 result, float3 color, float alpha)
            {
                result.rgb = color * alpha + result.rgb * (1.0 - alpha);
                result.a = alpha + result.a * (1.0 - alpha);
            }

            float Ring(float distance, float radius, float width)
            {
                return Coverage(abs(distance - radius) - width);
            }

            float Arc(float distance, float angle, float radius, float width, float start, float arcLength)
            {
                float localAngle = frac((angle - start) / TWO_PI) * TWO_PI;
                float u = saturate(localAngle / arcLength);
                float taper = sin(u * PI);
                return Ring(distance, radius, width * taper) * step(localAngle, arcLength);
            }

            float2 Polar(float angle) { return float2(cos(angle), sin(angle)); }

            void Sparks(inout float4 result, float2 position, float radius, float sparkLength,
                int count, float3 tint, float fade, float t)
            {
                [loop]
                for (int i = 0; i < count; i++)
                {
                    float angle = i * TWO_PI / count + 0.23;
                    float2 center = Polar(angle) * radius;
                    float2 along = Polar(angle + 0.3) * sparkLength * (0.7 + 0.3 * sin(i * 5.0));
                    float2 across = Polar(angle + PI * 0.5) * 0.035 * (1.0 - t);
                    float2 relative = position - center;
                    // Invert the skewed diamond basis, preserving the original spark deflection.
                    float determinant = max(along.x * across.y - along.y * across.x, 0.0000001);
                    float x = (relative.x * across.y - relative.y * across.x) / determinant;
                    float y = (along.x * relative.y - along.y * relative.x) / determinant;
                    Over(result, tint, Coverage(abs(x) + abs(y) - 1.0) * fade);
                }
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float age = _Time.y - _StartTime;
                float t = saturate(age / max(_Duration, 0.0001));
                float remaining = 1.0 - t;
                float fade = remaining * remaining;
                float distance = length(input.position);
                float angle = atan2(input.position.y, input.position.x);
                float4 result = 0;
                float3 tint = _Tint.rgb;
                float3 highlight = _HighlightTint.rgb;
                // The previous mesh supplied raw vertex colors, without sRGB conversion.
                #ifndef UNITY_COLORSPACE_GAMMA
                    tint = LinearToSRGB(tint);
                    highlight = LinearToSRGB(highlight);
                #endif

                if (_Impact > 0.5)
                {
                    float radius = _Radius * lerp(0.2, 1.0, 1.0 - remaining * remaining * remaining);
                    Over(result, tint, Ring(distance, radius, _Radius * 0.13 * remaining) * fade);
                    Sparks(result, input.position, radius, _Radius * 0.35 * remaining, 7, tint, fade, t);
                }
                else
                {
                    Over(result, tint, Ring(distance, _Radius, 0.12) * fade * 0.16);
                    Over(result, tint, Ring(distance, _Radius, 0.025) * fade * 0.65);
                    float rotation = -t * PI * 2.5;
                    float radius = _Radius * lerp(0.76, 1.0, sin(t * PI * 0.5));
                    Over(result, tint, Arc(distance, angle, radius, 0.24 * remaining, rotation, PI * 1.4) * fade * 0.3);
                    Over(result, highlight, Arc(distance, angle, radius, 0.075 * remaining, rotation, PI * 1.4) * fade);
                    Over(result, tint, Arc(distance, angle, radius * 0.78, 0.035, rotation + PI, PI * 0.85) * fade * 0.6);
                    Sparks(result, input.position, _Radius * (0.92 + t * 0.2), 0.22 * remaining, 12, tint, fade, t);
                }

                return result * step(0.0, age);
            }
            ENDHLSL
        }
    }
}
