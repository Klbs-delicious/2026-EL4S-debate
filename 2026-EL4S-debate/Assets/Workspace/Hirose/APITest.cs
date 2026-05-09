/*
 * 最終更新日時：2026/05/08
 * 作成者：廣瀬宗貴
 *
 * 概要：
 * APICommunicatorを使って、AIレスバトルのAPI送信テストを行うクラス。
 * 1回のAPI送信で、両陣営の出力結果とニュートラルAIの判定結果を取得する。
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class APITest : MonoBehaviour
{
    [SerializeField] private APICommunicator api;

    private void Start()
    {
        StartCoroutine(RunBattleTest());
    }

    private IEnumerator RunBattleTest()
    {
        APICommunicator.BattleRequestData battleData =
            CreateBattleData();

        yield return StartCoroutine(api.SendBattleRequest(
            battleData,
            result =>
            {
                OutputResult(result);
            },
            error =>
            {
                Debug.LogError("API失敗: " + error);
            }
        ));
    }

    private APICommunicator.BattleRequestData CreateBattleData()
    {
        return new APICommunicator.BattleRequestData
        {
            battle_id = "battle_001",
            round = 1,
            topic = "犬派 vs 猫派",

            side_a = new APICommunicator.SideRequestData
            {
                side = "犬派",
                comments = PostData.GetPythonPosts(),
                previous_opponent_output = ""
            },

            side_b = new APICommunicator.SideRequestData
            {
                side = "猫派",
                comments = PostData.GetCppPosts(),
                previous_opponent_output = ""
            },

            previous_neutral_comment = ""
        };
    }

    private void OutputResult(APICommunicator.BattleCombinedResult result)
    {
        Debug.Log("========== AIレスバトル生成結果 ==========");

        Debug.Log("【" + result.side_a_result.side + "】");
        Debug.Log(result.side_a_result.output_result);

        Debug.Log("【" + result.side_b_result.side + "】");
        Debug.Log(result.side_b_result.output_result);

        Debug.Log("【ニュートラルAI判定】");
        Debug.Log("スコア: " + result.neutral_result.score);
        Debug.Log("選択陣営: " + result.neutral_result.selected_side);
        Debug.Log("一言コメント: " + result.neutral_result.comment);
    }
}