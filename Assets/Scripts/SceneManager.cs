using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private Scene[] scenes;
    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private string startSceneName;
    private Scene currentScene;
    private bool musicOn = true;

    private void Awake()
    {
        scenes = GetComponentsInChildren<Scene>(true);
        foreach(var scene in scenes)
        {
            scene.gameObject.SetActive(false);
            foreach(var obj in scene.DependentObjects)
            {
                obj.SetActive(false);
            }
        }
    }
    private void Start()
    {
        SetScene(startSceneName);
    }
    public Scene GetCurrentScene() => currentScene;
    public void SetScene(string sceneFlag)
    {
        if (sceneFlag == string.Empty) return;
        foreach (var scene in scenes)
        {
            if(scene.SceneFlag == sceneFlag)
            {
                if (currentScene != null)
                {
                    currentScene.gameObject.SetActive(false);
                    foreach (var obj in currentScene.DependentObjects) obj.SetActive(false);
                }
                currentScene = scene;
                Vector3 cameraPos = scene.CameraPosition;
                cameraPos.z = -10f;
                Camera.main.transform.position = cameraPos;
                currentScene.gameObject.SetActive(true);
                foreach (var obj in scene.DependentObjects) obj.SetActive(true);
                languageManager.LoadTextFlags(scene);
                scene.OnStartScene?.Invoke();
                return;
            }
        }
    }

    public void ToggleVolume()
    {
        musicOn = !musicOn;
        var sounds = transform.GetComponentsInChildren<AudioSource>(true);
        Debug.Log("Sounds found: " + sounds.Length);
        foreach (var sound in sounds) sound.mute = !musicOn;
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
