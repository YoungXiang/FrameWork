# FrameWork
A collection of patterns and frameworks I used in my own game development

# AssetManagement
Using a gameObject as a host when loading any asset, which allows auto assetBundle unload.
Using WebClient instead of Unity's builtin WWW class.
Asset build pipeline is still under development, it requires a json config as build rule, and parse it when building an assetBundle.

# SceneManagement
Allows you to write your own logic when scene is loaded or unloaded.
Usage:
public class MainScene : UnityScene 
{
    // do your logic here.
    public override void OnLoaded(){}
}

// register all scenes before load
SceneManager.Instance.RegisterScene<MainScene>("MainScene", "Assets/Res/Scenes/MainScene.unity");

//... 

// now this should load the scene for you
SceneManager.Instance.LoadScene("MainScene");

# Event
Usage pattern : 
EventListener.Do(What).When("EventName");
Well, it's better to change it to EventListener.Do("EventName", What); This is the most common usage of any event system.

# StateMachine
Provides MonoStateMachine, which comes with Editor to generate the code as well. 
The state machine is a generic pattern used in everywhere, UI logic, other logics.
Let's say you have an UI that shows and hides, you only need to write two states for the UI stateMachine : idle and show. And do your logic in there.

# Threading
Useful for loading large datas. I used it in my own caching system.

# ConfDataLoader
This comes with an Excel exporter, which exports the data class and the excel data(Serialized). 
And ConfDataLoader is used to load that specific type of data.

