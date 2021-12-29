using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSize : MonoBehaviour
{
    private int width = 1366;
    private int height = 768;

    // Start is called before the first frame update
    void Start()
    {

        Screen.SetResolution(width, height,true);
    }

    // Update is called once per frame
    void Update()
    {


    }
}
