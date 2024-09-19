using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

// コメントの生成・移動・削除

public class CommentManager : SingletonMonoBehaviour<CommentManager>
{
    const float CommentHeight = 90.0f;      // コメントの高さ   
    const int CommentLineHeight = 12;       // コメントの縦の行数
    const int MaxCommentDraw = 100;           // 一行に表示できるコメントの数

    [SerializeField] GameObject m_commentPrefab;
    [SerializeField] GameObject m_carryCommentPrefab;
    [SerializeField] float m_commentDrawTime = 4.0f;     // コメントの表示時間

    GameObject[][] m_commentArray = new GameObject[CommentLineHeight][];    // 表示しているコメントの配列
    List<GameObject> m_nextCommentList = new List<GameObject>();    // 次のフレームで表示するコメントのリスト
    List<GameObject> m_deleteCommentList = new List<GameObject>();  // 次のフレームで削除するコメントのリスト
    RectTransform m_canvasRectTransform;

    private void Awake()
    {
        // コメントを持つ配列の初期化
        for(int i = 0; i < CommentLineHeight; i++)
        {
            m_commentArray[i] = Enumerable.Repeat<GameObject>(null, MaxCommentDraw).ToArray();
        }

        m_canvasRectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // 配列に入っているコメントを右から左へ移動させる
        for(int i = 0; i < CommentLineHeight; i++)
        {
            for(int j = 0; j < MaxCommentDraw; j++)
            {
                // 空ならスキップ
                if (m_commentArray[i][j] == null)
                {
                    continue;
                }

                TextMeshProUGUI textMeshProUGUI = m_commentArray[i][j].GetComponent<TextMeshProUGUI>();

                // 総移動距離を取得
                float length;
                length = (m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2) * 2;

                // コメントが表示範囲外にいたら削除リストに登録
                if (-(length / 2) > textMeshProUGUI.rectTransform.localPosition.x)
                {
                    RemoveComment(i, j);
                }

                // 総移動距離と前フレームからの経過時間で移動量を出す
                float move = length * (Time.deltaTime / m_commentDrawTime);
                Vector3 pos = textMeshProUGUI.rectTransform.localPosition - new Vector3(move, 0, 0);
                textMeshProUGUI.rectTransform.localPosition = pos;
            }
        }

        // 削除するコメントがあれば削除する
        while(m_deleteCommentList.Count != 0)
        {
            // コメントを削除する
            GameObject deleteComment = m_deleteCommentList[0];
            m_deleteCommentList.Remove(deleteComment);
            Destroy(deleteComment);
        }

        // 配列を整理
        for (int i = 0; i < CommentLineHeight; i++)
        {
            for (int j = 0; j < MaxCommentDraw; j++)
            {
                if (m_commentArray[i][j] == null) continue;
                if (j - 1 < 0) continue;
                if (m_commentArray[i][j - 1] != null) continue;

                m_commentArray[i][j - 1] = m_commentArray[i][j];
                m_commentArray[i][j] = null;
            }
        }

        // 表示するコメントがあれば生成して配列に登録
        while(m_nextCommentList.Count != 0)
        {
            // 配列の空きをチェック
            Vector2Int num = CheckCommentArray(m_nextCommentList[0].GetComponent<TextMeshProUGUI>());

            // 空きがなければスキップ
            if (num.x == -1 || num.y == -1) break;

            // コメントを挿入
            AddCommentArray(m_nextCommentList[0]);
            m_nextCommentList.Remove(m_nextCommentList[0]);
        }
    }

    // コメントの生成と初期設定
    public GameObject InstantiateComment(string text, bool canCarry)
    {
        // コメントの生成
        GameObject comment;
        if (canCarry == false)
        {
            comment = Instantiate(m_commentPrefab, transform);
        }
        else
        {
            comment = Instantiate(m_carryCommentPrefab, transform);
            comment.GetComponent<CarryPlayerComment>().m_canvas = GetComponent<Canvas>();
        }

        TextMeshProUGUI textMeshProUGUI = comment.GetComponent<TextMeshProUGUI>();

        // テキストを挿入
        textMeshProUGUI.text = text;

        // 幅をテキストに合わせる
        textMeshProUGUI.rectTransform.sizeDelta = new Vector2(textMeshProUGUI.preferredWidth, CommentHeight);

        // 画面の右端に移動
        Vector3 pos = Vector3.zero;
        pos.x = m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2;
        textMeshProUGUI.rectTransform.localPosition = pos;

        return comment;
    }

    // 配列にコメントを挿入
    void AddCommentArray(GameObject comment)
    {
        TextMeshProUGUI textMeshProUGUI = comment.GetComponent<TextMeshProUGUI>();

        // コメントの配列番号を取得
        Vector2Int arrayNum = CheckCommentArray(textMeshProUGUI);

        // 縦の座標を設定
        Vector3 commentPos = textMeshProUGUI.rectTransform.localPosition;
        commentPos.y = (m_canvasRectTransform.rect.height / 2 - CommentHeight / 2) - CommentHeight * arrayNum.x;
        textMeshProUGUI.rectTransform.localPosition = commentPos;

        // 配列に挿入
        m_commentArray[arrayNum.x][arrayNum.y] = comment;
    }

    public Vector2Int CheckCommentArray(TextMeshProUGUI textMeshProUGUI)
    {
        // なるべく上の段にコメントを挿入する
        // かぶらず表示できる段がなければ-1,-1を返す
        for (int i = 0; i < CommentLineHeight; i++)
        {
            for (int j = 0; j < MaxCommentDraw; j++)
            {
                if (m_commentArray[i][j] != null) continue;

                // 前にコメントがある場合
                if (j > 0)
                {
                    TextMeshProUGUI frontComment = m_commentArray[i][j - 1].GetComponent<TextMeshProUGUI>();

                    float frontCommentRightPosX =
                        frontComment.rectTransform.localPosition.x + frontComment.rectTransform.rect.width / 2;

                    // まだコメント全体が画面に入っていなければ下の段に移動
                    if (frontCommentRightPosX > m_canvasRectTransform.rect.width / 2)
                    {
                        j = MaxCommentDraw;
                        continue;
                    }

                    float commentLeftPosX =
                        textMeshProUGUI.rectTransform.localPosition.x - (textMeshProUGUI.rectTransform.rect.width / 2);
                    float frontCommentLength = (m_canvasRectTransform.rect.width / 2 + frontComment.rectTransform.rect.width / 2) * 2;
                    float commentLength = (m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2) * 2;
                    float frontCommentSpeed = frontCommentLength / m_commentDrawTime;
                    float commentSpeed = commentLength / m_commentDrawTime;

                    // 前のコメントのほうが遅い場合
                    if (frontCommentSpeed < commentSpeed)
                    {
                        float frontCommentRestDistance = m_canvasRectTransform.rect.width / 2 + frontCommentRightPosX;
                        float restTime = frontCommentRestDistance / frontCommentSpeed;

                        // 表示中にかぶってしまう場合は下の段に移動
                        if (frontCommentRightPosX - frontCommentSpeed * restTime > commentLeftPosX - commentSpeed * restTime)
                        {
                            j = MaxCommentDraw;
                            continue;
                        }
                    }
                }

                return new Vector2Int(i, j);
            }
        }

        // 見つからなければ空いている場所を上から探す
        for (int i = 0; i < MaxCommentDraw; i++)
        {
            for (int j = 0; j < CommentLineHeight; j++)
            {
                // 空ならスキップ
                if (m_commentArray[j][i] != null) continue;

                return new Vector2Int(j, i);
            }
        }

        // まったく空きがない場合は-1,-1を返す
        return new Vector2Int(-1,-1);
    }

    // 予約コメントのリストに登録
    public void AddNextCommentList(GameObject comment)
    {
        m_nextCommentList.Add(comment);
    }

    // 削除リストに登録
    void RemoveComment(int height, int j)
    {
        m_deleteCommentList.Add(m_commentArray[height][j]);
    }
}
