using UnityEngine;
using System.Collections;
using UnityTest;

public class DebugResults : MonoBehaviour {

    public float SecondsDelay = 5.0f;      //Delay system
    public string TestName = "";
    public string NextSceneName = "Menu";
    public bool EnableAllChildren = true;   //Enable all objects attached to this script
    private bool anyTestsFailed = false;

	// Use this for initialization
	void Start () {
        //Enable all child objects if flag is set
        if (EnableAllChildren)
        {
            foreach (Transform child in this.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log("Activating " + child.name);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (SecondsDelay > 0 && SecondsDelay < 9000)
        {
            SecondsDelay -= Time.deltaTime;
        }
        else if(SecondsDelay < 0)
        {
            GetResults();
            SecondsDelay = 9001;
        }
	}

    //Show results window
    void OnGUI()
    {
        if (SecondsDelay > 9000) //It's over 9000!
        {
            //Results are pass/fail
            string testResults = "All tests passed!";
            if(anyTestsFailed)
            {
                testResults = "One or more tests failed.";
            }

            //Basic window
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Box(new Rect((Screen.width - 400) / 2, (Screen.height - 120) / 2, 400, 120), "Test Results");
            GUI.Label(new Rect((Screen.width - 300) / 2, (Screen.height - 50) / 2 - 10, 300, 50), TestName + ": " + testResults, centeredStyle);

            //Button for proceeding to next test
            if (GUI.Button(new Rect((Screen.width - 150) / 2 - 100, (Screen.height - 50) / 2 + 20, 150, 50), "Run next test")) {
                Debug.Log("Running next test, scene name: " + NextSceneName);
                Application.LoadLevel(NextSceneName);
            }

            //Button for returning to lobby
            if (GUI.Button(new Rect((Screen.width - 150) / 2 + 100, (Screen.height - 50) / 2 + 20, 150, 50), "Exit testing")) {
                Debug.Log("Returning to menu...");
                Application.LoadLevel("Menu");
            }
        }
    }

    // Get results of all assertion tests run by the parent component
    void GetResults()
    {
        ArrayList results = new ArrayList();
        foreach(AssertionComponent c in this.gameObject.GetComponentsInParent<AssertionComponent>())
        {
            results.Add(c.hasFailed);
            if(c.hasFailed)
            {
                anyTestsFailed = true;
            }
        }
    }
}
