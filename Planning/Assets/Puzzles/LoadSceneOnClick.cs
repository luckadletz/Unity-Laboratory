using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class LoadSceneOnClick : MonoBehaviour
{

    public string levelName;

    public void Start()
    {
        GetComponentInChildren<Text>().text = levelName;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
