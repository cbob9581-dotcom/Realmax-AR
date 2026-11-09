package com.unity3d.player;
import android.content.Context;
import android.view.View;
/** Compatibility surface expected by the factory 2019 SVR native plugin. */
public final class CompatUnityPlayer extends UnityPlayerForActivityOrService {
 public CompatUnityPlayer(Context context,IUnityPlayerLifecycleEvents events){super(context,events);}
 public View getChildAt(int index){return index==0?getSurfaceView():getFrameLayout().getChildAt(index);}
}
