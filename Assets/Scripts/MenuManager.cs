using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Store menus as an array
    public GameObject[] menus;

    void Start()
    {
        ToggleMenu("StartScreen");
    }

    // Method to toggle a menu on or off by its name
    public void ToggleMenu(string menuName)
    {
        // Hide all menus before opening the new one
        HideAllMenus();

        // Search for the menu in the array by its name
        GameObject menu = FindMenuByName(menuName);
        
        if (menu != null)
        {
            menu.SetActive(!menu.activeSelf); // Toggle the menu's active state
        }
        else
        {
            Debug.LogWarning("Menu not found: " + menuName);
        }
    }

    public void ToggleMenu2(string menuName)
    {
        // Search for the menu in the array by its name
        GameObject menu = FindMenuByName(menuName);
        
        if (menu != null)
        {
            menu.SetActive(!menu.activeSelf); // Toggle the menu's active state
        }
        else
        {
            Debug.LogWarning("Menu not found: " + menuName);
        }
    }

    // Method to hide all menus
    public void HideAllMenus()
    {
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
    }


    // Helper method to find a menu by its name
    private GameObject FindMenuByName(string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.name == menuName)
            {
                return menu;
            }
        }
        return null; // Return null if the menu wasn't found
    }
}
