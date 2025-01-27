using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string NameLoadeScene;
    public Button LoadSceneButton;

    private void Start()
    {
        LoadSceneButton.onClick.AddListener(LoadScene);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(NameLoadeScene);
    }
}
