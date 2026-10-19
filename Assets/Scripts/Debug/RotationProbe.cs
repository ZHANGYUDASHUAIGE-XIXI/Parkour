using System.Text;
using UnityEngine;

/// <summary>
/// 旋转探针：每隔一段时间打印层级里各物体的 Y 轴旋转
/// 用来找出"是哪个物体在转"
/// </summary>
public class RotationProbe : MonoBehaviour
{
    [Tooltip("多久打印一次（秒）")]
    public float interval = 1.5f;

    [Tooltip("往下展开几层")]
    public int maxDepth = 5;

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = interval;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("===== 层级 Y 轴旋转 =====");
        Dump(transform, 0, sb);
        Debug.Log(sb.ToString());
    }

    void Dump(Transform t, int depth, StringBuilder sb)
    {
        if (depth > maxDepth) return;

        // 把 0~360 归一化到 -180~180
        float y = Mathf.DeltaAngle(0f, t.localEulerAngles.y);

        sb.AppendLine(new string(' ', depth * 2) + t.name + "  Y = " + y.ToString("F1") + "°");

        for (int i = 0; i < t.childCount; i++)
        {
            Dump(t.GetChild(i), depth + 1, sb);
        }
    }
}
