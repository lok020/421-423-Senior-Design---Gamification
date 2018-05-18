using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC_TestWalk : MonoBehaviour {

    NPC_Controller npc;
    MessageOverlayController messageController;

    // walk in octagonal pattern
    void Start()
    {
        npc = this.GetComponentInParent<NPC_Controller>();
        messageController = GameObject.Find("Message Overlay").GetComponent<MessageOverlayController>();
        ExecuteTestWalk();
    }

    private void ExecuteTestWalk()
    {
        StartCoroutine(TestWalk());
    }

    private IEnumerator TestWalk()
    {
        Debug.Log("Start");
        npc.Move(1, 0, 1);
        yield return new WaitForSeconds(2);
        messageController.EnqueueMessage("Hello world!");
        //npc.Speak(null, null);
        npc.Move(1, 1, 1);
        yield return new WaitForSeconds(2);
        npc.Move(0, 1, 1);
        yield return new WaitForSeconds(2);
        messageController.EnqueueMessage("How are you today?");
        npc.Move(-1, 1, 1);
        yield return new WaitForSeconds(2);
        npc.Move(-1, 0, 1);
        yield return new WaitForSeconds(2);
        npc.Move(-1, -1, 1);
        yield return new WaitForSeconds(2);
        messageController.EnqueueMessage("Leave me alone! :(");
        npc.Move(0, -1, 1);
        yield return new WaitForSeconds(2);
        npc.Move(1, -1, 1);
        //npc.Speak(null, null);
        yield return new WaitForSeconds(2);
        npc.Face(0);
        yield return new WaitForSeconds(1);
        Debug.Log("Done");
    }
}
