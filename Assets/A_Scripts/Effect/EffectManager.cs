using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("特效预制体")]
    public GameObject hitEffectPrefab;  // 特效预制体（需要在Inspector中拖入）

    private void OnEnable()
    {
        NoteBehaviour.OnNoteHit += SpawnHitEffect;
    }

    public void OnDisable()
    {
        NoteBehaviour.OnNoteHit -= SpawnHitEffect;
    }

    void SpawnHitEffect(GameObject note, Transform hitTransform, NoteData_SO.NoteType type)
    {
        // 检查特效预制体是否已赋值
        if (hitEffectPrefab == null)
        {
            Debug.LogError("特效预制体为空！请在 Inspector 中拖入特效预制体");
            return;
        }

        // 实例化特效预制体
        GameObject newEffect = Instantiate(hitEffectPrefab);

        // 设置特效位置和旋转
        newEffect.transform.position = hitTransform.position;
        newEffect.transform.rotation = hitTransform.rotation;

        // 获取 Animator 并播放动画
        Animator animator = newEffect.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isHit", true);

        }

        // 启动协程，2秒后销毁特效
        StartCoroutine(DestroyEffectAfterDelay(newEffect, 2f));


    }

    /// <summary>
    /// 协程：延迟销毁特效
    /// </summary>
    IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        // 等待 delay 秒
        yield return new WaitForSeconds(delay);

        // 如果特效还存在，销毁它
        if (effect != null)
        {
            Destroy(effect);

        }
    }
}