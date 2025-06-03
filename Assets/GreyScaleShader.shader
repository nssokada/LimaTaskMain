Shader "Unlit/GrayScaleShader"
{
Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Desaturation ("Desaturation Amount", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Desaturation;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float3 grayColor = float3(gray, gray, gray);
                col.rgb = lerp(col.rgb, grayColor, _Desaturation);
                return col;
            }
            ENDCG
        }
    }
}
