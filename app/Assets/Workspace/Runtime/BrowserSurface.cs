using UnityEngine;
using Realmax;
public interface ISpatialPointerTarget { void TriggerFrom(Ray ray); }
public class BrowserSurface : MonoBehaviour,IControllerPointHandle,ISpatialPointerTarget {
 public SpatialBrowserView browser;
 public void OnPress(){}
 public void OnDeviate(){}
 public void OnTrigger(){var m=SvrManager.Instance;if(m&&m.head)TriggerFrom(new Ray(m.head.position,m.head.forward));}
 public bool TryMapRay(Ray ray,int width,int height,out int x,out int y){
  x=y=0;RaycastHit hit;var collider=GetComponent<Collider>();
  return collider&&collider.Raycast(ray,out hit,1000)&&PanelCoordinates.TryMap(hit.textureCoord.x,hit.textureCoord.y,width,height,out x,out y);
 }
 public void TriggerFrom(Ray ray){int x,y;if(browser&&TryMapRay(ray,browser.Width,browser.Height,out x,out y))browser.Click(x,y);}
}