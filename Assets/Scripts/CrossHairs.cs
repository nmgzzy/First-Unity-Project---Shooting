using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public SpriteRenderer dot;
    public Transform dotTransform;
    public LayerMask targetMask;
    public Color dotHighlightColor;
    Color originalDotColor;
    Player player;
    Vector3 init;
    
    void Start() {
        originalDotColor = dot.color;
        Cursor.visible = false;
        player = FindObjectOfType<Player> ();
        init = this.transform.localScale;
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * 80 * Time.deltaTime);
        float spd = 0.2f;
        if (player.isAiming) {
            float delta = Time.time - player.aimingTime;
            if (delta < 0.5f) {
                Vector3 add = Vector3.one * spd * delta;
                this.transform.localScale = init - add;
                dotTransform.localScale = Vector3.one * 4 + add*50;
            }
        }
        else {
            this.transform.localScale = init;
            dotTransform.localScale = Vector3.one * 4;
        }
    }

    public void DetectTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
