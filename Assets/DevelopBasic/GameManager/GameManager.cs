using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using SimpleAudioSystem;
using SimpleSaveSystem;

//Please make sure "GameManager" is excuted before every custom script
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int targetFrameRate = 60;
[Header("Scene Transition")]
    [SerializeField] private CanvasGroup BlackScreenCanvasGroup;
    [SerializeField] private float transitionDuration = 1;
    [SerializeField] private float restartDuration = 1;
[Header("Init")]
    [SerializeField] private string InitScene;
[Header("Demo")]
    [SerializeField] private string InitDemoScene;
    [SerializeField] private bool isDemo = true;
[Header("Debug")]
    [SerializeField] private bool loadInitSceneFromGameManager = false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private InputActionMap debugActions;
#endif

    private static bool isPaused = false;

    public bool IsSwitchingScene{get; private set;} = false;
    public bool IsDemo => isDemo;
    public string lastScene{get; private set;} = string.Empty;
    public string currentScene{get; private set;} = string.Empty;

    protected override void Awake(){
        base.Awake();
        Application.targetFrameRate = targetFrameRate;
        SaveManager.Initialize();
        if(isDemo)
            SaveManager.SwitchSaving(false);

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["save"].performed += Debug_Save;
        debugActions["load"].performed += Debug_Load;
        debugActions["quit"].performed += Debug_EndGame;
    #endif
    }
    void Start()
    {
        SaveManager.LoadGameState(0);
    //To Do: Game Loading
    #if UNITY_EDITOR
    //Load Level
        if(loadInitSceneFromGameManager){
            BlackScreenCanvasGroup.alpha = 1;
            SwitchingScene(string.Empty, GetInitSceneName(), transitionDuration, 0.5f);
        }
        else {
            currentScene = SceneManager.GetActiveScene().name;
        }

    #else
    //Since we don't have the saving system yet, the initiation should be done by loading the debug progress data.
        SwitchingScene(string.Empty, GetInitSceneName(), transitionDuration, 0.5f);
    #endif
    }
    protected override void OnDestroy(){
        base.OnDestroy();

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["save"].performed -= Debug_Save;
        debugActions["load"].performed -= Debug_Load;
        debugActions["quit"].performed -= Debug_EndGame;

        if(debugActions.enabled)debugActions.Disable();
    #endif
    }

#region GAME BASIC
    public void PauseTheGame(){
        if(isPaused) return;
        
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
    }
    public void ResumeTheGame(){
        if(!isPaused) return;

        Time.timeScale = 1;
        AudioListener.pause = false;
        isPaused = false;
    }
    public void EndGame(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(EndGameCoroutine(currentLevel));
    }
    public void RestartLevel(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(RestartLevel(currentLevel));
    }
#endregion
    string GetInitSceneName()
    {
        return isDemo?InitDemoScene:InitScene;
    }
#region Scene Transition
    public void SwitchingScene(string to, float transition = 1, float intersection = 0, bool autosaveAfterTransition = true){
        string from = SceneManager.GetActiveScene().name;
        SwitchingScene(from, to, transition, intersection, autosaveAfterTransition);
    }
    void SwitchingScene(string from, string to, float transition, float intersection, bool autosaveAfterTransition = true){
        if(!IsSwitchingScene) StartCoroutine(SwitchSceneCoroutine(from, to, transition, intersection, autosaveAfterTransition));
    }
    IEnumerator EndGameCoroutine(string level){
        StartCoroutine(FadeInBlackScreen(1f));
        StartCoroutine(new WaitForLoopUnscale(3f, (t)=>{
            AudioManager.Instance.ChangeMasterVolume(Mathf.Lerp(1, 0, EasingFunc.Easing.QuadEaseIn(t)));
        }));

        yield return new WaitForSecondsRealtime(1f);
        EventHandler.Call_BeforeUnloadScene();
        yield return SceneManager.UnloadSceneAsync(level);
        yield return new WaitForSecondsRealtime(1f);
        Application.Quit();
    }
    IEnumerator RestartLevel(string level){
        yield return FadeInBlackScreen(restartDuration*0.5f);
        IsSwitchingScene = true;
        //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
        EventHandler.Call_BeforeUnloadScene();

        yield return SceneManager.UnloadSceneAsync(level);
        yield return null;
        //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(level));
        //TO DO: do something after the next scene is loaded. e.g: call event of loading
        yield return null;
        ResumeTheGame();
        yield return FadeOutBlackScreen(restartDuration*0.5f);
        EventHandler.Call_AfterLoadScene();
        
        IsSwitchingScene = false;
    }
    IEnumerator SwitchSceneCoroutine(string from, string to, float transition, float intersection, bool autosaveAfterTransition){
        IsSwitchingScene = true;
        if(from != string.Empty){
        //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
            lastScene = from;
            
            EventHandler.Call_BeforeUnloadScene();
            yield return FadeInBlackScreen(transition*0.5f);
            yield return SceneManager.UnloadSceneAsync(from);
        }
        else
            yield return null;
            
        if(intersection>0)
            yield return new WaitForSeconds(intersection);
    //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));
        currentScene = to;

    //TO DO: do something after the next scene is loaded. e.g: call event of loading
        EventHandler.Call_AfterLoadScene();
    //AutoSave Game when transition to New Scene
        if(autosaveAfterTransition) 
            SaveManager.SaveGameState(0);

        ResumeTheGame();
        yield return null;
        yield return FadeOutBlackScreen(transition*0.5f);

        IsSwitchingScene = false;
    }
    public IEnumerator FadeInBlackScreen(float fadeDuration){
        float initAlpha = BlackScreenCanvasGroup.alpha;
        yield return new WaitForLoopUnscale(fadeDuration, (t)=>{
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 1, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
    public IEnumerator FadeOutBlackScreen(float fadeDuration){
        float initAlpha = BlackScreenCanvasGroup.alpha;
        yield return new WaitForLoopUnscale(fadeDuration, (t)=>{
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 0, EasingFunc.Easing.QuadEaseIn(t));
        });
    }
#endregion

#region DEBUG ACTION
    void Debug_EndGame(InputAction.CallbackContext callback)=>EndGame();
    void Debug_Save(InputAction.CallbackContext callback)=>SaveManager.SaveGameState(0);
    void Debug_Load(InputAction.CallbackContext callback)=>SaveManager.LoadGameState(0);
#endregion
}
