namespace Jaket.Content;

using UnityEngine;

using Jaket.Net;

/// <summary> All teams. Teams needed for PvP mechanics. </summary>
public enum Team
{
    Yellow, Red, Green, Blue, Pink,

    // YAJF custom teams
    V1, V2, Purple, Cyan, White,
 
    /// <summary> this marks the start of values reserved for bitmasks that add extra properties to teams </summary>
    YAJF_MaskReserved = 0b100000000, 

    /// <summary> this mask adds cat ears to players of any team :3 </summary>
    YAJF_CatEars_Mask = 0b100000000,
}

/// <summary> Extension class that allows you to get team data. </summary>
public static class TeamExtensions
{
    /// <summary> Returns the team color, used only in the interface. </summary>
    public static Color Color(this Team team) => team.YAJF_RemoveMasks() switch
    {
        Team.Yellow => new(1f, .8f, .3f),
        Team.Red    => new(1f, .2f, .1f),
        Team.Green  => new(0f, .9f, .4f),
        Team.Blue   => new(0f, .5f,  1f),
        Team.Pink   => new(1f, .4f, .8f),

        // YAJF custom teams
        Team.V1     => new(.1f, .3f,  1f),
        Team.V2     => new( 1f,  0f, .1f),
        Team.Purple => new(.7f,  0f,  1f),
        Team.Cyan   => new( 0f,  1f,  1f),
        
        _ => new(1f, 1f, 1f) // this covers white team
    };

    /// <summary> Whether this team is allied with the player. </summary>
    public static bool Ally(this Team team) => team.YAJF_RemoveMasks() == Networking.LocalPlayer.Team || !LobbyController.PvPAllowed;

    /// <summary> The player's team without any of the bitmasks </summary>
    public static Team YAJF_RemoveMasks(this Team team) => team & ~Team.YAJF_MaskReserved;
}
