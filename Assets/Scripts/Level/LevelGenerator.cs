using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform segmentPrefab;     //道路片段预制体
    public Transform player;            //角色Transform组件
    public Transform _segmentPool;      //对象池父对象

    private float segmentLength = 20f;  //道路片段长度
    private float nextSpawnZ = -20f;    //下一个生成点在Z轴的位置

    private List<Transform> segmentList = new List<Transform>();  //道路片段列表

    private ObjectPool<Transform> segmentPool;  //对象池
    private int prewarmCount = 15;              //预热数量

    // Start is called before the first frame update
    void Start()
    {
        segmentPool = new ObjectPool<Transform>(segmentPrefab, prewarmCount, _segmentPool);

        for(int i = 0; i < 12; i++)
        {
            SpawnSegment();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnSegmentAhead();
        RecycleSegmentBehind();
    }

    /// <summary>
    /// 生成道路片段
    /// </summary>
    void SpawnSegment()
    {
        Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
        Transform seg = segmentPool.Get(spawnPos, Quaternion.identity);

        segmentList.Add(seg);

        nextSpawnZ += segmentLength;
    }

    /// <summary>
    /// 在前方生成道路片段
    /// </summary>
    void SpawnSegmentAhead()
    {
        while(nextSpawnZ - player.position.z < 200f)
        {
            SpawnSegment();
        }
    }

    /// <summary>
    /// 回收后方的道路片段
    /// </summary>
    void RecycleSegmentBehind()
    {
        if (segmentList.Count == 0) return;

        Transform head = segmentList[0];
        Vector3 headPos = head.position;

        if(player.position.z - headPos.z > 40f)
        {
            segmentPool.Release(head);
            segmentList.RemoveAt(0);
        }
    }
}
