using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealerAI : MonoBehaviour {
    public bool player;
    public CombatManager combatManager;
    public PlayerController controller;
    private PlayerStats _stats;
    public List<int> healTargets;
    public float wait = 0f;

	// Use this for initialization
	void Start () {
        _stats = GetComponent<PlayerStats>();
    }
	
	// Update is called once per frame
	void Update () {
        if(_stats.Cooldown <= 0)
        {
            healTargets = FindHealTargets();
            if (healTargets.Count > 1)
            {
                controller.friendlyTarget = healTargets[0];
                controller.DoAttack(1);
                healTargets.Clear();
            }
            else if (healTargets.Count == 1)
            {
                controller.friendlyTarget = healTargets[0];
                controller.DoAttack(0);
                healTargets.Clear();
            }
            else if(wait <= 0)
            {
                int atk = Random.Range(0, 2);
                controller.DoAttack(atk);
                wait = Random.Range(0f, 5f);
            }
            else
            {
                wait -= Time.deltaTime;
            }
        }
	}

    public List<int> FindHealTargets()
    {
        List<int> targets = new List<int>();
        if (player)
        {
            for (int i = 0; i < combatManager.Players.Count; i++)
            {
                if ((float)(combatManager.Players[i].GetComponent<PlayerController>().Health / combatManager.Players[i].GetComponent<PlayerController>().BaseHealth) < .8)
                {
                    targets.Add(i);
                }
            }
        }
        else//enemies
        {

        }
        return targets;
    }

    public void StartAI()
    {
        controller = GetComponent<PlayerController>();
        if (controller.attacks.Count == 0)
        {
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Basic Heal") as GameObject).GetComponent<AttackDetailsInstance>()));
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Party Heal") as GameObject).GetComponent<AttackDetailsInstance>()));
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Phys Wall") as GameObject).GetComponent<AttackDetailsInstance>()));
        }
        combatManager = FindObjectOfType<CombatManager>();
    }
}
