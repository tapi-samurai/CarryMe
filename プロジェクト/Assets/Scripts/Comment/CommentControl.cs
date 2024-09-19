using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

// コメントの生成速度の管理と命令

public class CommentControl : SingletonMonoBehaviour<CommentControl>
{
    [Header("Text")]
    [SerializeField] CommentData m_commentData;

    [Header("Value")]
    [SerializeField] float m_minInterval;               // コメントの最短生成間隔
    [SerializeField] float m_maxInterval;               // コメントの最長生成間隔
    [SerializeField] float m_minCarryCommentInterval;   // プレイヤーを担ぐコメントの最短生成間隔
    [SerializeField] float m_maxCarryCommentInterval;   // プレイヤーを担ぐコメントの最長生成間隔
    [SerializeField] int m_carryCommentHeight;          // プレイヤーを担ぐコメントを挿入する段数
    [SerializeField] int m_maxComplimentComment;        // プレイヤーをほめるコメントの最多生成数
    [SerializeField] int m_minComplimentComment;        // プレイヤーをほめるコメントの最少生成数

    CommentManager m_commentManager;
    List<CommentDataEntity> m_nowCommentList = new List<CommentDataEntity>();
    List<string> m_nextCommentList = new List<string>();

    float m_elapsedTime;
    float m_nextCommentInterval;
    float m_carryCommentElapsedTime;
    float m_nextCarryCommentInterval;

    private void Awake()
    {
        // 流すコメントのリストを初期化
        m_nowCommentList = m_commentData.normal;

        m_commentManager = GetComponent<CommentManager>();

        m_elapsedTime = 0;
        m_carryCommentElapsedTime = 0;
        m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);
        m_nextCarryCommentInterval = m_maxCarryCommentInterval;

        // 重くなるから最初に一度コメントを生成
        m_commentManager.AddNextCommentList(m_commentManager.InstantiateComment("初見です", false));
    }

    private void FixedUpdate()
    {
        m_elapsedTime += Time.deltaTime;
        if (StageCounter.CheckClearBelow((int)StageCounter.Item.Wall + 1))
        {
            m_carryCommentElapsedTime += Time.deltaTime;
        }

        if (m_elapsedTime >= m_nextCommentInterval)
        {
            // プレイヤーを担ぐコメントの生成は以下の条件を満たした場合のみ
            // ・ステージ上のオブジェクトがすべてなくなっている
            // ・プレイヤーを担ぐコメント用の必要時間が経過している
            // ・コメントの設定の段数に挿入できる
            if (
                StageCounter.CheckClear((int)StageCounter.Item.Wall) == true &&
                m_carryCommentElapsedTime >= m_nextCarryCommentInterval
                )
            {
                // 担ぐコメントを生成
                string carryCommentText = m_commentData.carry[Random.Range(0, m_commentData.carry.Count)].comment;
                GameObject carryComment = m_commentManager.InstantiateComment(carryCommentText, true);
                TextMeshProUGUI textMeshProUGUI = carryComment.GetComponent<TextMeshProUGUI>();

                // コメントを挿入できる段数が設定通りであれば
                if(m_commentManager.CheckCommentArray(textMeshProUGUI).x == m_carryCommentHeight - 1)
                {
                    // コメントを挿入
                    m_commentManager.AddNextCommentList(carryComment);

                    // 経過時間をリセット
                    m_elapsedTime = 0;
                    m_carryCommentElapsedTime = 0;

                    // インターバルを再設定
                    m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);
                    m_nextCarryCommentInterval = Random.Range(m_minCarryCommentInterval, m_maxCarryCommentInterval);

                    return;
                }

                // 担ぐコメントが挿入できなければ削除
                Destroy(carryComment);
            }

            // 通常コメントを生成して挿入
            // コメントリストに予約があればそれを流す
            // なければ完全ランダム
            GameObject comment;
            if(m_nextCommentList.Count == 0)
            {
                comment = m_commentManager.InstantiateComment(
                    m_nowCommentList[Random.Range(0, m_nowCommentList.Count)].comment, false);
            }
            else
            {
                comment = m_commentManager.InstantiateComment(
                    m_nextCommentList[0], false);

                // リストから削除
                m_nextCommentList.Remove(m_nextCommentList[0]);
            }

            m_commentManager.AddNextCommentList(comment);

            // 次のコメントを生成するまでの時間を再設定
            m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);

            // 経過時間をリセット
            m_elapsedTime = 0;
        }
    }

    void AddNextCommentList(string nextComment)
    {
        m_nextCommentList.Add(nextComment);
    }

    public async void ComplimentComment()
    {
        // プレイヤーをほめるコメントを大量に流す
        int i = Random.Range(m_minComplimentComment, m_maxComplimentComment);
        while(i > 0)
        {
            // コメントを生成
            GameObject comment = CommentManager.instance.InstantiateComment(
                m_commentData.homeru[Random.Range(0, m_commentData.homeru.Count)].comment, false);

            // 挿入
            CommentManager.instance.AddNextCommentList(comment);

            // カウントを減らす
            i--;

            // すこしディレイをかける
            await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0.0f, 0.1f)));
        }
    }

    public void GoalHintComment()
    {
        AddNextCommentList(m_commentData.hint[Random.Range(0, m_commentData.hint.Count)].comment);
    }

    public void End()
    {
        // エンディング時のコメントに差し替える
        m_nowCommentList = m_commentData.end;

        // コメントの頻度をあげる
        m_minInterval = 0;
        m_maxInterval = 0.2f;
    }
}
