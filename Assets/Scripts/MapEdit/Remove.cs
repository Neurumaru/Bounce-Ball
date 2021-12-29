using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{

    private void OnMouseOver()
    {

        //우클릭 혹은 좌클릭시 삭제.
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            Destroy(this.gameObject);
        }
    }

}