using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour
{
    [Header("引用音符数据")]
    public NoteData_SO noteData;  // 要挂载的SO文件

    [Header("运动参数")]
    public float moveDuration;
    public float speed = 5f;    // 移动时间
    public float startScale = 0.3f;  // 开始大小
    public float endScale = 1f;      // 结束大小

    private Transform startPosition;
    private Transform endPosition;
    private float currentTime;        // 当前已移动时间

    // 判定相关
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // 状态管理
    private bool isJudged;      // 是否已经被判定
    private bool isAtJudgeLine;  // 是否已到达判定线
    private float arrivalTime;           // 到达判定线的时间
    public float judgeWindow = 0.5f;

    public static System.Action<GameObject, Transform, NoteData_SO.NoteType> OnNoteHit;
    public static System.Action<bool> OnNoteDestroy;

    void Awake()
    {

        isAtJudgeLine = false;
        isJudged = false;
        // 获取或添加SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    void Start()
    {

        startPosition = GetStartPositionByLane(noteData.lane);
        endPosition = GetEndPositionByLane(noteData.lane);

        if (noteData != null && noteData.image != null)
        {
            spriteRenderer.sprite = noteData.image;
        }
        else
        {
            Debug.LogWarning("音符没有图片！");
            // 创建一个默认的白色方块ss
            //CreateDefaultSprite();
        }

        //初始化currentTime
        currentTime = 0f;

        //  自动计算移动时间（基于速度）
        float distance = Vector3.Distance(startPosition.position, endPosition.position);

        moveDuration = distance / speed;  // 覆盖默认值





        // 设置初始位置和大小
        transform.position = startPosition.position;
        transform.localScale = Vector3.one * startScale;
        transform.rotation = startPosition.rotation; // 旋转
        spriteRenderer.sortingOrder = 10;
    }

    void Update()
    {
        if (isJudged) return;

        // 增加时间
        currentTime += Time.deltaTime;

        // 计算进度 (0到1)
        float progress = currentTime / moveDuration;


        if (progress >= 1f)
        {
            // 到达判定线！不销毁，而是进入等待判定状态
            if (!isAtJudgeLine)
            {
                isAtJudgeLine = true;
                OnReachJudgeLine();
            }
            return;
        }

        // 移动位置：从起点移动到终点
        transform.position = Vector3.Lerp(startPosition.position, endPosition.position, progress);

        // 从小变大
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        transform.localScale = Vector3.one * currentScale;
    }

    //获取起始位置
    Transform GetStartPositionByLane(int lane)
    {
        string objectName = $"Lane{lane}_Start";

        // 在场景中查找该物体
        GameObject startObject = GameObject.Find(objectName);

        return startObject.transform;

    }


    // 获取判定线位置
    Transform GetEndPositionByLane(int lane)
    {
        string objectName = $"Lane{lane}_End";

        // 在场景中查找该物体
        GameObject startObject = GameObject.Find(objectName);

        return startObject.transform;
    }

    void OnReachJudgeLine()
    {
        isAtJudgeLine = true;
        arrivalTime = Time.time;

        //停在判定线位置，不再移动
        transform.position = endPosition.position;
        transform.localScale = Vector3.one * endScale;
        transform.rotation = startPosition.rotation;

        // 根据音轨开始检测对应的键盘按键
        StartCoroutine(WaitForPlayerInput());

        // 设置超时销毁
        StartCoroutine(AutoDestroyAfterDelay(judgeWindow));
    }


    /// <summary>
    /// 等待玩家按下对应轨道的按键
    /// </summary>
    IEnumerator WaitForPlayerInput()
    {
        // 获取当前音符对应轨道的按键
        KeyCode targetKey = GetKeyCodeByLane(noteData.lane);

        // 等待玩家按下按键
        while (!isJudged && isAtJudgeLine)
        {
            // 检测对应的按键是否被按下
            if (Input.GetKeyDown(targetKey))
            {
                // 玩家按下了正确的按键
                TryJudge(noteData.lane);
                yield break;
            }

            // 每帧检查一次
            yield return null;
        }
    }

    /// <summary>
    /// 根据音轨获取对应的键盘按键
    /// </summary>
    KeyCode GetKeyCodeByLane(int lane)
    {
        switch (lane)
        {
            case 0: return KeyCode.S;
            case 1: return KeyCode.D;
            case 2: return KeyCode.F;
            case 3: return KeyCode.J;
            case 4: return KeyCode.K;
            case 5: return KeyCode.L;
            default: return KeyCode.None;
        }
    }

    /// <summary>
    /// 玩家按下对应轨道时调用
    /// </summary>
    /// <param name="pressedLane">按下的音轨编号</param>
    /// <returns>是否成功判定</returns>
    public bool TryJudge(int pressedLane)
    {
        // 如果已经被判定过了，或者还没到达判定线，忽略
        if (isJudged || !isAtJudgeLine) return false;

        // 检查按下的轨道是否正确
        if (pressedLane != noteData.lane) return false;

        // 计算按下时间与到达时间的差值
        float timeDifference = Time.time - arrivalTime;

        // 检查是否在判定窗口内（可以自定义窗口大小）
        if (Mathf.Abs(timeDifference) > judgeWindow)
        {
            Debug.Log($"按下太了！差值: {timeDifference:F2}秒");
            return false;
        }

        // 判定成功！
        isJudged = true;
        OnNoteDestroy?.Invoke(true);
        Destroy(gameObject);


        // 根据音符类型播放不同的特效
        PlayHitEffectByType();

        // 根据时间差计算判定等级
        JudgeGrade grade = GetJudgeGrade(timeDifference);
        Debug.Log($"音轨{noteData.lane} 击中！判定: {grade}, 误差: {timeDifference:F3}秒");

        // 可以在这里调用分数管理器，增加分数
        // ScoreManager.Instance.AddScore(grade, noteData.type);

        // 销毁音符



        return true;
    }



    // 被玩家击中
    /// <summary>
    /// 根据音符类型播放不同的特效
    /// </summary>
    void PlayHitEffectByType()
    {
        switch (noteData.type)
        {
            case NoteData_SO.NoteType.Normal:

                OnNoteHit?.Invoke(this.gameObject, transform, noteData.type);
                break;

            case NoteData_SO.NoteType.Hold:

                OnNoteHit?.Invoke(this.gameObject, transform, noteData.type);
                break;

            case NoteData_SO.NoteType.SwipeUp:
                Debug.Log("向上划特效！⬆️");
                //PlaySwipeUpEffect();
                break;

            case NoteData_SO.NoteType.SwipeDown:
                Debug.Log("向下划特效！⬇️");
                //PlaySwipeDownEffect();
                break;
        }
    }

    // 没打中


    /// <summary>
    /// 超时自动销毁（玩家没按）
    /// </summary>
    IEnumerator AutoDestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isJudged && isAtJudgeLine)
        {
            isJudged = true;
            //Debug.Log($"音符超时销毁！音轨{noteData.lane} 玩家没有按下");
            // 不播放任何特效，直接销毁

            OnNoteDestroy?.Invoke(true);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 根据时间差计算判定等级
    /// </summary>
    JudgeGrade GetJudgeGrade(float timeDifference)
    {
        float absDiff = Mathf.Abs(timeDifference);

        if (absDiff < 0.05f) return JudgeGrade.Perfect;
        if (absDiff < 0.1f) return JudgeGrade.Great;
        if (absDiff < 0.15f) return JudgeGrade.Good;
        return JudgeGrade.Bad;
    }

    public enum JudgeGrade
    {
        Perfect,
        Great,
        Good,
        Bad
    }
}
