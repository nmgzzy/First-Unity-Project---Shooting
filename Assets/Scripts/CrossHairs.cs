using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public SpriteRenderer dot;
    public LayerMask targetMask;
    public Color dotHighlightColor;
    Color originalDotColor;
    
    void Start() {
        originalDotColor = dot.color;
        Cursor.visible = false;
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * 80 * Time.deltaTime);
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
