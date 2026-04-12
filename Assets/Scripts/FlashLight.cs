using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private GameObject lightObject;
    private bool isOn = false;
    void Start()
    {
        lightObject.SetActive(isOn);
    }
    private void OnFlashlight()
    {
        if (lightObject == null) return;

        isOn = !isOn;
        lightObject.SetActive(isOn);
    }
}
