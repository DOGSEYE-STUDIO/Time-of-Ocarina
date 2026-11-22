using DG.Tweening;
using DG.Tweening.Core.Easing;
using MelenitasDev.SoundsGood;
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
    private CanvasGroup textGroup;
    private CanvasGroup nameGroup;

    private Vector2 noteStartPos;

    private DialogueLine[] lines;
    private float textSpeed = 0.05f;

    private float characterShowY = -0.5f;
    private float characterHideY = -1.75f;
    private float characterLeftX = -4.3125f;
    private float characterRightX = 4.3125f;
    private float characterMoveDuration = 0.3f;

    private Sequence characterSequence;

    private bool isCharacterVisible = false;
    private bool isCharacterOnRight = false;

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
        textGroup = dialogueText.GetComponent<CanvasGroup>();
        nameGroup = characterText.GetComponent<CanvasGroup>();

        SetAlpha(dialogueSprite, 0f);
        SetAlpha(noteSprite, 0f);

        dialogue.SetActive(false);
        character.SetActive(false);
        note.SetActive(false);

        character.transform.localPosition = new Vector3(
            characterLeftX,
            characterHideY,
            character.transform.localPosition.z
        );

        isCharacterVisible = false;
        isCharacterOnRight = false;

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

        Camera mainCamera = Camera.main;

        Vector3 cameraPosition = mainCamera.transform.position;

        transform.position = new Vector3(
            cameraPosition.x,
            cameraPosition.y,
            transform.position.z
        );

        dialogueSequence?.Kill();

        lines = dialogueData.lines;
        index = 0;

        dialogue.SetActive(true);

        dialogueSprite.DOKill();
        SetAlpha(dialogueSprite, 0f);

        textGroup.DOKill();
        textGroup.alpha = 1f;

        nameGroup.DOKill();
        nameGroup.alpha = 1f;

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        characterText.text = "";

        dialogueSequence = DOTween.Sequence();

        dialogueSequence.Append(
            dialogueSprite.DOFade(1f, 0.4f)
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
        characterSequence?.Kill();

        dialogueSequence = DOTween.Sequence();

        dialogueSequence.Join(
            dialogueSprite.DOFade(0f, 0.15f)
        );

        dialogueSequence.Join(
            textGroup.DOFade(0f, 0.15f)
        );

        if (isCharacterVisible)
        {
            dialogueSequence.Join(
                character.transform.DOLocalMoveY(
                    characterHideY,
                    0.3f
                ).SetEase(Ease.InOutQuad)
            );

            dialogueSequence.Join(
                characterSprite.DOFade(0f, 0.15f)
            );

            dialogueSequence.Join(
                nameGroup.DOFade(0f, 0.15f)
            );
        }

        dialogueSequence.OnComplete(() =>
        {
            dialogue.SetActive(false);

            if (isCharacterVisible)
            {
                character.SetActive(false);
                isCharacterVisible = false;
            }

            characterText.text = "";

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

        UpdateCharacter(currentChar);

        for (int i = 0; i < dialogueText.text.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;

            if (currentChar != null && i % 2 == 0 && char.IsLetterOrDigit(dialogueText.text[i]))
            {
                new Sound(SFX.Dialogo).SetOutput(Output.SFX).SetPitch(currentChar.tono * UnityEngine.Random.Range(0.9f, 1.1f)).Play();
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

    void UpdateCharacter(Character newCharacter)
    {
        characterSequence?.Kill();

        if (newCharacter == null)
        {
            if (!isCharacterVisible)
                return;

            characterSequence = DOTween.Sequence();

            characterSequence.Append(
                character.transform.DOLocalMoveY(
                    characterHideY,
                    characterMoveDuration
                ).SetEase(Ease.InOutQuad)
            );

            characterSequence.OnComplete(() =>
            {
                character.SetActive(false);
                isCharacterVisible = false;
            });

            return;
        }

        bool shouldBeOnRight = newCharacter.characterName == "  Tamboril";

        float targetX = shouldBeOnRight
            ? characterRightX
            : characterLeftX;

        characterText.text = newCharacter.characterName;

        if (!isCharacterVisible)
        {
            character.SetActive(true);

            characterSprite.DOKill();
            SetAlpha(characterSprite, 1f);

            character.transform.localPosition = new Vector3(
                targetX,
                characterHideY,
                character.transform.localPosition.z
            );

            isCharacterVisible = true;
            isCharacterOnRight = shouldBeOnRight;

            characterSequence = DOTween.Sequence();

            characterSequence.Append(
                character.transform.DOLocalMoveY(
                    characterShowY,
                    characterMoveDuration
                ).SetEase(Ease.OutQuad)
            );

            return;
        }

        if (isCharacterOnRight != shouldBeOnRight)
        {
            isCharacterOnRight = shouldBeOnRight;

            characterSequence = DOTween.Sequence();

            characterSequence.Append(
                character.transform.DOLocalMoveX(
                    targetX,
                    characterMoveDuration
                ).SetEase(Ease.InOutQuad)
            );
        }
    }

    public bool IsDialogueOpen()
    {
        return dialogueOpen;
    }
}