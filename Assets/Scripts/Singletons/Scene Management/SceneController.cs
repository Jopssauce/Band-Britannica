using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[System.Serializable]
public class SceneEvents : UnityEvent<Scene> { }

public class SceneController : MonoBehaviour {

    public bool loadTitleOnStart = false;
	public static SceneController instance;
    public GameObject loadingScreen;
    string sceneToActive;
    public SceneEvents EventSceneLoaded;
    public AsyncOperation sceneAsync;
    // Use this for initialization
    void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += ReturnLoadedScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneLoaded += ReturnLoadedScene;
    }

    private void Start()
    {
        if (loadTitleOnStart)
        {
            LoadSceneAdditive("Title Scene", true);
        }

    }
    /// <summary>
    ///Loads a regular scene. True to set load scene active false if not
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="setActive"></param>

    public AsyncOperation LoadSceneAdditive(string scene, bool setActive)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        if(setActive == true) sceneToActive = scene;
        return async;
    }


    /// <summary>
    /// Loads a regular scene. Unloads everything but persistent scene
    /// </summary>
    /// <param name="scene"></param>
    public AsyncOperation LoadScene(string scene)
    {
        UnloadAllScenes();
        AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        StartCoroutine(InitializeLoadingScreen(async));
        sceneToActive = scene;
        return async;
    }

    /// <summary>
    /// Loads Stage along with Board. Unloads everything but persistent scene. Sets Stage as active scene
    /// </summary>
    /// <param name="scene"></param>
    public void LoadStage(string scene)
    {
        UnloadAllScenes();
        LoadSceneAdditive(scene, true);
        StartCoroutine(InitializeLoadingScreen(LoadSceneAdditive("Board Scene", false)));
    }

    public void ReloadStage(string stage)
    {
        UnloadAllScenes();
        LoadStage(stage);
    }

    public void UnloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(scene));
    }

    //Unloads Everything but the persistent scene
    void UnloadAllScenes()
    {
        foreach (var item in GetAllLoadedScenes())
        {
            if (item.name != "Persistent Scene")
            {
                UnloadScene(item.name);
            }
        }
    }

    public List<Scene> GetAllLoadedScenes()
    {
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            scenes.Add(SceneManager.GetSceneAt(i));
        }

        return scenes;
    }

    public IEnumerator DelayLoadScene(string scene, float delay, LoadSceneMode mode)
    {
        yield return new WaitForSeconds(delay);
        yield return SceneManager.LoadSceneAsync(scene, mode);
    }

    public bool isSceneOpen(string SceneName)
    {
        Scene scene = SceneManager.GetSceneByName(SceneName);
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i) == scene)
            {

                return true;
            }
        }
        return false;
    }

    public int GetSceneAmount(string sceneName)
    {
        int amt = 0;
        //return SceneManager.GetAllScenes().Count(scene => scene.name == sceneName);
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) amt++;
        }
        return amt;
    }

    public IEnumerator InitializeLoadingScreen(AsyncOperation asnyc)
    {

        if(loadingScreen != null)loadingScreen.gameObject.SetActive(true);
        yield return asnyc.isDone;
        yield return new WaitForSeconds(1);
        if (loadingScreen != null) loadingScreen.gameObject.SetActive(false);
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!SceneManager.GetSceneByName(sceneToActive).isLoaded) return;
        SetSceneToActive(sceneToActive);
    }

    void SetSceneToActive(string scene)
    {
        if (string.IsNullOrEmpty(scene)) return;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        sceneToActive = "";
        //if (loadingScreen != null) loadingScreen.gameObject.SetActive(false);
    }

    void ReturnLoadedScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        EventSceneLoaded.Invoke(scene);
    }
}
