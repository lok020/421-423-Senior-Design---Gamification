using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserPassBehavior : MonoBehaviour
{
    public GameObject Previous, Next, Button;
    public float Timer = 0;

    private EventSystem system;
    private float delay = 0.1f;      //Only register tabs every 0.1 seconds

    private void Start()
    {
        system = EventSystem.current;
        if (system == null) Debug.Log("WTF");
    }

    private void Update()
    {
        //If this isn't selected, skip processing
        if (system.currentSelectedGameObject != gameObject) return;
        //If the timer is still running, don't process next tab
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //If shift is being held, go to prev
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                // Select the previous item
                system.SetSelectedGameObject(Previous);

                // Simulate Inputfield MouseClick
                InputField inputfield = Previous.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));

                //Set delay
                Previous.GetComponent<UserPassBehavior>().Timer = delay;
            }
            else
            {
                // Select the next item
                system.SetSelectedGameObject(Next);

                // Simulate Inputfield MouseClick
                InputField inputfield = Next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));

                //Set delay
                Next.GetComponent<UserPassBehavior>().Timer = delay;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (Button.GetComponent<Button>() == null) return;
            Button.GetComponent<Button>().onClick.Invoke();
        }
    }
}