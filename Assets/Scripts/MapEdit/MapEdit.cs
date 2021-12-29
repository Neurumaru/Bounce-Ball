using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class MapData
{
    public int num1;   // 게임 오브젝트 식별 번호
    public float X, Y; // X , y좌표용
    public MapData(int a, float x, float y)
    {
        this.num1 = a;
        this.X = x;
        this.Y = y;
    }
}
public class MapEdit : MonoBehaviour
{
    public TileMake[] tileMakes;
    public GameObject[] itemPrefabs;
    public GameObject[] ItemImage;
    public int num;
    public int width, height;
    public string fileName; //파일이름 저자알것
    List<MapData> setting = new List<MapData>();
    private void Update()
    {
        //마우스 위치조정
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 createPos = new Vector2(Mathf.Floor(worldPosition.x / width) * width + width / 2f, Mathf.Floor(worldPosition.y / height) * height + height / 2f);//floor연산은 내림하는 연산 , 


        //좌클릭시
        if (Input.GetMouseButtonDown(0) && tileMakes[num].Clicked)
        {
            Instantiate(itemPrefabs[num], new Vector3(createPos.x, createPos.y, 0), Quaternion.identity);//마우스 위치에 가까운 grid에 생성

            for (int i = setting.Count - 1; i > 0; i--) {
                if ((setting[i].X == createPos.x) && (setting[i].Y == createPos.y)) {
                    setting.Remove(setting[i]);
                }
            }

            setting.Add(new MapData(num, createPos.x, createPos.y));

        }
        //우클릭시
        if (Input.GetMouseButtonDown(1))
        {
            tileMakes[num].Clicked = false;
            Destroy(GameObject.FindGameObjectWithTag("blueMap"));

        }
    }

    public void StoreName(string a)
    {
        fileName = a;
        if (fileName.Contains(".json") == false)
        {
            fileName += ".json";
        }
        fileName = Path.Combine(Application.dataPath, fileName);
        fileName = Path.Combine(Application.persistentDataPath, fileName); 
    }
    public void PliceTile(int X, int Y, int id)
    {
        Instantiate(itemPrefabs[id], new Vector3(X, Y, 0), Quaternion.identity);//주어신 int 값에 맞는
    }
    //버튼클릭시 다른 친구들 클릭 안된걸로 설정 
    public void DontUseOther()
    {
        for (int i = 0; i < num; i++)
        {
            tileMakes[i].Clicked = false;
        }
        tileMakes[num].Clicked = true;
    }

    public void _save() // json 파일로 저장
    {
        print("Saveing...");
        string jsonFile = JsonConvert.SerializeObject(setting);
        File.WriteAllText(fileName, jsonFile);
    }

    public void _load() //json 파일 불러오기 
    {
        string jsonFile;
        print("Loading...");
        try {
             jsonFile = File.ReadAllText(fileName);
        }
        catch (FileNotFoundException e) { print("파일명 존재 X");return; }
        jsonFile = File.ReadAllText(fileName);
        setting = JsonConvert.DeserializeObject<List<MapData>>(jsonFile);

        //json 파일을 토대로 맵 생성 
        foreach (MapData key in setting)
        {
            Instantiate(itemPrefabs[key.num1], new Vector3(key.X, key.Y, 0), Quaternion.identity);
        }
    }
}