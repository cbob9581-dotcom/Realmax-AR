using UnityEngine;
using Realmax;
public class ContrastPointer : MonoBehaviour {
 Transform anchor,quad;Material material;HeadController head;
 void Start(){
  head=GetComponent<HeadController>();var controller=GetComponent<ControllerRay>();anchor=head?head.EndPoint:(controller?controller.Reticle:null);if(!anchor)return;
  foreach(var renderer in anchor.GetComponentsInChildren<Renderer>())renderer.enabled=false;
  var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name="HighContrastPointer";g.layer=2;Destroy(g.GetComponent<Collider>());quad=g.transform;quad.SetParent(anchor,false);material=new Material(Resources.Load<Material>("ContrastReticle"));g.GetComponent<Renderer>().material=material;
 }
 void LateUpdate(){if(!quad)return;var m=SvrManager.Instance;if(!m||!m.head)return;float distance=Vector3.Distance(m.head.position,anchor.position);quad.gameObject.SetActive(distance<30);if(distance>=30)return;var scale=anchor.lossyScale;float diameter=.045f*Mathf.Max(.2f,distance);quad.localScale=new Vector3(diameter/Mathf.Max(.0001f,Mathf.Abs(scale.x)),diameter/Mathf.Max(.0001f,Mathf.Abs(scale.y)),1);quad.rotation=Quaternion.LookRotation(anchor.position-m.head.position,m.head.up);material.SetFloat("_Progress",head?head.DwellProgress:0);}
 void OnDestroy(){if(material)Destroy(material);if(quad)Destroy(quad.gameObject);}
}