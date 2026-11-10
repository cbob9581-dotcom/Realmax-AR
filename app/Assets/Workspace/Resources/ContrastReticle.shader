Shader "Workspace/ContrastReticle" {
 Properties { _Progress("Progress",Range(0,1))=0 }
 SubShader { Tags { "Queue"="Overlay+50" "RenderType"="Transparent" "IgnoreProjector"="True" } Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
  Pass { CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;};
   float _Progress;
   output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
   float ring(float r,float a,float b,float w){return smoothstep(a-w,a+w,r)*(1-smoothstep(b-w,b+w,r));}
   fixed4 frag(output i):SV_Target {
    float2 p=(i.uv-.5)*2;float r=length(p),w=max(fwidth(r),.008);float outer=ring(r,.56,.83,w);float white=ring(r,.64,.75,w);float center=1-smoothstep(.19-w,.19+w,r);float yellow=1-smoothstep(.115-w,.115+w,r);
    float arc=ring(r,.87,.98,w);float angle=frac(atan2(p.x,p.y)/6.2831853+1);float progress=step(angle,_Progress)*step(.001,_Progress);
    float alpha=max(max(outer,center),arc*step(.001,_Progress));float3 color=white.xxx; color=lerp(color,float3(1,.9,.05),yellow);color=lerp(color,float3(.1,1,1),arc*progress);
    return fixed4(color,alpha);
   }
  ENDCG }
 }
}