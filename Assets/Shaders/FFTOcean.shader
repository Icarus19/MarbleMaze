Shader "Custom/FFTOcean"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Intensity("Intensity", float) = 1
        
        [Header(Cascade 0)]
        [HideInInspector]_Displacement_c0("Displacement C0", 2D) = "black" {}
        [HideInInspector]_Derivatives_c0("Derivatives C0", 2D) = "black" {}
        [HideInInspector]_Turbulence_c0("Turbulence C0", 2D) = "white" {}
        [Header(Cascade 1)]
        [HideInInspector]_Displacement_c1("Displacement C1", 2D) = "black" {}
        [HideInInspector]_Derivatives_c1("Derivatives C1", 2D) = "black" {}
        [HideInInspector]_Turbulence_c1("Turbulence C1", 2D) = "white" {}
        [Header(Cascade 2)]
        [HideInInspector]_Displacement_c2("Displacement C2", 2D) = "black" {}
        [HideInInspector]_Derivatives_c2("Derivatives C2", 2D) = "black" {}
        [HideInInspector]_Turbulence_c2("Turbulence C2", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _Displacement_c0;
            sampler2D _Derivatives_c0;
            sampler2D _Turbulence_c0;

            sampler2D _Displacement_c1;
            sampler2D _Derivatives_c1;
            sampler2D _Turbulence_c1;

            sampler2D _Displacement_c2;
            sampler2D _Derivatives_c2;
            sampler2D _Turbulence_c2;

            float LengthScale0;
            float LengthScale1;
            float LengthScale2;

            fixed4 _Color;
            float _Intensity;

            struct VertexData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 worldUV : TEXCOORD1;
            };

            v2f vert (VertexData v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 worldUV = float4(worldPos.xz, 0.0, 0.0);
                float3 displacement = 0.0;
                float largeWaveBias = 0.0;

                displacement += tex2Dlod(_Displacement_c0, worldUV / LengthScale0);
                largeWaveBias = displacement.y; //unused
                displacement += tex2Dlod(_Displacement_c1, worldUV / LengthScale1);
                displacement += tex2Dlod(_Displacement_c2, worldUV / LengthScale2);
                v.vertex.xyz += mul(unity_WorldToObject, displacement) * _Intensity;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldUV = worldUV.xy;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col *= tex2D(_Turbulence_c0, i.worldUV);
                col *= tex2D(_Turbulence_c1, i.worldUV);
                col *= tex2D(_Turbulence_c2, i.worldUV);
                return col;
            }
            ENDCG
        }
    }
}
