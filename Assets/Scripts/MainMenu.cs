using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Galaxy;
    [SerializeField] GameObject Shooter;
    [SerializeField] GameObject TWOD;
    [SerializeField] GameObject CodedByText;
    [SerializeField] GameObject NewGameButton;
    [SerializeField] GameObject GDHQSheild;
    //[SerializeField] GameObject LoadingProgress;
    [SerializeField] Slider SceneLoadingSlider;
    public void LoadGame()
    {
        SceneManager.LoadScene(1); // Start Game
        //SceneManager.LoadScene("GalaxyShooterDemo");
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);

        Galaxy.SetActive(true);
        Shooter.SetActive(true);
        TWOD.SetActive(true);

        yield return new WaitForSeconds(2f);

        CodedByText.SetActive(true);

        yield return new WaitForSeconds(.5f);

        GDHQSheild.SetActive(true);

        yield return new WaitForSeconds(.5f);

        NewGameButton.SetActive(true);
    }

    public void LevelLoader(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex);

        SceneLoadingSlider.enabled = true;

        while (!asyncOp.isDone)
        {
            float progress = Mathf.Clamp01(asyncOp.progress / .9f);

            Debug.Log("Scene Loading Progress: " + progress);
            Debug.Log("Scene Loading Compplete:" + asyncOp.isDone);

            SceneLoadingSlider.value = progress;

            yield return null;
        }
    }
}
