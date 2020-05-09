using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadMenu;

    public GameObject firstMenuButton;
    public GameObject firstLoadButton;
    public GameObject firstOptionButton;

    // Update is called once per frame
    void Update()
    {

        CheckMouse();
        
        if (loadMenu.activeSelf)
        {
            if (Input.GetButtonDown("Cancel"))
                ToMainMenu();
        }
    }
    


    private void CheckMouse()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            
            if (raycastResults[i].gameObject.GetComponent<Button>() != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(raycastResults[i].gameObject);
            }
        }
        
    }

    public void ToLoadMenu()
    {
        mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        loadMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstLoadButton);
    }

    public void ToMainMenu()
    {
        loadMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstMenuButton);
    }
   

}
