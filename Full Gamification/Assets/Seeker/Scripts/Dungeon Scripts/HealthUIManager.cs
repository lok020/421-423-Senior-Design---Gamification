using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthUIManager : MonoBehaviour {

    public Slider health_bar;
    public Text hp_text;
    public PlayerStats player_health;

    public Slider stamina_bar;
    public Text stamina_text;
    public PlayerStats player_stamina;

    public UnityEvent death;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        health_bar.maxValue = player_health.max_health;
        health_bar.value = player_health.current_health;
        hp_text.text = (player_health.current_health + "/" + player_health.max_health);

        stamina_bar.maxValue = player_stamina.max_stamina;
        stamina_bar.value = player_stamina.current_stamina;
        stamina_text.text = (player_stamina.current_stamina + "/" + player_stamina.max_stamina);

        if (player_health.current_health <= 0)
        {
            if (death != null)
            {
                death.Invoke();
            }
        }
	}
}
