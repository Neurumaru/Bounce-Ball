using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{

    private void OnMouseOver()
    {

        //��Ŭ�� Ȥ�� ��Ŭ���� ����.
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            Destroy(this.gameObject);
        }
    }

}