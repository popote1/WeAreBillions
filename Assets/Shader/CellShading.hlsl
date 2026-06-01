#ifndef LIGHTING_CEL_SHADER_INCLUDED
#define LIGHTING_CEL_SHADER_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT


#ifndef SHADERGRAPH_PREVIEW
struct EdgeConstants{
    float Diffuse;
    float Specular;
    float SpecularOffset;
    float DistanceAttenuation;
    float ShadowAttenuation;
    float Rim;
    float RimOffset;
    };
struct SurfaceVariables{
    float3 normal;
    float3 view;
    float smoothness;
    float shininess;
    float rimThreshold;
    EdgeConstants ec;
};

float3 CalculateCelShading(Light l, SurfaceVariables s){
    float shadowAttenuation = smoothstep(0, s.ec.ShadowAttenuation, l.shadowAttenuation);
    float distanceAttenuation = smoothstep(0, s.ec.DistanceAttenuation, l.distanceAttenuation);
    float attenuation =l.shadowAttenuation*l.distanceAttenuation;
    float diffuse = shadowAttenuation*distanceAttenuation;
    //diffuse *= attenuation;

    float3 h = SafeNormalize(l.direction+ s.view);
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *=diffuse *s.smoothness;

    float rim =1- dot(s.view, s.normal);
    rim*= pow(diffuse, s.rimThreshold);
    diffuse = smoothstep(0, s.ec.Diffuse, diffuse);
    specular = s.smoothness*smoothstep(
        (1-s.smoothness)*s.ec.Specular+s.ec.SpecularOffset,
        s.ec.Specular+s.ec.SpecularOffset,
        specular);

    rim = s.smoothness * smoothstep(
        s.ec.Rim-0.5f*s.ec.RimOffset,
        s.ec.Rim+0.5f *s.ec.RimOffset,
        rim);

    //return attenuation;
    //return specular;
    return l.color*(diffuse+max(specular, rim));   
}
#endif

void LightingCellShaded_float(float3 normal,float3 view,float smoothness,float rimThreashold,float3 position,
        float EdgeDiffuse, float EdgeSpecular, float EdgeSpecularOffset,float EdgeDistanceAttenuation, float EdgeShadowAttenuation, float EdgeRim, float EdgeRimOffset,
    out float3 Color){

    #if defined(SHADERGRAPH_PREVIEW)
    Color = float3(0.5f,0.5f,0.5f); 
    #else
    SurfaceVariables s;
    s.normal =normalize(normal);
    s.view = view;
    s.smoothness = smoothness;
    s.shininess = exp2(10*smoothness+1);
    s.rimThreshold = rimThreashold;
    EdgeConstants ec;
    ec.Diffuse = EdgeDiffuse;
    ec.Specular = EdgeSpecular;
    ec.SpecularOffset = EdgeSpecularOffset;
    ec.DistanceAttenuation = EdgeDistanceAttenuation;
    ec.ShadowAttenuation = EdgeShadowAttenuation;
    ec.Rim = EdgeRim;
    ec.RimOffset = EdgeRimOffset;
    s.ec = ec;
   
    #if SHADOWS_SCREEN
      float4 clipPos = TransformWorldToHClip(position);
      float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
      float4 shadowCoord = TransformWorldToShadowCoord(position);
    #endif
    
    Light light = GetMainLight(shadowCoord);
    Color =  CalculateCelShading(light, s);

    int pixelLightCount = GetAdditionalLightsCount();
    for(int i = 0; i < pixelLightCount; i++)
    {
        light = GetAdditionalLight(i, position, 1);
        Color += CalculateCelShading(light, s);
    }
    
    #endif

   
}
#endif

