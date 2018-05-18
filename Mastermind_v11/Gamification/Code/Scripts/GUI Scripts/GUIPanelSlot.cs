using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIPanelSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public GameObject Tooltip;
    public string Text;

    private Text _tooltipText;

	// Use this for initialization
	void Start () {
        _tooltipText = Tooltip.GetComponentsInChildren<Text>(true)[0];
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.SetActive(true);
        var mouse = Input.mousePosition;
        _tooltipText.text = Text;
        /*
        Tooltip.GetComponent<RectTransform>().localPosition = new Vector3(
            GetComponentInParent<RectTransform>().localPosition.x + GetComponent<RectTransform>().localPosition.x - (GetComponent<RectTransform>().rect.width / 2) - (Tooltip.GetComponent<RectTransform>().rect.width / 2),
            GetComponentInParent<RectTransform>().localPosition.y + GetComponent<RectTransform>().localPosition.y);*/
        //Tooltip.transform.localPosition = new Vector3(parent.localPosition.x + transform.localPosition.x, parent.localPosition.y + transform.localPosition.y);
        float x = mouse.x - GetComponent<RectTransform>().rect.width;
        float y = transform.position.y + GetComponent<RectTransform>().rect.height;
        Tooltip.transform.position = new Vector3(x, y);
        Tooltip.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipText.text = "";
        Tooltip.SetActive(false);
    }
}
