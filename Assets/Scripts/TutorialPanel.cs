using System.Collections;
using UnityEngine;
using Michsky.UI.Dark;

public class TutorialPanel : MonoBehaviour
{
    public ModalWindowManager modalWindow;
    public SceneTransition sceneTransition;

    public void ShowTutorial()
    {
        modalWindow.ModalWindowIn();
    }

    public void OnConfirm()
    {
        modalWindow.ModalWindowOut();
        StartCoroutine(WaitAndStart());
    }

    IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(modalWindow.disableAfter);
        sceneTransition.PlayGame();
    }
}