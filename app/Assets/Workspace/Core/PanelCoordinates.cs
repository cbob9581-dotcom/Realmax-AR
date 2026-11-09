public static class PanelCoordinates {
 public static bool TryMap(float u,float v,int width,int height,out int x,out int y) {
  x=y=0;
  if(width<=0||height<=0||float.IsNaN(u)||float.IsNaN(v)||float.IsInfinity(u)||float.IsInfinity(v))return false;
  u=System.Math.Max(0,System.Math.Min(1,u));v=System.Math.Max(0,System.Math.Min(1,v));
  x=System.Math.Min(width-1,(int)(u*width));y=System.Math.Min(height-1,(int)((1-v)*height));return true;
 }
}
