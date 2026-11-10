package com.ar.workspace;
import android.app.Activity;
import android.graphics.PixelFormat;
import org.mozilla.geckoview.MediaSession;
import android.media.Image;
import android.media.ImageReader;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.SystemClock;
import android.view.MotionEvent;
import android.util.Log;
import java.nio.ByteBuffer;
import org.mozilla.geckoview.GeckoRuntime;
import org.mozilla.geckoview.GeckoSession;
import org.mozilla.geckoview.GeckoSessionSettings;
import org.mozilla.geckoview.GeckoDisplay;
public class SpatialBrowser {
 private static GeckoRuntime runtime;
 private final Activity activity;
 private GeckoSession session;
 private GeckoDisplay display;
 private ImageReader reader;
 private HandlerThread frames;
 private final Object lock=new Object();
 private byte[] latest;
 private volatile String status="Starting GeckoView";
 private volatile boolean closed;
 private long lastFrame,downTime;
 private final int width,height;
 public SpatialBrowser(Activity a,int w,int h){activity=a;width=w;height=h;activity.runOnUiThread(this::start);}
 private void start(){try{

  android.system.Os.setenv("MOZ_DISABLE_SIG_HANDLER","1",true);
  if(runtime==null)runtime=GeckoRuntime.create(activity.getApplicationContext());
  session=new GeckoSession(new GeckoSessionSettings.Builder().usePrivateMode(false).userAgentMode(GeckoSessionSettings.USER_AGENT_MODE_DESKTOP).suspendMediaWhenInactive(true).build());
  session.setMediaSessionDelegate(new MediaSession.Delegate(){
   @Override public void onActivated(GeckoSession s,MediaSession media){media.muteAudio(true);}
   @Override public void onPlay(GeckoSession s,MediaSession media){media.muteAudio(true);}
  });
  session.setNavigationDelegate(new GeckoSession.NavigationDelegate(){
   @Override public org.mozilla.geckoview.GeckoResult<GeckoSession> onNewSession(GeckoSession s,String uri){
    if(isWebUrl(uri)){activity.runOnUiThread(()->s.loadUri(uri));status="Opening link in spatial browser";}else status="App link blocked; use the web page";
    return null;
   }
   @Override public org.mozilla.geckoview.GeckoResult<org.mozilla.geckoview.AllowOrDeny> onLoadRequest(GeckoSession s,LoadRequest request){
    boolean allow=isWebUrl(request.uri)||"about:blank".equals(request.uri);
    if(!allow)status="App link blocked; use the web page";
    return org.mozilla.geckoview.GeckoResult.fromValue(allow?org.mozilla.geckoview.AllowOrDeny.ALLOW:org.mozilla.geckoview.AllowOrDeny.DENY);
   }
   @Override public org.mozilla.geckoview.GeckoResult<String> onLoadError(GeckoSession s,String uri,org.mozilla.geckoview.WebRequestError error){status="Page load failed ("+error.code+"); check network";return null;}
  });
  session.setProgressDelegate(new GeckoSession.ProgressDelegate(){
   @Override public void onPageStart(GeckoSession s,String uri){status="Loading HTTPS page...";}
   @Override public void onPageStop(GeckoSession s,boolean success){status=success?"HTTPS page loaded":"Page load failed; check network";Log.i("ARWORKSPACE",success?"Web page load succeeded":"Web page load failed");}
  });
  session.open(runtime);
  frames=new HandlerThread("WorkspaceBrowserFrames");frames.start();
  reader=ImageReader.newInstance(width,height,PixelFormat.RGBX_8888,3);
  reader.setOnImageAvailableListener(source->{
   try(Image image=source.acquireLatestImage()){
    if(image==null||closed)return;long now=SystemClock.uptimeMillis();if(now-lastFrame<100)return;lastFrame=now;
    Image.Plane p=image.getPlanes()[0];ByteBuffer buf=p.getBuffer();int row=p.getRowStride(),stride=p.getPixelStride();byte[] pixels=new byte[width*height*4];
    for(int y=0;y<height;y++){
     if(stride==4){buf.position(y*row);buf.get(pixels,y*width*4,width*4);}
     else for(int x=0;x<width;x++)for(int c=0;c<4;c++)pixels[(y*width+x)*4+c]=buf.get(y*row+x*stride+c);
    }
    synchronized(lock){latest=pixels;}
   }catch(Exception e){status="Frame error: "+e.getClass().getSimpleName();Log.e("ARWORKSPACE",status);}
  },new Handler(frames.getLooper()));
  display=session.acquireDisplay();display.surfaceChanged(new GeckoDisplay.SurfaceInfo.Builder(reader.getSurface()).size(width,height).build());
  session.setActive(true);session.setFocused(true);session.loadUri("https://www.bilibili.com/");
  status="GeckoView 143 ready";Log.i("ARWORKSPACE",status);
 }catch(Exception e){status="Browser initialization failed: "+e.getClass().getSimpleName();Log.e("ARWORKSPACE",status,e);}}
 public byte[] takeFrame(){synchronized(lock){byte[] p=latest;latest=null;return p;}}
 public String getStatus(){return status;}
 private static boolean isWebUrl(String url){return url!=null&&(url.startsWith("https://")||url.startsWith("http://127.0.0.1:"));}
 public void load(String url){if(!isWebUrl(url)){status="HTTPS URL required";return;}activity.runOnUiThread(()->{if(session!=null)session.loadUri(url);});}
 public void typeText(String text){activity.runOnUiThread(()->{if(session!=null&&text!=null)session.getTextInput().onKeyMultiple(android.view.KeyEvent.KEYCODE_UNKNOWN,1,new android.view.KeyEvent(SystemClock.uptimeMillis(),text,android.view.KeyCharacterMap.VIRTUAL_KEYBOARD,0));});}
 public void back(){activity.runOnUiThread(()->{if(session!=null)session.goBack();});}
 public void click(int x,int y){activity.runOnUiThread(()->{if(session==null)return;long t=SystemClock.uptimeMillis();touch(t,t,MotionEvent.ACTION_DOWN,x,y);touch(t,t+40,MotionEvent.ACTION_UP,x,y);});}
 public void scroll(int direction){activity.runOnUiThread(()->{if(session==null)return;long t=SystemClock.uptimeMillis();float x=width/2f,y=height*.6f;touch(t,t,MotionEvent.ACTION_DOWN,x,y);for(int i=1;i<=6;i++)touch(t,t+i*20,MotionEvent.ACTION_MOVE,x,y+direction*i*height*.05f);touch(t,t+140,MotionEvent.ACTION_UP,x,y+direction*height*.3f);});}
 private void touch(long down,long when,int action,float x,float y){MotionEvent e=MotionEvent.obtain(down,when,action,x,y,0);session.getPanZoomController().onTouchEvent(e);e.recycle();}
 public void setActive(boolean active){activity.runOnUiThread(()->{if(session!=null)session.setActive(active);});}
 public void close(){closed=true;activity.runOnUiThread(()->{if(display!=null){display.surfaceDestroyed();session.releaseDisplay(display);display=null;}if(session!=null){session.close();session=null;}if(reader!=null){reader.close();reader=null;}if(frames!=null){frames.quitSafely();frames=null;}synchronized(lock){latest=null;}});}
}
