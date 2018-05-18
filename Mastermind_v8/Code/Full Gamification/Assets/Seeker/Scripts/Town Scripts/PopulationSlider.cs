using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopulationSlider : MonoBehaviour {

    public Slider population_bar;
    public Text population_text;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        population_bar.maxValue = GlobalControl.Instance.max_population;
        population_bar.value = GlobalControl.Instance.current_population;
        population_text.text = (GlobalControl.Instance.current_population + "/" + GlobalControl.Instance.max_population);
    }
}
