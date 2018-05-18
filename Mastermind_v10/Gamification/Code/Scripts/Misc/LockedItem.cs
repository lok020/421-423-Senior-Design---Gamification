using UnityEngine;
using System.Collections;

public class LockedItem : MonoBehaviour {

    public string Name;
    public GameObject Key;
    public double UnlockAfterQuestID;
    public double UnlockAfterObjectiveID;
    public bool DisableWhenUnlocked;

    private QuestManager qm;
    private SpriteRenderer sprite;
    private bool Locked = true;
    private float timer = 0.0f;
    
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) qm = player.GetComponent<QuestManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!Locked && timer <= 0) return;
        else if(!Locked)
        {
            timer -= Time.deltaTime;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, timer);
            if (timer <= 0 && DisableWhenUnlocked) gameObject.SetActive(false);
        }
        else if (qm == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) qm = player.GetComponent<QuestManager>();
        }
        else if(qm.ObjectiveCompleted(UnlockAfterQuestID, UnlockAfterObjectiveID))
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        Locked = false;
        timer = 1.0f;
    }
}
