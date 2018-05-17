using UnityEngine;
using System.Collections;

public class LoadTestScene : MonoBehaviour {

	public void LoadScene(string SceneName)
    {
        Application.LoadLevel(SceneName);
    }
}
