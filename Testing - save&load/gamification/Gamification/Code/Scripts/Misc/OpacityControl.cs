using UnityEngine;

public class OpacityControl : MonoBehaviour {

    public float TransparentOpacity = 0.2f;

    private bool Transparent = false;
    private int FramesSincePlayerCollision = 0;
    private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        sprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Transparent && FramesSincePlayerCollision > 3)
            Transparent = false;
        if (Transparent)
        {
            if (sprite.color.a > TransparentOpacity)
            {
                float newTransparency = sprite.color.a - Time.deltaTime *2.5f;
                if (newTransparency < TransparentOpacity) newTransparency = TransparentOpacity;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newTransparency);
            }
        }
        else
        {
            if (sprite.color.a < 1)
            {
                float newTransparency = sprite.color.a + Time.deltaTime;
                if (newTransparency > 1) newTransparency = 1;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newTransparency);
            }
        }
        FramesSincePlayerCollision++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "VirtualPlayer")
        {
            Transparent = true;
            FramesSincePlayerCollision = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "VirtualPlayer")
        {
            Transparent = true;
            FramesSincePlayerCollision = 0;
        }
    }
}
