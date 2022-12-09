using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static AfterImageEffect instance { get; private set; }

    private void Awake()
    {
        instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i < 20; i++)
        {
            GameObject addObjectInstance = Instantiate(afterImagePrefab);
            addObjectInstance.transform.SetParent(this.transform);
            AddToPool(addObjectInstance);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public void GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }
        GameObject instance = availableObjects.Dequeue();
        instance.SetActive(true);
        //return instance;
    }
}
