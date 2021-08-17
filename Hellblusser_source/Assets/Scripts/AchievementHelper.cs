using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHelper
{
    public static void UnlockAchievement ( string achName )
    {
        #if !DISABLESTEAMWORKS
            if ( SteamManager.Initialized )
            {
                Steamworks.SteamUserStats.SetAchievement(achName);
                Steamworks.SteamUserStats.StoreStats();
            }
        #endif
    }
}