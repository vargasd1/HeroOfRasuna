﻿Shader "Unlit/VideoGammaFix"
{

    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Stencil("Stencil Ref", Float) = 1
        _StencilComp("Stencil Comparison", Float) = 3
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 0
        _StencilReadMask("Stencil Read Mask", Float) = 1
        _ColorMask("Color Mask", Float) = 15
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            /*Stencil
            {
                Ref[_Stencil]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
                Comp[_StencilComp]
                Pass[_StencilOp]
            }*/

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

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

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.rgb = GammaToLinearSpace(col.rgb);
                    return col;
                }
                ENDCG
            }
        }
}