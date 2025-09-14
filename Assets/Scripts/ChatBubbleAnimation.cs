using UnityEngine;
using System.Collections;

public class ChatBubbleAnimation : MonoBehaviour
{
    [Header("�A�j���[�V�����ݒ�")]
    public float animationDuration = 0.3f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        // �K�v�ȃR���|�[�l���g���擾/�ǉ�
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // ������Ԃ�ݒ�i��\���j
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
    }

    void Start()
    {
        // �A�j���[�V�����J�n
        StartCoroutine(PlayEntranceAnimation());
    }

    // �o��A�j���[�V����
    IEnumerator PlayEntranceAnimation()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // �t�F�[�h�C��
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            // �X�P�[���A�j���[�V����
            float scaleValue = scaleCurve.Evaluate(progress);
            rectTransform.localScale = Vector3.one * scaleValue;

            yield return null;
        }

        // �ŏI��Ԃ��m���ɐݒ�
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
    }

    // �ޏ�A�j���[�V�����i�����g�p�j
    public IEnumerator PlayExitAnimation()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);

            yield return null;
        }
    }
}
