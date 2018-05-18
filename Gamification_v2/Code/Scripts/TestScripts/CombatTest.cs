using UnityEngine;
using System.Collections;

public class CombatTest : MonoBehaviour {
    bool wasInCombat = false;

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        if(wasInCombat == false)
        {
            CombatTests();
        }
	}

    void CombatTests()
    { 
        wasInCombat = true;
        GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().JoinCombat(gameObject);
        GameObject enemy = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().Enemies[0];
        //GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().ExecutePlayerAttack(gameObject, 0, 0, GetComponent<PlayerController>().attacks[0]);

        if (enemy.GetComponent<EnemyController>().Stats.Health == 80)
        {
           Debug.Log("Attack damage test passed");
        }
        else
        {
           Debug.Log("Attack damage test failed");
        }
        GetComponent<PlayerController>().Health = 50;
        //GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().ExecutePlayerAttack(gameObject, 0, 0, GetComponent<PlayerController>().attacks[1]);
        if(GetComponent<PlayerController>().Health == 62)
        {
            Debug.Log("Healing test passed");
        }
        else
        {
            Debug.Log("Healing test failed");
        }
        enemy.GetComponent<PlayerStats>().InflictDamage(1000);
        if (!enemy.activeSelf)
            Debug.Log("Enemy Death test passed");
        else
            Debug.Log("Enemy Death test failed");
        if (CombatResultsTest())
            Debug.Log("All combat results tests passed");
    }

    bool CombatResultsTest()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().GiveRewards();
        if (playerController.Stats.XP == 10)
        {
            Debug.Log("Xp Test Passed");
            if (GetComponent<Inventory>().Gold == 10)
            {
                Debug.Log("Gold Test Passed");
                if (GetComponent<Inventory>().ItemInInventory(1) && GetComponent<Inventory>().ItemInInventory(3) && GetComponent<Inventory>().ItemInInventory(4))
                {
                    Debug.Log("Item Rewards test Passed");
                    return true;
                }
                else
                {
                    Debug.Log("Inventory Test Failed");
                    return false;
                }
            }
            else
            {
                Debug.Log("Gold test failed");
                return false;
            }
        }
        else
        {
            Debug.Log("Xp test failed");
            return false;
        }
    }
}
