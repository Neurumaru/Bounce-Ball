using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseMapedit : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 createPos = new Vector2(Mathf.Floor(worldPosition.x ) + 1/2f, Mathf.Floor(worldPosition.y)  + 1/ 2f);
        transform.position = createPos;
    }
}
