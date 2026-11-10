using UnityEngine;
using UnityEngine.UI;
using Realmax;
public class SpatialAction : MonoBehaviour,IControllerPointHandle {
 public System.Action action;
 public Image image; public bool textOnly;
 public Color normal=new Color(.08f,.17f,.27f,.95f);
 public void OnPress(){if(image&&!textOnly)image.color=new Color(.13f,.46f,.57f,1);}
 public void OnDeviate(){if(image&&!textOnly)image.color=normal;}
 public void OnTrigger(){Debug.Log("ARWORKSPACE spatial control activated");if(action!=null)action();}
}
