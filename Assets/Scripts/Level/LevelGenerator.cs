using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance {get; private set;}

    public Transform segmentPrefab;     //道路片段预制体
    public Transform player;            //角色Transform组件
    public Transform _segmentPool;      //对象池父对象

    private float segmentLength = 20f;  //道路片段长度
    private float nextSpawnZ = -20f;    //下一个生成点在Z轴的位置
  
    private List<Transform> segmentList = new List<Transform>();  //道路片段列表

    //用字典管理多个池
    private Dictionary<Transform, ObjectPool<Transform>> dicPools = new Dictionary<Transform, ObjectPool<Transform>>();
    private Dictionary<Transform, Transform> instanceOwner = new Dictionary<Transform, Transform>();    //存储对象属于哪个prefab

    //障碍物预制体
    public Transform lowObstaclePrefab; //矮障碍
    public Transform crossbarPrefab;    //横杆
    public Transform wallPrefab;        //墙
    public Transform coinPrefab;        //金币

    public Transform _lowObstaclePool; //矮障碍池父对象
    public Transform _crossbarPool;    //横杆池父对象
    public Transform _wallPool;        //墙池父对象
    public Transform _coinPool;       //金币池父对象
    private float[] laneX = new float[] { -4f, 0f, 4f };    //三条车道的x坐标
    private float rowLocalZ = 10f;    //一行障碍物的局部z坐标
    
    //每条道的随机概率
    private const int chanceEmpty = 50; //空道概率
    private const int chanceCoin = 30; //金币概率
    private const int chanceLow = 8;   //矮障碍概率
    private const int chanceCross = 8; //横杆概率
    //剩下的4概率是墙概率

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PrewarmPool(segmentPrefab, 15, _segmentPool);
        PrewarmPool(lowObstaclePrefab, 20, _lowObstaclePool);
        PrewarmPool(crossbarPrefab, 20, _crossbarPool);
        PrewarmPool(wallPrefab, 20, _wallPool);
        PrewarmPool(coinPrefab, 40, _coinPool);

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
    /// 为指定prefab建池并预热
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="prewarmCount"></param>
    /// <param name="parent"></param>
    void PrewarmPool(Transform prefab, int prewarmCount, Transform parent)
    {
        if (prefab == null) return;
        if (dicPools.ContainsKey(prefab)) return;

        dicPools.Add(prefab, new ObjectPool<Transform>(prefab, prewarmCount, parent));
    }

    /// <summary>
    /// 按prefab取池
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    ObjectPool<Transform> GetPool(Transform prefab)
    {
        if (!dicPools.ContainsKey(prefab))
        {
            dicPools.Add(prefab, new ObjectPool<Transform>(prefab, 0));
        }

        return dicPools[prefab];
    }

    /// <summary>
    /// 从池中取出对象
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    Transform GetFromPool(Transform prefab, Vector3 position, Quaternion rotation)
    {
        Transform obj = GetPool(prefab).Get(position, rotation);
        instanceOwner[obj] = prefab;
        return obj;
    }

    /// <summary>
    /// 归还对象
    /// </summary>
    /// <param name="obj"></param>
    void ReleaseToPool(Transform obj)
    {
        if (obj == null) return;

        Transform prefab;
        if(instanceOwner.TryGetValue(obj, out prefab))
        {
            Transform poolParent = GetPoolParent(prefab);
            if(poolParent != null) obj.SetParent(poolParent);

            GetPool(prefab).Release(obj);
            instanceOwner.Remove(obj);
        }
    }

    /// <summary>
    /// 生成道路片段
    /// </summary>
    void SpawnSegment()
    {
        Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
        Transform seg = GetFromPool(segmentPrefab, spawnPos, Quaternion.identity);

        segmentList.Add(seg);

        if(segmentList.Count > 2)
        {
            SpawnRowForSegment(seg);
        }

        nextSpawnZ += segmentLength;
    }

    /// <summary>
    /// 给一个段生成一排随机障碍物
    /// </summary>
    /// <param name="segment"></param>
    void SpawnRowForSegment(Transform segment)
    {
        Transform row = segment.Find("Row");

        if(row == null)
        {
            GameObject rowObj = new GameObject("Row");
            rowObj.transform.SetParent(segment);
            rowObj.transform.localPosition = new Vector3(0f, 0f, rowLocalZ);
            row = rowObj.transform;
        }

        row.gameObject.SetActive(true);

        SpawnRandomRow(row);
    }

    /// <summary>
    /// 生成一排随机障碍
    /// </summary>
    /// <param name="row"></param>
    void SpawnRandomRow(Transform row)
    {
         Transform[] choice = new Transform[3];

         for(int i = 0; i < 3; i++)
         {
            choice[i] = RandomObstacle();
         }

        //三条车道不能同时为墙，否则腾空一条
         if(choice[0] == wallPrefab && choice[1] == wallPrefab && choice[2] == wallPrefab)
         {
            int openLane = Random.Range(0, 3);
            choice[openLane] = null;
         }

         for(int i = 0; i < 3; i++)
         {
            if(choice[i] == null) continue;
            SpawnObstacleAt(row, i, choice[i]);
         }
    }

    /// <summary>
    /// 随机生成一个障碍物
    /// </summary>
    /// <returns></returns>
    Transform RandomObstacle()
    {
        int roll = Random.Range(0, 100);

        if(roll < chanceEmpty) return null;
        else if(roll < chanceEmpty + chanceCoin) return coinPrefab;
        else if(roll < chanceEmpty + chanceCoin +chanceLow) return lowObstaclePrefab;
        else if(roll < chanceEmpty + chanceCoin + chanceLow + chanceCross) return crossbarPrefab;
        else return wallPrefab;
    }

    /// <summary>
    /// 在指定Row的指定道路上放一个障碍物
    /// </summary>
    /// <param name="row"></param>
    /// <param name="laneIndex"></param>
    /// <param name="prefab"></param>
    void SpawnObstacleAt(Transform row, int laneIndex, Transform prefab)
    {
        if (prefab == null) return;

        Transform obj = GetFromPool(prefab, Vector3.zero, Quaternion.identity);

        obj.SetParent(row);
        obj.localPosition = new Vector3(laneX[laneIndex], 0f, 0f);
        obj.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 在前方生成道路片段
    /// </summary>
    void SpawnSegmentAhead()
    {
        int guard = 0;
        while(nextSpawnZ - player.position.z < 200f)
        {
            SpawnSegment();

            guard++;
            if(guard > 20) break;   //防止无限生成
        }
    }

    /// <summary>
    /// 获取指定prefab的池父对象
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    Transform GetPoolParent(Transform prefab)
    {
        if(prefab == lowObstaclePrefab) return _lowObstaclePool;
        if(prefab == crossbarPrefab) return _crossbarPool;
        if(prefab == wallPrefab) return _wallPool;
        if(prefab == coinPrefab) return _coinPool;
        
        return _segmentPool;
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
            //先回收障碍物
            RecycleObstacleInSegment(head);

            ReleaseToPool(head);
            segmentList.RemoveAt(0);
        }
    }

    void RecycleObstacleInSegment(Transform segment)
    {
        for(int i = segment.childCount - 1; i >= 0; i--)
        {
            Transform row = segment.GetChild(i);
            if(row.name != "Row") continue;

            RecycleObstacleInRow(row);
            
            row.gameObject.SetActive(false);
        }
    }

    void RecycleObstacleInRow(Transform row)
    {
        for(int i = row.childCount - 1; i >= 0; i--)
        {
            ReleaseToPool(row.GetChild(i));
        }
    }

    /// <summary>
    /// 回收指定对象
    /// </summary>
    /// <param name="obj"></param>
    public void RecycleObject(Transform obj)
    {
        ReleaseToPool(obj);
    }
}
