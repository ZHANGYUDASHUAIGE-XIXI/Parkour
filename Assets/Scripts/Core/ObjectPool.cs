using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    private Queue<T> pool = new Queue<T>(); //池队列

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="prewarmCount">预热数量</param>
    /// <param name="parent">没传参 默认为空</param>
    public ObjectPool(T prefab, int prewarmCount, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for(int i = 0; i < prewarmCount; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 从池中取出对象
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj;

        if(pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Object.Instantiate(prefab, parent);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);

        return obj;
    }

    /// <summary>
    /// 将对象归还给池中
    /// </summary>
    /// <param name="obj"></param>
    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
