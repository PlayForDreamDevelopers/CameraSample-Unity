using UnityEngine;
using UnityEngine.SceneManagement;
using YVR.Core;

public class SceneChangeManager : MonoBehaviour
{
    private void Start() { YVRManager.instance.hmdManager.SetPassthrough(true); }

    public void EnterSample(string sceneName) { SceneManager.LoadScene(sceneName); }
}