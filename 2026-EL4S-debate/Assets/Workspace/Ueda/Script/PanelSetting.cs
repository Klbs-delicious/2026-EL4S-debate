using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelSetting : MonoBehaviour
{
    [Header("キャラテクスチャ")]
    [SerializeField]
    private Image charaImage;

    [Header("文章")]
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    public void IssueSettings(Sprite sprite, string sentence)
    {
        charaImage.sprite = sprite;
        textMeshProUGUI.text = sentence;
    }
}
