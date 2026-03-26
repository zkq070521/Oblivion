using System;
using System.Collections.Generic;

// 整个JSON的根对象
[Serializable]
public class SongDatabase
{
    public List<SongData> songs;
}

// 单首歌曲的数据
[Serializable]
public class SongData
{
    public string songName;
    public float bpm;
    public float offset;
    public List<NoteJsonData> notes;
}

//单个音符的数据
[Serializable]
public class NoteJsonData
{
    public int lane;           // 音轨 0-5
    public float time;         // 时间（秒）
    public int type;           // 0=普通, 1=长按, 2=连按, 3=向下划等
    public float holdDuration; // 长按专用（可选）
    public int swipeDirection; // 滑动专用（可选）
    public string spriteName;  // 图片名称（可选）
}