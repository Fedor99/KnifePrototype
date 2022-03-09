using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private List<bool> savedColliderEnabledStatus;
    Collider[] collidersObj;

    // Start is called before the first frame update
    void Start()
    {
        collidersObj = GetComponentsInChildren<Collider>();
    }

    public void MakeAll_Trigger(bool isTrigger)
    {
        savedColliderEnabledStatus = new List<bool>();
        for (var index = 0; index < collidersObj.Length; index++)
        {
            var colliderItem = collidersObj[index];
            savedColliderEnabledStatus.Add(colliderItem.isTrigger);
            colliderItem.isTrigger = isTrigger;
        }
    }

    public void ReturnToSavedTriggerStatus()
    {
        if (savedColliderEnabledStatus == null)
            return;
        if (savedColliderEnabledStatus.Count == 0)
            return;

        for (var index = 0; index < collidersObj.Length; index++)
        {
            var colliderItem = collidersObj[index];
            colliderItem.isTrigger = savedColliderEnabledStatus[index];
        }
    }
}
