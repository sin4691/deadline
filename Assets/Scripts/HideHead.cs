using UnityEngine;

public class HideHead : MonoBehaviour
{
    void Start()
    {
        Transform head = GameObject.Find("Neck_M").transform;
        head.localScale = Vector3.zero;
    }
}