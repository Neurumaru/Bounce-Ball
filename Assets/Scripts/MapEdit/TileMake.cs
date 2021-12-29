using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMake : MonoBehaviour
{
    public int ID;
    public bool Clicked = false; // ��ư ų�� Ȯ�ο� 
    //private bool useing = false; // �Ķ� map�� ��������� Ȯ�ο�
    private MapEdit editer;


    void Start()
    {
        editer = GameObject.FindGameObjectWithTag("MapEdit").GetComponent<MapEdit>();
    }

    public void ButtonClicked()
    {

        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 createPos = new Vector2(Mathf.Floor(worldPosition.x / editer.width) * editer.width + editer.width / 2f, Mathf.Floor(worldPosition.y / editer.height) * editer.height + editer.height / 2f);
        editer.num = ID;
        Destroy(GameObject.FindGameObjectWithTag("blueMap"));
        //Instantiate(editer.ItemImage[ID], new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
        Instantiate(editer.ItemImage[ID], new Vector3(createPos.x, createPos.y, 0), Quaternion.identity);
        editer.DontUseOther();
    }
}
 