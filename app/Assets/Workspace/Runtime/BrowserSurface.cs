using UnityEngine;
using Realmax;
public class BrowserSurface : MonoBehaviour,IControllerPointHandle {
 public SpatialBrowserView browser;
 public void OnPress(){}
 public void OnDeviate(){}
 public void OnTrigger(){
  var m=SvrManager.Instance;if(!m||!m.head)return;
  Ray ray=new Ray(m.head.position,m.head.forward);
  var c=ControllerDataCallBack.Instance;
  if(c&&c.IsConnect){var start=GameObject.Find("RayStartPoint");var model=GameObject.Find("ConntrollerMode");if(start&&model)ray=new Ray(start.transform.position,model.transform.forward);}
  RaycastHit hit;if(Physics.Raycast(ray,out hit,30)&&hit.collider.gameObject==gameObject){int x,y;if(PanelCoordinates.TryMap(hit.textureCoord.x,hit.textureCoord.y,browser.Width,browser.Height,out x,out y))browser.Click(x,y);}
 }
}
