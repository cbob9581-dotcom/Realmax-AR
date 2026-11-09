using UnityEngine;
public static class WorkspacePlacement {
 public static bool TrackingReady(bool running,int pose){return running&&(pose&1)!=0;}
 public static Pose InFront(Transform view,float distance){
  return new Pose(view.position+view.forward*distance,Quaternion.LookRotation(view.forward,view.up));
 }
}
