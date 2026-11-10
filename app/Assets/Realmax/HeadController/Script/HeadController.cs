using UnityEngine;
namespace Realmax {
 public class HeadController : MonoBehaviour {
  public Transform EndPoint;public ReticleAnimation ReticleAnimation;
  readonly GazeDwellState dwell=new GazeDwellState();GameObject current;float initial=2.5f;
  public float DwellProgress{get{return dwell.Progress;}}
  public bool HasTarget{get{return current;}}
  void Start(){if(EndPoint)initial=Mathf.Clamp(EndPoint.localPosition.z,1,3);}
  void Update(){
   if(!EndPoint)return;Ray ray=new Ray(transform.position,transform.forward);RaycastHit hit;IControllerPointHandle target=null;GameObject next=null;
   if(Physics.Raycast(ray,out hit,1000)){
    EndPoint.position=hit.point-ray.direction*.015f;target=hit.collider.GetComponent<IControllerPointHandle>();if(target!=null)next=hit.collider.gameObject;
   }else EndPoint.position=ray.origin+ray.direction*initial;
   EndPoint.rotation=Quaternion.LookRotation(ray.direction,transform.up);
   float distance=Vector3.Distance(transform.position,EndPoint.position);EndPoint.localScale=new Vector3(distance,distance,1);
   if(current&&current!=next){var old=current.GetComponent<IControllerPointHandle>();if(old!=null)old.OnDeviate();}
   current=next;
   if(ReticleAnimation){if(current)ReticleAnimation.OpenRotateAnimation();else ReticleAnimation.CloseRotateAnimation();}
   if(target!=null)target.OnPress();
   bool click=dwell.Step(current?current.GetInstanceID():0,ray.direction,Time.unscaledDeltaTime);
   if((click||Input.GetKeyDown((KeyCode)10))&&target!=null){var spatial=target as ISpatialPointerTarget;if(spatial!=null)spatial.TriggerFrom(ray);else target.OnTrigger();}
  }
 }
}