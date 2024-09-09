Shader "Custom/WaterShader"
{
    Properties
    {
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

            sampler2D _WaveTexture;
            float4 _Color;

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (VertexData v, uint instanceID : SV_INSTANCEID)
            {
                v2f o;
                float3 displacement = tex2Dlod(_WaveTexture, float4(v.uv.xy, 0.0, 0.0)).rgb;
                v.vertex.y += (displacement.r * 0.1);

                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = UnityObjectToClipPos(p);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_WaveTexture, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
