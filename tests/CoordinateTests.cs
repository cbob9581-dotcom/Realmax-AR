using System;
class CoordinateTests {
 static void Eq(bool value,string message){if(!value)throw new Exception(message);}
 static void Map(float u,float v,int w,int h,int ex,int ey){int x,y;Eq(PanelCoordinates.TryMap(u,v,w,h,out x,out y),"Expected valid mapping");Eq(x==ex&&y==ey,"Wrong pixel mapping: "+x+","+y);}
 static int Main(){try{Map(0,1,1280,720,0,0);Map(1,0,1280,720,1279,719);Map(.5f,.5f,1280,720,640,360);Map(-1,2,1280,720,0,0);int x,y;Eq(!PanelCoordinates.TryMap(float.NaN,0,1,1,out x,out y),"NaN must fail");Eq(!PanelCoordinates.TryMap(0,0,0,1,out x,out y),"Empty texture must fail");Console.WriteLine("6 coordinate checks passed");return 0;}catch(Exception e){Console.Error.WriteLine(e.Message);return 1;}}
}
