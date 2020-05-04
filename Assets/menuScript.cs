using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour
{
    public void Soczewka()
    {
        SceneManager.LoadScene(1);
    }
    public void Lustro()
    {
        SceneManager.LoadScene(2);
    }
    public void Peryskop()
    {
        SceneManager.LoadScene(3);
    }
    public void wyjdz()
    {
        Application.Quit();
    }
}
