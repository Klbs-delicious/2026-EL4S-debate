using UnityEngine;
using System.Collections.Generic;

public class CommentSet : MonoBehaviour
{
    [SerializeField]
    private List<string> comments = new List<string>();

    // コメント追加
    public void AddComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return;
        }

        comments.Add(comment);
    }

    // 配列として取得
    public string[] GetPosts()
    {
        return comments.ToArray();
    }

    // 全消去
    public void ClearComments()
    {
        comments.Clear();
    }
}