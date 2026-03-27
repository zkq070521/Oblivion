using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NoteSpawner : MonoBehaviour
{
    [Header("游戏设置")]
    public float startDelay;  // 进入场景后等待几秒开始
    public bool autoStart = true;   // 是否自动开始

    //private bool isGameStarted = false;


    [Header("音符预制体")]
    public GameObject notePrefab;  // 拖入我们做的Note_Prefab
    public GameObject effectPrefab;

    [Header("歌曲信息")]
    public AudioSource audioSource;

    [Header("谱面数据")]
    public List<NoteData_SO> chart = new List<NoteData_SO>();  // 在这里手动添加音符

    private int nextNoteIndex = 0;
    private bool isPlaying = false;
    public bool isMusicStart;
    public float sceneStartTime;

    void Start()
    {
        sceneStartTime = Time.deltaTime;

        // 按时间排序所有音符
        chart.Sort((a, b) => a.time.CompareTo(b.time));
        isPlaying = true;
        isMusicStart = false;


        // 开始播放
        if (autoStart)
        {
            // 延迟开始游戏
            StartCoroutine(DelayedGameStart());
        }
    }

    IEnumerator DelayedGameStart()
    {
        Debug.Log($"等待 {startDelay} 秒后开始游戏...");

        // 可以在这里显示倒计时 UI
        yield return new WaitForSeconds(startDelay);

        // 开始游戏
        StartGame();
        isMusicStart = true;
    }


    void StartGame()
    {
        //isGameStarted = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        Debug.Log("歌曲开始播放！");
    }

    void Update()
    {
        // if (!isMusicStart)
        // {
        //     // 音乐还没开始，用场景时间生成负数时间的音符
        //     float sceneTime = Time.time - sceneStartTime;

        //     while (nextNoteIndex < chart.Count)
        //     {
        //         NoteData_SO noteData = chart[nextNoteIndex];

        //         // 负数时间：场景时间到了就生成
        //         if (-noteData.time <= sceneTime + 1f && noteData.time < 0)
        //         {
        //             SpawnNote(noteData);
        //             nextNoteIndex++;
        //         }
        //         else break;
        //     }

        // 检查是否所有引导音符都被击中了
        //     if (allGuideNotesHit)
        // {
        //         isMusicStarted = true;
        //         audioSource.Play();
        //     }

        //     return;
        // }

        if (!isPlaying) return;

        // 获取当前播放时间
        float currentTime = audioSource.time;
        //Debug.Log($"当前播放时间: {currentTime}");

        // 生成应该出现的音符
        while (nextNoteIndex < chart.Count)
        {
            NoteData_SO noteData = chart[nextNoteIndex];



            // 如果音符的时间 <= 当前时间 + 提前量，就生成
            if (noteData.time <= currentTime + 0.5f)  // 提前几秒生成就意味着在轨道上移动几秒！！！就是速度！！！！
            {
                SpawnNote(noteData);
                nextNoteIndex++;
            }
            else
            {
                break;  // 还没到时间，停止生成
            }
        }
    }

    void SpawnNote(NoteData_SO noteData)
    {
        // 实例化音符
        GameObject newNote = Instantiate(notePrefab);


        // 获取音符脚本
        NoteBehaviour note = newNote.GetComponent<NoteBehaviour>();

        // 把音符数据传给它
        note.noteData = noteData;

        //Debug.Log($"生成音符: 类型={noteData.type}, 音轨={noteData.lane}, 时间={noteData.time}");
    }

    public void StartPlaying()
    {
        isPlaying = true;
        audioSource.Play();
        nextNoteIndex = 0;
    }

    public void StopPlaying()
    {
        isPlaying = false;
        audioSource.Stop();
    }
}