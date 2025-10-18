Shader "Unlit/EraserShader"
{
    Properties
    {
        _MainTex ("Top Texture (被擦除的圖)", 2D) = "white" {}
        _RevealTex ("Bottom Texture (露出的圖)", 2D) = "white" {}
        _MaskTex ("Mask (渲染紋理)", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // 啟用透明混合
        ZWrite Off

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
            sampler2D _RevealTex;
            sampler2D _MaskTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 採樣三張貼圖
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                fixed4 revealColor = tex2D(_RevealTex, i.uv);
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // 使用 Mask 的 Alpha 通道來混合頂層和底層的顏色
                // Mask 的 Alpha 為 0 時，顯示 mainColor；為 1 時，顯示 revealColor
                fixed4 finalColor = lerp(mainColor, revealColor, maskColor.a);
                
                // 為了實現擦除效果，我們需要反過來
                // 我們希望在 Mask 上繪製不透明時，頂層圖片變透明
                // 所以我們用 mainColor 的 Alpha 乘以 (1 - maskColor.a)
                finalColor = mainColor;
                finalColor.a *= (1.0 - maskColor.a);

                return finalColor;
            }
            ENDCG
        }
    }
}