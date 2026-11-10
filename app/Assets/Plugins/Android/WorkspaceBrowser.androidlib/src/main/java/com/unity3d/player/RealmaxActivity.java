package com.unity3d.player;

import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.os.Build;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;

public class RealmaxActivity extends Activity implements IUnityPlayerLifecycleEvents, IUnityPermissionRequestSupport, IUnityPlayerSupport
{
    public UnityPlayer mUnityPlayer; // legacy SVR JNI field descriptor
    protected CompatUnityPlayer unityPlayer; // don't change the name of this variable; referenced from native code

    // Override this in your custom UnityPlayerActivity to tweak the command line arguments passed to the Unity Android Player
    // The command line arguments are passed as a string, separated by spaces
    // UnityPlayerActivity calls this from 'onCreate'
    // Supported: -force-gles20, -force-gles30, -force-gles31, -force-gles31aep, -force-gles32, -force-gles, -force-vulkan
    // See https://docs.unity3d.com/Manual/CommandLineArguments.html
    // @param cmdLine the current command line arguments, may be null
    // @return the modified command line string or null
    protected String updateUnityCommandLineArguments(String cmdLine)
    {
        return cmdLine;
    }

    // Setup activity layout
    @Override protected void onCreate(Bundle savedInstanceState)
    {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        String cmdLine = updateUnityCommandLineArguments(getIntent().getStringExtra("unity"));
        getIntent().putExtra("unity", cmdLine);

        unityPlayer = new CompatUnityPlayer(this, this);
        mUnityPlayer = unityPlayer;
        setContentView(unityPlayer.getFrameLayout());
        unityPlayer.getFrameLayout().requestFocus();
        com.ar.workspace.SettingsProvisioner.handleIntent(this,getIntent());
    }

    @Override
    public UnityPlayerForActivityOrService getUnityPlayerConnection() {
        return unityPlayer;
    }

    // When Unity player unloaded move task to background
    @Override public void onUnityPlayerUnloaded() {
        moveTaskToBack(true);
    }

    // Callback before Unity player process is killed
    @Override public void onUnityPlayerQuitted() {
    }

    @Override protected void onNewIntent(Intent intent)
    {
        // To support deep linking, we need to make sure that the client can get access to
        // the last sent intent. The clients access this through a JNI api that allows them
        // to get the intent set on launch. To update that after launch we have to manually
        // replace the intent with the one caught here.
        setIntent(intent);
        com.ar.workspace.SettingsProvisioner.handleIntent(this,intent);
        unityPlayer.newIntent(intent);
    }

    // Quit Unity
    @Override protected void onDestroy ()
    {
        unityPlayer.destroy();
        super.onDestroy();
    }

    // If the activity is in multi window mode or resizing the activity is allowed we will use
    // onStart/onStop (the visibility callbacks) to determine when to pause/resume.
    // Otherwise it will be done in onPause/onResume as Unity has done historically to preserve
    // existing behavior.
    @Override protected void onStop()
    {
        super.onStop();
        unityPlayer.onStop();
    }

    @Override protected void onStart()
    {
        super.onStart();
        unityPlayer.onStart();
    }

    // Pause Unity
    @Override protected void onPause()
    {
        super.onPause();
        com.ar.workspace.JarvisVoiceBridge.setWorkspaceVisible(false);
        unityPlayer.onPause();
    }

    // Resume Unity
    @Override protected void onResume()
    {
        super.onResume();
        unityPlayer.onResume();
        com.ar.workspace.JarvisVoiceBridge.setWorkspaceVisible(true);
    }

    // Low Memory Unity
    @Override public void onLowMemory()
    {
        super.onLowMemory();
        unityPlayer.onTrimMemory(UnityPlayerForActivityOrService.MemoryUsage.Critical);
    }

    // Trim Memory Unity
    @Override public void onTrimMemory(int level)
    {
        super.onTrimMemory(level);
        switch (level)
        {
        case TRIM_MEMORY_RUNNING_MODERATE:
            unityPlayer.onTrimMemory(UnityPlayerForActivityOrService.MemoryUsage.Medium);
            break;
        case TRIM_MEMORY_RUNNING_LOW:
            unityPlayer.onTrimMemory(UnityPlayerForActivityOrService.MemoryUsage.High);
            break;
        case TRIM_MEMORY_RUNNING_CRITICAL:
            unityPlayer.onTrimMemory(UnityPlayerForActivityOrService.MemoryUsage.Critical);
            break;
        }
    }

    // This ensures the layout will be correct.
    @Override public void onConfigurationChanged(Configuration newConfig)
    {
        super.onConfigurationChanged(newConfig);
        unityPlayer.configurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    @Override public void onWindowFocusChanged(boolean hasFocus)
    {
        super.onWindowFocusChanged(hasFocus);
        unityPlayer.windowFocusChanged(hasFocus);
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    @Override public boolean dispatchKeyEvent(KeyEvent event)
    {
        if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
            return unityPlayer.injectEvent(event);
        return super.dispatchKeyEvent(event);
    }

    @Override
    @TargetApi(Build.VERSION_CODES.M)
    public void requestPermissions(PermissionRequest request)
    {
        unityPlayer.addPermissionRequest(request);
    }

    @Override public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        unityPlayer.permissionResponse(this, requestCode, permissions, grantResults);
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    @Override public boolean onKeyUp(int keyCode, KeyEvent event)     { return unityPlayer.getFrameLayout().onKeyUp(keyCode, event); }
    @Override public boolean onKeyDown(int keyCode, KeyEvent event)   { return unityPlayer.getFrameLayout().onKeyDown(keyCode, event); }
    @Override public boolean onTouchEvent(MotionEvent event)          { return unityPlayer.getFrameLayout().onTouchEvent(event); }
    @Override public boolean onGenericMotionEvent(MotionEvent event)  { return unityPlayer.getFrameLayout().onGenericMotionEvent(event); }
}
