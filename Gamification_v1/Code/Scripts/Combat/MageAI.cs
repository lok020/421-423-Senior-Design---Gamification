using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAI : MonoBehaviour
{
    public bool player;
    public CombatManager combatManager;
    public PlayerController controller;
    public List<int> healTargets;
    public float mindelay = 0f, maxdelay = 5f;
    private float wait = 0f;
    public bool isBuffed;

    private PlayerStats _stats;

    // Use this for initialization
    void Start()
    {
        _stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_stats.Cooldown <= 0)
        {
            //If taunted, 80% chance it targets them, else it targets randomly
            if (controller.Stats.TauntTarget != null && (Random.Range(0, 1) < 0.8))
            {
                controller.enemyTarget = combatManager.Enemies.IndexOf(controller.Stats.TauntTarget);
            }
            else
            {
                controller.enemyTarget = Random.Range(0, combatManager.Enemies.Count);
            }
            if (wait <= 0)
            {
                int atk = Random.Range(0, controller.attacks.Count - 1);
                if (controller.attacks[atk].Type == AttackType.BUFF)
                {
                    if (isBuffed) return;
                    else
                    {
                        StartCoroutine(BuffDuration(controller.attacks[atk]));
                    }
                }
                controller.DoAttack(atk);
                wait = Random.Range(mindelay, maxdelay);
            }
            else
            {
                wait -= Time.deltaTime;
            }
        }
    }

    IEnumerator BuffDuration(AttackDetails attack)
    {
        isBuffed = true;
        yield return new WaitForSeconds(attack.BuffDuration);
        isBuffed = false;
    }

    public void StartAI()
    {
        controller = GetComponent<PlayerController>();
        if (controller.attacks.Count == 0)
        {
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Magic Bolt") as GameObject).GetComponent<AttackDetailsInstance>()));
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Magic Wave") as GameObject).GetComponent<AttackDetailsInstance>()));
            controller.attacks.Add(new AttackDetails((Resources.Load("Attacks/Concentrate") as GameObject).GetComponent<AttackDetailsInstance>()));
        }
        combatManager = FindObjectOfType<CombatManager>();
    }
}

