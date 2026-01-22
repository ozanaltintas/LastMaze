Shader "Custom/InvertColorUI"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        // İŞTE SİHİR BURADA: Renkleri ters çeviren matematik (Difference Blend)
        Blend OneMinusDstColor OneMinusSrcAlpha 
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; };
            struct v2f { float4 vertex : SV_POSITION; };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Beyaz renk döndür (Blend modu bunu ters çevirecek)
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}