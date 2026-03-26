using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;

    // 存储所有活跃的音符（在场景中存在的）
    private List<NoteBehaviour> activeNotes = new List<NoteBehaviour>();

    void Start()
    {
        // 定期更新活跃音符列表（或者通过事件更新，但你说不想改 NoteBehaviour）
        // 这里用定期更新的方式
        InvokeRepeating("UpdateActiveNotes", 0f, 0.1f);
    }

    void Update()
    {
        // 检测键盘输入
        if (Input.GetKeyDown(KeyCode.S))
        {
            TryJudgeLane(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TryJudgeLane(1);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryJudgeLane(2);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TryJudgeLane(3);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryJudgeLane(4);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            TryJudgeLane(5);
        }
    }

    /// <summary>
    /// 更新活跃音符列表（查找场景中所有 NoteBehaviour）
    /// </summary>
    void UpdateActiveNotes()
    {
        // 查找场景中所有 NoteBehaviour
        NoteBehaviour[] allNotes = FindObjectsOfType<NoteBehaviour>();
        activeNotes.Clear();
        activeNotes.AddRange(allNotes);
    }

    /// <summary>
    /// 尝试判定指定轨道
    /// </summary>
    void TryJudgeLane(int lane)
    {
        // 找到该轨道上到达判定线且未被判定的音符
        NoteBehaviour targetNote = GetEarliestPendingNote(lane);

        if (targetNote != null)
        {
            //  调用 TryJudge 方法
            bool success = targetNote.TryJudge(lane);


        }
        else
        {
            // Debug.Log($"⚠️ 音轨 {lane} 没有等待判定的音符");
        }
    }

    /// <summary>
    /// 获取指定轨道上等待判定的音符
    /// 判断标准：isAtJudgeLine = true 且 isJudged = false
    /// </summary>
    NoteBehaviour GetEarliestPendingNote(int lane)
    {
        NoteBehaviour earliestNote = null;
        float earliestArrivalTime = float.MaxValue;

        // 遍历所有活跃音符
        foreach (var note in activeNotes)
        {
            if (note == null) continue;

            // 检查是否是目标轨道、是否到达判定线、是否未被判定
            // 注意：isAtJudgeLine 是 private 字段，需要通过反射或者添加公共属性访问
            // 这里用反射（不推荐但可行），或者你可以告诉我可以添加一个公共属性
            bool isAtJudgeLine = IsNoteAtJudgeLine(note);
            bool isJudged = IsNoteJudged(note);

            if (note.noteData.lane == lane && isAtJudgeLine && !isJudged)
            {
                // 获取到达时间（需要通过反射或添加公共属性）
                float arrivalTime = GetNoteArrivalTime(note);

                if (arrivalTime < earliestArrivalTime)
                {
                    earliestArrivalTime = arrivalTime;
                    earliestNote = note;
                }
            }
        }

        return earliestNote;
    }

    /// <summary>
    /// 检查音符是否到达判定线（使用反射访问私有字段）
    /// </summary>
    bool IsNoteAtJudgeLine(NoteBehaviour note)
    {
        // 使用反射获取私有字段
        var field = typeof(NoteBehaviour).GetField("isAtJudgeLine",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (bool)field.GetValue(note);
        }
        return false;
    }

    /// <summary>
    /// 检查音符是否已被判定
    /// </summary>
    bool IsNoteJudged(NoteBehaviour note)
    {
        var field = typeof(NoteBehaviour).GetField("isJudged",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (bool)field.GetValue(note);
        }
        return false;
    }

    /// <summary>
    /// 获取音符到达判定线的时间
    /// </summary>
    float GetNoteArrivalTime(NoteBehaviour note)
    {
        var field = typeof(NoteBehaviour).GetField("arrivalTime",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (float)field.GetValue(note);
        }
        return 0f;
    }
}