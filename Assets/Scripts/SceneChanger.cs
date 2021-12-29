using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void LoadMapSelect()
    {
        SceneManager.LoadScene("MapSelect");
    }
    public void LoadMapList()
    {
        SceneManager.LoadScene("MapList");
    }
    public void LoadMaptest()
    {
        SceneManager.LoadScene("Maptest");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMap1()
    {
        SceneManager.LoadScene("Map1");
    }

    public void LoadMap2()
    {
        SceneManager.LoadScene("Map2");
    }
    public void LoadMap3()
    {
        SceneManager.LoadScene("Map3");
    }
}
