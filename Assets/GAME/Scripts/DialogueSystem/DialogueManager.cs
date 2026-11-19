using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public Character character;
        public Action onNext;
    }

    [SerializeField] private GameObject dialogue;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject character;
    [SerializeField] private TextMeshProUGUI characterText;
    [SerializeField] private GameObject note;
    
    private SpriteRenderer dialogueSprite;
    private SpriteRenderer characterSprite;
    private SpriteRenderer noteSprite;

    private Vector2 noteStartPos;

    private DialogueLine[] lines;
    private float textSpeed = 0.05f;

    private int index;
    private bool dialogueOpen = false;

    private Tween noteTween;
    private Sequence noteHideSequence;
    private Sequence dialogueSequence;

    public InputActionReference interactAction;
    private bool isTextLoaded = false;
    private bool canSkipDialogue = true;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        dialogueSprite = dialogue.GetComponent<SpriteRenderer>();
        characterSprite = character.GetComponent<SpriteRenderer>();
        noteSprite = note.GetComponent<SpriteRenderer>();

        SetAlpha(dialogueSprite, 0f);
        SetAlpha(characterSprite, 0f);
        SetAlpha(noteSprite, 0f);

        dialogue.SetActive(false);
        character.SetActive(false);
        note.SetActive(false);

        dialogueText.text = "";
        noteStartPos = note.transform.localPosition;
    }

    void SetAlpha(SpriteRenderer sprite, float alpha)
    {
        Color color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    void Update()
    {
        if (!dialogueOpen) return;

        if (interactAction.action.WasPressedThisFrame() && isTextLoaded)
        {
            if (dialogueText.maxVisibleCharacters == dialogueText.text.Length)
            {
                StopArrow();
                NextLine();
            }
            else
            {
                if (!canSkipDialogue) return;

                StopAllCoroutines();
                dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                StartArrow();
            }
        }
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        PlayerController.Instance.StopPlayer();

        transform.DOMove(PlayerController.Instance.transform.position, 0);

        dialogueSequence?.Kill();

        lines = dialogueData.lines;
        index = 0;

        dialogue.SetActive(true);

        dialogueSprite.DOKill();
        SetAlpha(dialogueSprite, 0f);

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        characterText.text = "";

        dialogueSequence = DOTween.Sequence();

        dialogueSequence.Append(
            dialogueSprite.DOFade(1f, 0.3f)
        );

        dialogueSequence.OnComplete(() =>
        {
            isTextLoaded = true;
            StartCoroutine(TypeLine());
            dialogueOpen = true;
        });
    }

    void HideDialogue()
    {
        dialogueSequence?.Kill();

        dialogueSequence = DOTween.Sequence();

        dialogueSequence.Join(
            dialogueSprite.DOFade(0f, 0.15f)
        );

        dialogueSequence.Join(
            dialogueText.GetComponent<CanvasGroup>().DOFade(0f, 0.15f)
        );

        dialogueSequence.OnComplete(() =>
        {
            dialogue.SetActive(false);
            dialogueOpen = false;
            PlayerController.Instance.ResumePlayer();
            PlayerController.Instance.ResumeLocomotion();
            isTextLoaded = false;
        });
    }

    IEnumerator TypeLine()
    {
        Character currentChar = lines[index].character;

        dialogueText.text = lines[index].text;
        dialogueText.maxVisibleCharacters = 0;

        if (currentChar != null)
            characterText.text = currentChar.characterName;
        else
            characterText.text = "";

        for (int i = 0; i < dialogueText.text.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;

            if (currentChar != null && i % 2 == 0 && currentChar.sound != null)
            {
                // Sonido
            }

            yield return new WaitForSeconds(textSpeed);
        }

        StartArrow();
    }

    void NextLine()
    {
        lines[index].onNext?.Invoke();
        index++;

        StopArrow();

        if (index < lines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }
        else
        {
            HideDialogue();
        }
    }

    void StartArrow()
    {
        note.SetActive(true);

        noteTween?.Kill();
        noteSprite.DOKill();

        noteSprite.color = new Color(
            noteSprite.color.r,
            noteSprite.color.g,
            noteSprite.color.b,
            0f
        );

        note.transform.localPosition = noteStartPos;

        noteSprite.DOFade(1f, 0.1f);

        noteTween = note.transform.DOLocalMoveY(noteStartPos.y - 0.1f, 0.4f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    void StopArrow()
    {
        noteTween?.Kill();

        noteHideSequence?.Kill();

        noteHideSequence = DOTween.Sequence();

        noteHideSequence.Join(noteSprite.DOFade(0f, 0.1f));

        noteHideSequence.OnComplete(() =>
        {
            note.gameObject.SetActive(false);
        });
    }

    public bool IsDialogueOpen()
    {
        return dialogueOpen;
    }
}