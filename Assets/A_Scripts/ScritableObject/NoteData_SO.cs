using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNoteData", menuName = "Note Data")]
public class NoteData_SO : ScriptableObject
{
    [Header("音符基本信息")]
    public Sprite image;
    public NoteType type;           // 音符类型：普通、连按、长按等
    public int lane;                // 在哪个音轨上（0-5）
    public float time;              // 什么时候出现（秒）

    [Header("长按音符专用")]
    public float holdDuration;      // 如果是长按音符，持续多长时间

    [Header("滑动音符专用")]
    public SwipeDirection swipeDirection;  // 滑动方向

    // 枚举：音符类型
    public enum NoteType
    {
        Normal,     // 普通音符 - 点击一下
        Hold,
        Chain,      // 连按音符 - 连续点击    
        SwipeDown,  // 向下划音符
        SwipeUp,    // 向上划音符

    }

    // 枚举：滑动方向
    public enum SwipeDirection
    {
        Up,
        Down,

    }
}
