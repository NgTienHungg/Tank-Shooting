using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class DarkTransition : SingletonPersistent<DarkTransition>
{
    [SerializeField] private Image darkImage;
    [SerializeField] private float fadeDuration = 0.3f;

    public void FadeIn()
    {
        darkImage.gameObject.SetActive(true);
        darkImage.DOFade(1f, fadeDuration);
    }

    public void FadeOut()
    {
        darkImage.DOFade(0f, fadeDuration)
            .OnComplete(Hide);
    }

    private void Start() => Hide();

    private void Hide()
    {
        darkImage.gameObject.SetActive(false);
        darkImage.DOFade(0, 0);
    }

    public async void LoadSceneAsync(string sceneName, Action callback = null)
    {
        // Start loading the scene
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        // Effect fade out
        darkImage.gameObject.SetActive(true);
        darkImage.DOFade(1f, fadeDuration);

        // Wait for both the scene to load and the fade out complete
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => scene.progress >= 0.9f),
            UniTask.Delay(TimeSpan.FromSeconds(fadeDuration))
        );

        // Activate the next scene & Fade in
        scene.allowSceneActivation = true;
        await UniTask.DelayFrame(3);
        darkImage.DOFade(0f, fadeDuration).OnComplete(Hide);

        callback?.Invoke();
    }
}