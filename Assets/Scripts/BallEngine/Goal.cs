using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Goal");
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); // 충돌 대상 투명하게
            SceneManager.LoadScene("GameClear");
        }
    }
}
