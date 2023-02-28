using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DetectMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public static bool enterUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enterUI = false;
    }
}
