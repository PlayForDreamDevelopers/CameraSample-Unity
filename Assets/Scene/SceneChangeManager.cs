using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    
    public void EnterSample(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}