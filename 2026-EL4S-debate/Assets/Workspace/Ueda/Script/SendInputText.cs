using UnityEngine;
using TMPro;

public class SendInputText : MonoBehaviour
{
    [Header("送るテキスト")]
    [SerializeField]
    private TMP_InputField inputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnClickButton()
    {
        if (inputField == null)
        {
            Debug.LogError("送るテキストが設定されていません。");
            return;
        }

        //なんか良い感じにテキストを送る

        //------------------------------

        Debug.Log("送るテキスト: " + inputField.text);

        //テキストはリセットする
        inputField.text = "";
    }
}
