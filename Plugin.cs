using BepInEx;
using ChillzMenu;
using Granalax.Mods;
using UnityEngine;

namespace Granalax
{
    [BepInPlugin(
        "com.chillz.basicmenu",
        "Chillz Basic Menu",
        "1.0.0"
    )]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (gameObject.GetComponent<WristMenu>() == null)
            {
                gameObject.AddComponent<WristMenu>();
                gameObject.AddComponent<PCMenu>();
            }

            Debug.Log(
                "[Chillz] Wrist Menu loaded."
            );
        }
    }
}