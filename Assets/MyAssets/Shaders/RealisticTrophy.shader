Shader "Custom/RealisticTrophy"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _SpecMap ("Specular Map", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off  // Disables backface culling (for inside visibility)

        CGPROGRAM
        #pragma surface surf Standard

        #include "UnityCG.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SpecMap;
        float _Smoothness;
        float4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Sample base texture
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Sample normal map for surface detail
            half3 normalTex = tex2D(_NormalMap, IN.uv_NormalMap).rgb;
            normalTex = normalize(normalTex * 2.0 - 1.0); 

            // Detect if the surface is facing inward (inside the mesh)
            bool isInside = dot(normalTex, IN.viewDir) < 0;

            if (isInside)
            {
                // Adjust inside face appearance to be more realistic
                c.rgb *= 0.8; // Slightly darken inside
            }

            o.Normal = normalTex;
            o.Metallic = tex2D(_SpecMap, IN.uv_MainTex).r;
            o.Smoothness = _Smoothness;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Diffuse"
}
