using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject segmentPrefab;    //道路片段预制体
    public Transform player;            //角色Transform组件

    private float segmentLength = 20f;  //道路片段长度
    private float nextSpawnZ = -20f;    //下一个生成点在Z轴的位置

    private List<GameObject> segmentList = new List<GameObject>();  //道路片段列表

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 60; i++)
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
        GameObject seg = Instantiate(segmentPrefab, spawnPos, Quaternion.identity);

        segmentList.Add(seg);

        nextSpawnZ += segmentLength;
    }

    /// <summary>
    /// 在前方生成道路片段
    /// </summary>
    void SpawnSegmentAhead()
    {
        while(nextSpawnZ - player.position.z < 60f)
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

        GameObject head = segmentList[0];
        Vector3 headPos = head.transform.position;

        if(player.position.z - headPos.z > 40f)
        {
            Destroy(head);
            segmentList.RemoveAt(0);
        }
    }
}
