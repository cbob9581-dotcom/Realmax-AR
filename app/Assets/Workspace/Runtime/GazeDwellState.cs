using UnityEngine;
public sealed class GazeDwellState {
 const float Seconds=1.4f;int target;Vector3 anchor;float elapsed;bool fired,anchored;
 public float Progress{get{return target==0?0:fired?1:Mathf.Clamp01(elapsed/Seconds);}}
 public bool Step(int id,Vector3 direction,float delta){
  if(anchored&&Vector3.Angle(anchor,direction)>1.25f){fired=false;elapsed=0;target=0;anchor=direction;}
  if(id==0){target=0;elapsed=0;return false;}
  if(fired){target=id;return false;}
  if(id!=target){target=id;anchor=direction;anchored=true;elapsed=0;}
  elapsed+=Mathf.Max(0,delta);if(elapsed>=Seconds){fired=true;return true;}return false;
 }
}