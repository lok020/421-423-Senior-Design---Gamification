using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerAI : MonoBehaviour
{
    public CombatManager combatManager;
    public PlayerController controller;
    public List<AttackDetailsInstance> preLoadedAttacks; //for hardcoded cpus
    public List<bool> isBuffActive = new List<bool> { false, false, false, false, false };
    public List<int> singleheals,areaheals, heals;
    public Dictionary<int,float> healTargets;
    public float wait = 0f;
    public float lowest = 1f;

    private PlayerStats _stats;

    // Use this for initialization
    void Start()
    {
        _stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller && controller.CPU)
        {
            if (_stats.Cooldown <= 0)
            {
                if(heals.Count > 0)
                {
                    healTargets.Clear();
                    controller.friendlyTarget = FindHealTargets();
                    if (lowest < .6f)
                    {
                        AiHeal();
                    }
                }

                if (wait <= 0)
                {
                    int atk = 0;
                    while (isBuffActive[atk = Random.Range(0, controller.attacks.Count)])

                        if (controller.attacks[atk].Type == AttackType.BUFF)
                            StartCoroutine(SetBuffActive(controller.attacks[atk], atk));

                    if (controller.attacks[atk].Type != AttackType.HEAL)
                    {
                        controller.enemyTarget = FindAttackTargets(true);
                    }

                    controller.DoAttack(atk);
                    wait = Random.Range(0.5f, 1f);
                }
                else
                {
                    wait -= Time.deltaTime;
                }
            }
        }
    }

    public int FindHealTargets()
    {
        int target = 0;
        lowest=1;
        for (int i = 0; i < combatManager.Players.Count; i++)
        {
            float value = (float)(combatManager.Players[i].GetComponent<PlayerController>().Health / combatManager.Players[i].GetComponent<PlayerController>().BaseHealth);
            if(value < 1) healTargets.Add(i,value);
            if (value < lowest)
            {
                lowest = value;
                target = i;
            }
        }
        return target;
    }

    public int FindAttackTargets(bool attackweakest)
    {
        int target = 0;
        lowest = 1;
        //If taunted, 80% chance it targets them, else it targets randomly
        if (controller.Stats.TauntTarget != null && (Random.Range(0, 1) < 0.8))
        {
            return combatManager.Enemies.IndexOf(controller.Stats.TauntTarget);
        }
        else if (attackweakest)
        {
            for (int i = 0; i < combatManager.Enemies.Count; i++)
            {
                float value = (combatManager.Enemies[i].GetComponent<EnemyController>().Stats.Health / combatManager.Enemies[i].GetComponent<EnemyController>().Stats.BaseHealth);
                if (value < lowest)
                {
                    lowest = value;
                    target = i;
                }
            }
            return target;
        }
        else
        {
            //Debug.Log(Random.Range(0, combatManager.enemies.Count));
            return Random.Range(0, combatManager.Enemies.Count);
        }
    }

    public void AiHeal()
    {
        if (healTargets.Count > 1 && areaheals.Count > 0)
        {
            int i = Random.Range(0, areaheals.Count-1);
            controller.DoAttack(areaheals[i]);
            return;
        }
        else if(healTargets.Count == 1 && singleheals.Count > 0)
        {
            int i = Random.Range(0, singleheals.Count);
            controller.DoAttack(singleheals[i]);
            return;
        }
        else
        {
            int i = Random.Range(0, heals.Count);
            controller.DoAttack(heals[i]);
            return;
        }
    }

    IEnumerator SetBuffActive(AttackDetails attack, int index)
    {
        isBuffActive[index] = true;
        yield return new WaitForSeconds(attack.BuffDuration);
        isBuffActive[index] = false;
    }

    public void StartAI()
    {
        controller = GetComponent<PlayerController>();
        combatManager = FindObjectOfType<CombatManager>();
        controller.combatManager = combatManager.gameObject;
        healTargets = new Dictionary<int, float>();
        for (int i = 0; i < preLoadedAttacks.Count; i++)
        {
            controller.attacks.Add( new AttackDetails(preLoadedAttacks[i]));
        }

        List<AttackDetails> attacks = controller.attacks;
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i].Type == AttackType.HEAL)
            {
                if (attacks[i].Target != AttackTarget.SINGLE && attacks[i].Target != AttackTarget.SELF)
                {
                    areaheals.Add(i);
                }
                else
                {
                    singleheals.Add(i);
                }
                heals.Add(i);
            }
        }
    }
}
