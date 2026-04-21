using Michsky.UI.Dark;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public UIDissolveEffect dissolveEffect;
    public float fadeTime = 1f;

    public void PlayGame()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        UIDissolveEffect dissolve = dissolveEffect;
        dissolve.location = 1f; 

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            dissolve.location = 1f - (elapsed / fadeTime);
            yield return null;
        }

        SceneManager.LoadScene("InGame");
    }
}