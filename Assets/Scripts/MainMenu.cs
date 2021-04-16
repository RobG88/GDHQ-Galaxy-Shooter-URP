using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Galaxy;
    [SerializeField] GameObject Shooter;
    [SerializeField] GameObject TWOD;
    [SerializeField] GameObject CodedByText;
    [SerializeField] GameObject NewGameButton;
    [SerializeField] GameObject GDHQSheild;
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
    }
}
