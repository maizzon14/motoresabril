using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1;
    }
    public void TryAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
