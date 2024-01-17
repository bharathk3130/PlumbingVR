using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    enum Scene
    {
        MainMenu,
        GameScene,
    }

    [SerializeField] Scene targetScene;
    [SerializeField] Button playButton;
    
    Animator anim;

    float transitionTime = 1.2f;


    void Start()
    {
        anim = GetComponent<Animator>();

        playButton.onClick.AddListener(() =>
        {
            anim.SetTrigger("SceneEnd");
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Called by an event in the "SceneEnd" animation
    public void LoadScene()
    {
        SceneManager.LoadScene(targetScene.ToString()); 
    }
}
