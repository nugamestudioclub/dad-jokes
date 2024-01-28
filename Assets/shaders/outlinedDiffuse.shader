// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outlined/Silhouette Only" {
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (0.0, 0.05)) = .005
    }
CGINCLUDE
#include "UnityCG.cginc"

struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

struct v2f {
    float3x3 pos : POSITION;
    float4 color : COLOR;
};

uniform float _Outline;
uniform float4 _OutlineColor;

v2f vert(appdata v) {
    //just make a copy of incoming vertex data but scaled according to normal direction
    v2f o;
    o.pos = UNITY_MATRIX_MVP, v.vertex;
    float3 norm = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
    float2 offset = TransformViewToProjection(norm.xy);

    o.pos.xy += offset * o.pos.x * _Outline;
    o.color = _OutlineColor;
    return o;
}
ENDCG
    SubShader {
        Tags { "Queue" = "Transparent" }

        Pass {
            Name "BASE"
            Cull Back
            Blend Zero One
            //uncomment this to hide inner details:
            //offset -8, -8

            SetTexture [_OutlineColor] {
                ConstantColor (0,0,0,0)
                Combine constant
            }
        }

        //note that a vertex shader is specified here but its using the one above
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front

            //you can chose what kind of blending mode you want for the outline
            //Blend SrcAlpha oneMinusSrcAlpha // Normal
            //Blend One One // Additive
            Blend One OneMinusDstColor // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor Zero // 2x Multiplicative

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

half4 frag(v2f i) :COLOR {
    return i.color;
}
ENDCG
        }
    }
    Fallback "Diffuse"
}
