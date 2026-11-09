using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using SnapdragonVR;

public class Main : MonoBehaviour
{
    private int sceneIndex = -1;
    private SvrManager svrManager = null;
    private Coroutine handleBackButton = null;

    void Awake()
    {
        svrManager = SvrManager.Instance;

        Input.backButtonLeavesApp = false;

        var activeScene = SceneManager.GetActiveScene();
        sceneIndex = activeScene.buildIndex;
    }

    IEnumerator HandleBackButton()
    {
        svrManager.SetOverlayFade(SvrManager.eFadeState.FadeOut);
        yield return new WaitUntil(() => svrManager.IsOverlayFading() == false);

        // Load next scene in build settings, quit when done
        if (++sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            svrManager.Shutdown();
            yield return new WaitUntil(() => svrManager.Initialized == false);

            SceneManager.LoadScene(sceneIndex);

            System.GC.Collect();
        }
        else
        {
            sceneIndex = -1;
            Application.Quit();
        }

        handleBackButton = null;
    }

    void LateUpdate()
    {
        if (!SvrPlugin.Instance.IsInitialized())
            return;

        if (!SvrPlugin.Instance.IsRunning())
            return;

        if(!Input.backButtonLeavesApp && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Application Back");

            if(handleBackButton == null)
                handleBackButton = StartCoroutine(HandleBackButton());
        }
        else if(Input.GetMouseButton(0))
        {
            SvrPlugin.Instance.RecenterTracking();
        }
    }
}
