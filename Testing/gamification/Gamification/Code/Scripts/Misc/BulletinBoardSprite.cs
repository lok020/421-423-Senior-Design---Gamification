using UnityEngine;

public class BulletinBoardSprite : MonoBehaviour {

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameObject.Find("Bulletin Board UI").GetComponentsInChildren<Canvas>(true)[0].gameObject.SetActive(false);
        }
    }
}
