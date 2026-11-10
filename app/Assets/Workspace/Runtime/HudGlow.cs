using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[DisallowMultipleComponent] public sealed class HudGlow:BaseMeshEffect {
 readonly List<UIVertex> source=new List<UIVertex>();readonly List<UIVertex> output=new List<UIVertex>();
 public override void ModifyMesh(VertexHelper helper){if(!IsActive())return;source.Clear();helper.GetUIVertexStream(source);if(source.Count==0||source.Count*17>60000)return;output.Clear();
  for(int ring=0;ring<2;ring++){float radius=ring==0?7f:2.8f,alpha=ring==0?.042f:.095f;for(int step=0;step<8;step++){float a=step*Mathf.PI/4;var offset=new Vector3(Mathf.Cos(a)*radius,Mathf.Sin(a)*radius,0);foreach(var original in source){var v=original;v.position+=offset;v.color=new Color32(12,125,255,(byte)(original.color.a*alpha));output.Add(v);}}}
  output.AddRange(source);helper.Clear();helper.AddUIVertexTriangleStream(output);
 }
}
