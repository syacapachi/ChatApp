using UnityEngine;

public class SizeEdit : MonoBehaviour
{
    public RectTransform Text;
    RectTransform RectTransform;
    public Vector2 Size = new();
    public Vector2 Space = new();
    public void Start()
    {
        RectTransform = GetComponent<RectTransform>();
    }
    public void Update()
    {
        RectTransform.sizeDelta = Text.sizeDelta * Size +Space;
    }
}
