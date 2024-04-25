using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

public class PokeFeedback : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer pokeFeedbackRenderer;
    private Material pokeFeedbackRendererMaterial;

    [SerializeField]
    private VRPokeInputObserver pokeObserver;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(pokeFeedbackRenderer);
        Assert.IsNotNull(pokeObserver);

        pokeObserver.Poked += Trigger;

        pokeFeedbackRendererMaterial = new Material(pokeFeedbackRenderer.material);
        pokeFeedbackRenderer.material = pokeFeedbackRendererMaterial;
        maxAlphaValue = pokeFeedbackRendererMaterial.color.a;

        pokeFeedbackRendererMaterial.color = new Color(pokeFeedbackRendererMaterial.color.r,
                                                        pokeFeedbackRendererMaterial.color.g,
                                                        pokeFeedbackRendererMaterial.color.b,
                                                        0);
    }

    [Header("Fading"), SerializeField]
    private float durationIn = 0.25f;

    [SerializeField]
    private float durationOut = 1f;

    private float maxAlphaValue;
    private float currentAlphaValue => pokeFeedbackRendererMaterial.color.a;

    private bool fadeCoroutineRunning;
    private Coroutine fadingCoroutine;
    private bool feedbackCoroutineRunning;
    private Coroutine feedbackCoroutine;

    private void Trigger()
    {
        if (feedbackCoroutineRunning)
        {
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
            feedbackCoroutineRunning = false;
        }
        feedbackCoroutine = StartCoroutine(FeedbackRoutine());
    }

    private IEnumerator FeedbackRoutine()
    {
        feedbackCoroutineRunning = true;
        StartFeedback();

        yield return new WaitForSeconds(durationIn * (maxAlphaValue - currentAlphaValue) / maxAlphaValue);

        EndFeedback();
        feedbackCoroutineRunning = false;
    }

    void StartFeedback()
    {
        if (fadeCoroutineRunning)
        {
            if (fadingCoroutine != null)
                StopCoroutine(fadingCoroutine);
            fadingCoroutine = null;
            fadeCoroutineRunning = false;
        }

        fadingCoroutine = StartCoroutine(FadingCoroutine(durationIn * (maxAlphaValue - currentAlphaValue)/ maxAlphaValue, pokeFeedbackRendererMaterial, maxAlphaValue));
    }

    void EndFeedback()
    {
        if (fadeCoroutineRunning)
        {
            if (fadingCoroutine != null)
                StopCoroutine(fadingCoroutine);
            fadingCoroutine = null;
            fadeCoroutineRunning = false;
        }

        StartCoroutine(FadingCoroutine(durationOut * (currentAlphaValue / maxAlphaValue), pokeFeedbackRendererMaterial, 0));
    }


    private IEnumerator FadingCoroutine(float duration, Material material, float targetAlpha)
    {
        fadeCoroutineRunning = true;
        var timeStart = Time.time;
        var baseAlpha = material.color.a;

        while (Time.time < timeStart + duration && material.color.a != targetAlpha)
        {
            float value = baseAlpha + ((Time.time - timeStart) / duration) * (targetAlpha - baseAlpha);
            material.color = new Color(material.color.r,
                                        material.color.g,
                                        material.color.b,
                                        value);
            yield return new WaitForEndOfFrame();
        }
        material.color = new Color(material.color.r,
                                    material.color.g,
                                    material.color.b,
                                    targetAlpha);
        fadeCoroutineRunning = false;
    }

}
