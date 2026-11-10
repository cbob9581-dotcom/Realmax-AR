using System.Collections.Generic;
using UnityEngine;
// Hide only underlying drawing while a transparent keyboard is open; never resurrect page activity.
public sealed class HudModalVeil {
 readonly Dictionary<CanvasGroup,float> groups=new Dictionary<CanvasGroup,float>();readonly Dictionary<Renderer,bool> renderers=new Dictionary<Renderer,bool>();
 public void HideOthers(Transform parent,Transform except=null){Restore();foreach(Transform child in parent){if(child==except||!child.gameObject.activeSelf||child.name=="Persistent navigation and real telemetry")continue;var group=child.GetComponent<CanvasGroup>();if(!group)group=child.gameObject.AddComponent<CanvasGroup>();groups[group]=group.alpha;group.alpha=0;foreach(var renderer in child.GetComponentsInChildren<Renderer>()){renderers[renderer]=renderer.enabled;renderer.enabled=false;}}}
 public void Restore(){foreach(var pair in groups)if(pair.Key)pair.Key.alpha=pair.Value;groups.Clear();foreach(var pair in renderers)if(pair.Key)pair.Key.enabled=pair.Value;renderers.Clear();}
}
