Shader "CustomEffects/Blur"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _VerticalBlur;
        float _HorizontalBlur;
    
        float GaussianWeight(float offset, float sigma)
        {
            float g = 1.0f / sqrt(2.0f * 3.14159f * sigma * sigma);
            return g * exp(-(offset * offset) / (2.0f * sigma * sigma));
        }

        float4 BlurVertical (Varyings input) : SV_Target
        {
            const float BLUR_SAMPLES = 16; // 64;
            //const float BLUR_SAMPLES_RANGE = BLUR_SAMPLES / 2;
            const float BLUR_RADIUS = (BLUR_SAMPLES) / 2.0f;
            
            float3 color = 0;
            float totalWeight = 0.0f;
            
            //float blurIntensity = abs(input.texcoord.y - 0.5);
            //float blurPixels = _VerticalBlur * _ScreenParams.y * blurIntensity;

            float blurIntensity = abs(input.texcoord.y - 0.5);
            float blurPixels = _VerticalBlur * _ScreenParams.y * blurIntensity;
            float sigma = max(blurPixels / BLUR_RADIUS, 0.01f); 

            // for(float i = -BLUR_SAMPLES_RANGE; i <= BLUR_SAMPLES_RANGE; i++)
            // {
            //     float2 sampleOffset = float2(0, (blurPixels / _BlitTexture_TexelSize.w) * (i / BLUR_SAMPLES_RANGE));
            //     color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb;
            // }

            for(float i = -BLUR_RADIUS; i <= BLUR_RADIUS; i++)
            {
                float weight = GaussianWeight(i, sigma);
                float2 sampleOffset = float2(0, (blurPixels / _BlitTexture_TexelSize.w) * (i / BLUR_RADIUS));
                color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb * weight;
                totalWeight += weight;
            }
            //return float4(color.rgb / (BLUR_SAMPLES + 1), 1);
            return float4(color.rgb / totalWeight, 1);
            
        }

        float4 BlurHorizontal (Varyings input) : SV_Target
        {
            const float BLUR_SAMPLES = 16; //64;
            //const float BLUR_SAMPLES_RANGE = BLUR_SAMPLES / 2;
            const float BLUR_RADIUS = (BLUR_SAMPLES) / 2.0f;
            
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float3 color = 0;
            float totalWeight = 0.0f;

            float blurIntensity = abs(input.texcoord.x - 0.5);
            float blurPixels = _HorizontalBlur * _ScreenParams.x * blurIntensity;

            // for(float i = -BLUR_SAMPLES_RANGE; i <= BLUR_SAMPLES_RANGE; i++)
            // {
            //     float2 sampleOffset = float2((blurPixels / _BlitTexture_TexelSize.z) * (i / BLUR_SAMPLES_RANGE), 0);
            //     color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb;
            // }
            // return float4(color.rgb / (BLUR_SAMPLES + 1), 1);

            float sigma = max(blurPixels / BLUR_RADIUS, 0.01f);
        
            for(float i = -BLUR_RADIUS; i <= BLUR_RADIUS; i++)
            {
                float weight = GaussianWeight(i, sigma);
                float2 sampleOffset = float2((blurPixels / _BlitTexture_TexelSize.z) * (i / BLUR_RADIUS), 0);
                color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb * weight;
                totalWeight += weight;
            }
        
            return float4(color.rgb / totalWeight, 1);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BlurPassVertical"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurVertical
            
            ENDHLSL
        }
        
        Pass
        {
            Name "BlurPassHorizontal"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            
            ENDHLSL
        }
    }
}
