using UnityEngine;
public static class WorkspacePlacement {
 public static bool NeedsRecovery(Transform view,Vector3 position){
  var delta=position-view.position;return delta.magnitude<1.8f||delta.magnitude>3.5f||Vector3.Angle(view.forward,delta)>35f;
 }
 public static bool TrackingReady(bool running,int pose){return running&&(pose&1)!=0;}
 public static Pose InFront(Transform view,float distance){
  return new Pose(view.position+view.forward*distance,Quaternion.LookRotation(view.forward,view.up));
 }
}
