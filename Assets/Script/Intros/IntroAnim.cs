// Unity namespaces
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnim : MonoBehaviour
{

    public void IntroAnimFinished()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
