using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 180f;  //旋转速度

    public static event System.Action<int> OnCoinCollected;     //收集到的金币事件

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnCoinCollected?.Invoke(1); //收集到1个金币
            LevelGenerator.Instance.RecycleObject(transform);   
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}
