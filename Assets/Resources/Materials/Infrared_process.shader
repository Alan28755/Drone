Shader "Hidden/Shader/Infrared_process"

{

    HLSLINCLUDE

    #pragma target 4.5

    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes

    {

        uint vertexID : SV_VertexID;

        UNITY_VERTEX_INPUT_INSTANCE_ID

    };

    struct Varyings

    {

        float4 positionCS : SV_POSITION;

        float2 texcoord   : TEXCOORD0;

        UNITY_VERTEX_OUTPUT_STEREO

    };

    Varyings Vert(Attributes input)

    {

        Varyings output;

        UNITY_SETUP_INSTANCE_ID(input);

        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);

        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);

        return output;

    }

    // 用于控制后期处理效果的属性列表

    float _Intensity;

    TEXTURE2D_X(_InputTexture);

    float4 CustomPostProcess(Varyings input) : SV_Target

    {

        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;

        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        float grayscale=lerp(outColor, Luminance(outColor).xxx, _Intensity).r;

        float3 falseColor;

        //伪彩色映射
        falseColor.r=grayscale<0.5?0:(grayscale-0.5)/0.25;
        falseColor.r=grayscale>0.75?1:falseColor.r;

        falseColor.g=grayscale>0.25?(grayscale-0.25)/0.25:0;
        falseColor.g=grayscale>0.5?1:falseColor.g;
        falseColor.g=grayscale>0.75?(1-grayscale)/0.25:falseColor.g;

        falseColor.b=grayscale>0.25?1:grayscale/0.25;
        falseColor.b=grayscale>0.5?(0.75-grayscale)/0.25:falseColor.b;
        falseColor.b=grayscale>0.75?0:falseColor.b;

        return float4(falseColor,1);





        // return float4(lerp(outColor, Luminance(outColor).xxx, _Intensity), 1);

    }

    ENDHLSL

    SubShader

    {

        Pass

        {

            Name "GrayScale"

            ZWrite Off

            ZTest Always

            Blend Off

            Cull Off

            HLSLPROGRAM

                #pragma fragment CustomPostProcess

                #pragma vertex Vert

            ENDHLSL

        }

    }

    Fallback Off

}
