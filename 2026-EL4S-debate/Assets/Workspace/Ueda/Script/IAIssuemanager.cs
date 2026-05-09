using TMPro;
using UnityEngine;
using static APICommunicator;

public class IAIssuemanager : MonoBehaviour
{
    //BattleCombinedResultをもつ関数

    //
    [Header("意見を言う")]
    [SerializeField]
    GameObject isuuePanel;

    [Header("Aサイドキャラクタ")]
    [SerializeField]
    Sprite a_sprite;

    [Header("Bサイドキャラクタ")]
    [SerializeField]
    Sprite b_sprite;

    [Header("ニュートラル意見")]
    [SerializeField]
    TextMeshProUGUI tmp_text;

    [Header("親")]
    [SerializeField]
    private Transform parentObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ReflecttionAnswer(BattleCombinedResult result)
    {
        var sideA = result.side_a_result;
        var sideB = result.side_b_result;
        var nuetral = result.neutral_result;

        var obj = Instantiate(isuuePanel);

        obj.GetComponent<PanelSetting>().IssueSettings(a_sprite, sideA.output_result);
        obj.transform.SetParent(parentObj, false);

        obj = Instantiate(isuuePanel);

        obj.GetComponent<PanelSetting>().IssueSettings(b_sprite, sideB.output_result);
        obj.transform.SetParent(parentObj, false);

        tmp_text.text = nuetral.comment;
    }
}
