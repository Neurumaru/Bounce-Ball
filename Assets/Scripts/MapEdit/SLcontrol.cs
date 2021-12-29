using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.IO;

class Data2
{
    public int data;
    public Data2(int num)
    {
        this.data = num;
    }
}
public class SLcontrol : MonoBehaviour
{
    public Text tx;
    List<Data2> usin = new List<Data2>();

    // Start is called before the first frame update
    void Start()
    {
        usin.Add(new Data2(100));
        usin.Add(new Data2(1000));
        usin.Add(new Data2(10));
       
        print(usin);
    }

    public void _save()
    {
        string json = JsonConvert.SerializeObject(usin);
        File.WriteAllText(Application.dataPath + "/test.json", json);
    }

    public void _load()
    {
        string json = File.ReadAllText(Application.dataPath + "/test.json");
        usin = JsonConvert.DeserializeObject<List<Data2>>(json);
        foreach (Data2 key in usin)
        {
            print(key.data);
        }
    }
}
