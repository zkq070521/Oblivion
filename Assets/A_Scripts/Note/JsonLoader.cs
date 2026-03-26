using System.Collections;
using System.Collections.Generic;//用于使用List
using UnityEngine;
using System.IO;//用于读写文件

public class JsonLoader : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    public Sprite[] allNoteSprites; //指向音符生成器，等会儿要把翻译好的数据给它
    void Start()
    {
        LoadSongFromJson();//游戏一启动就开始读取json文件
    }

    void LoadSongFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "song1.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            SongDatabase database = JsonUtility.FromJson<SongDatabase>(jsonContent);

            if (database != null && database.songs.Count > 0)
            {
                SongData song = database.songs[0];
                ConvertJsonToSO(song);
            }
        }
    }


    void ConvertJsonToSO(SongData songData)
    {
        List<NoteData_SO> chart = new List<NoteData_SO>();

        foreach (var jsonNote in songData.notes)
        {
            // 创建ScriptableObject实例
            NoteData_SO noteSO = ScriptableObject.CreateInstance<NoteData_SO>();

            // 设置基本信息
            noteSO.time = jsonNote.time;
            noteSO.lane = jsonNote.lane;
            noteSO.type = (NoteData_SO.NoteType)jsonNote.type;

            // 长按音符的特殊处理
            if (noteSO.type == NoteData_SO.NoteType.Hold)
            {
                noteSO.holdDuration = jsonNote.holdDuration;
            }

            // 滑动音符的特殊处理
            if (noteSO.type == NoteData_SO.NoteType.SwipeDown ||
                noteSO.type == NoteData_SO.NoteType.SwipeUp)
            {
                noteSO.swipeDirection = (NoteData_SO.SwipeDirection)jsonNote.swipeDirection;
            }

            Sprite noteSprite = FindSpriteByName(jsonNote.spriteName);

            if (noteSprite != null)
            {
                noteSO.image = noteSprite;

            }
            else
            {
                Debug.LogWarning($"找不到图片: {jsonNote.spriteName}，使用默认图片");
                //noteSO.image = GetDefaultSprite();
            }

            chart.Add(noteSO);
        }

        noteSpawner.chart = chart;

    }

    // 根据名字在切割的图片中查找
    Sprite FindSpriteByName(string spriteName)
    {
        if (allNoteSprites == null) return null;

        // 精确匹配
        foreach (var sprite in allNoteSprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }

        // 如果精确匹配找不到，尝试忽略大小写
        foreach (var sprite in allNoteSprites)
        {
            if (sprite.name.ToLower() == spriteName.ToLower())
            {
                return sprite;
            }
        }

        return null;
    }
    // 根据音符类型获取默认图片
    // Sprite GetDefaultSpriteByType(NoteData_SO.NoteType type)
    // {
    //     string spriteName = type switch
    //     {
    //         NoteData_SO.NoteType.Normal => "note_normal",
    //         NoteData_SO.NoteType.Hold => "note_hold",
    //         NoteData_SO.NoteType.SwipeDown => "note_swipe",
    //         NoteData_SO.NoteType.SwipeUp => "note_swipe_up",
    //         _ => "note_default"
    //     };

    //     return Resources.Load<Sprite>($"Notes/{spriteName}");
    // }
}
