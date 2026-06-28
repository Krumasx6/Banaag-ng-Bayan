using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Data Assets")]
    public CharacterData[] characters;        // CD_DatuSulo, CD_TalaLingayan, etc.

    [Header("Card Slots")]
    public Button[]     cardButtons;          // CardSlot0 - 3
    public GameObject[] cardBorders;          // Border child of each CardSlot
    public Animator[]   cardAnimators;        // Animator on each CardSlot root

    [Header("Bottom Buttons")]
    public Button backButton;
    public Button summonButton;

    [Header("Summon Button Colors")]
    public Color summonLockedColor = new Color(60f/255f,  60f/255f,  60f/255f,  235f/255f);
    public Color summonReadyColor  = new Color(183f/255f, 183f/255f, 183f/255f, 255f/255f);

    [Header("Fade Panel")]
    public Image fadePanel;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";
    public string gameplayScene = "TrainingGround";

    [Header("Timing")]
    public float fadeDuration = 0.5f;

    // ── Runtime ──────────────────────────────────────────────────────────────
    private int  _selected        = -1;
    private bool _isTransitioning = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // Wire card buttons
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => OnCardClicked(idx));
        }

        backButton.onClick.AddListener(OnBackClicked);
        summonButton.onClick.AddListener(OnSummonClicked);

        // Hide all borders at start
        foreach (GameObject border in cardBorders)
            if (border != null) border.SetActive(false);

        // Lock summon button at start
        SetSummonLocked(true);

        // Fade in from black on scene load
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnCardClicked(int index)
    {
        if (_isTransitioning) return;
        if (index < 0 || index >= characters.Length) return;

        _selected = index;

        // Only selected card shows border
        for (int i = 0; i < cardBorders.Length; i++)
            if (cardBorders[i] != null)
                cardBorders[i].SetActive(i == index);

        // Unlock summon button
        SetSummonLocked(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnSummonClicked()
    {
        if (_isTransitioning) return;
        if (_selected < 0) return;

        _isTransitioning = true;

        // Pass selected character to gameplay scene
        CharacterSelector.SelectedData = characters[_selected];

        StartCoroutine(SummonSequence());
    }

    IEnumerator SummonSequence()
    {
        // Play summon animation on selected card
        Animator anim = cardAnimators[_selected];
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.SetTrigger("Summon");
            yield return null;
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            float clipLength = state.length;
            if (clipLength > 0f)
                yield return new WaitForSeconds(clipLength);
            else
                yield return new WaitForSeconds(1f); // fallback
        }

        // Fade to black then load scene
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        SceneManager.LoadScene(gameplayScene);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnBackClicked()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(FadeAndLoad(mainMenuScene));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        SceneManager.LoadScene(sceneName);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Fade coroutine
    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = to;
        fadePanel.color = c;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────────────────
    void SetSummonLocked(bool locked)
    {
        summonButton.interactable = !locked;
        ColorBlock cb = summonButton.colors;
        cb.normalColor = locked ? summonLockedColor : summonReadyColor;
        summonButton.colors = cb;
    }
}