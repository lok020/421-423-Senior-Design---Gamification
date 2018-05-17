using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum minigame
{
    none,
    seeker,
    conquer,
    mastermind
}

public static class MySceneManager{

    public static string scene_seeker = "";
    public static string scene_mastermind = "";
    public static string scene_conquer = "";
    public static minigame activeGame;

    public static void sceneChange(minigame type, string newScene)
    {

        //unload
        if (activeGame == minigame.seeker)
        {
            SceneManager.UnloadScene(scene_seeker);
            scene_seeker = "";
        }
        else if (activeGame == minigame.mastermind)
        {
            SceneManager.UnloadScene(scene_mastermind);
            scene_mastermind = "";
        }
        else if (activeGame == minigame.conquer)
        {
            SceneManager.UnloadScene(scene_conquer);
            scene_conquer = "";
        }

        //load
        if (type == minigame.seeker)
        {
            SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
            scene_seeker = newScene;
        }
        else if (type == minigame.mastermind)
        {
            SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
            scene_mastermind = newScene;
        }
        else if (type == minigame.conquer)
        {
            SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
            scene_conquer = newScene;
        }
        else
            Debug.Log("Please insert valid type");
        activeGame = type;
    }
}
