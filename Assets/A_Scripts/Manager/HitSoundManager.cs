using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundManager : MonoBehaviour
{
    [Header("音效列表")]

    public List<AudioClip> hitSounds = new List<AudioClip>();

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        // 订阅音符击中事件
        NoteBehaviour.OnNoteHit += PlayHitSound;
    }

    void OnDisable()
    {
        // 取消订阅
        NoteBehaviour.OnNoteHit -= PlayHitSound;
    }

    void PlayHitSound(GameObject ob, Transform hitPosition, NoteData_SO.NoteType type)
    {
        // 把枚举转成 int，作为列表索引
        int index = (int)type;

        // 检查索引是否在列表范围内
        if (index >= 0 && index < hitSounds.Count)
        {
            AudioClip clip = hitSounds[index];

            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
                // Debug.Log($"播放音效: type={type}, index={index}, clip={clip.name}");
            }
            else
            {
                Debug.LogWarning($"音效列表第 {index} 项为空，无法播放");
            }
        }
        else
        {
            //Debug.LogWarning($"音效列表长度={hitSounds.Count}，但请求的索引={index} 超出范围");
        }
    }
}