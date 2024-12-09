namespace Jaket.UI.Dialogs;

using UnityEngine;
using Jaket.Assets;
using Jaket.Content;
using Jaket.Net;
using Jaket.World;

using static Pal;
using static Rect;

/// <summary> List of all players and teams. </summary>
public class PlayerList : CanvasSingleton<PlayerList>
{
    Team max => (LobbyController.Offline || LobbyController.YAJF_Modded)? Team.White : Team.Pink;
    
    private void Start()
    {
        UIB.Shadow(transform);
        Version.Label(transform);
        Rebuild();
    }

    // <summary> Toggles visibility of the player list. </summary>
    public void Toggle()
    {
        if (!Shown) UI.HideLeftGroup();

        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();

        if (Shown && transform.childCount > 0) Rebuild();
    }

    /// <summary> Rebuilds the player list to add new players or remove players left the lobby. </summary>
    public void Rebuild()
    {
        // destroy old player list
        for (int i = 2; i < transform.childCount; ++i) Destroy(transform.GetChild(i).gameObject);

        float teamListHeight = 38f + 64f * Mathf.CeilToInt((float)max / 5f + 1f);
        UIB.Table("Teams", "#player-list.team", transform, Tlw(16f + teamListHeight / 2f, teamListHeight), table =>
        {
            UIB.Text("#player-list.info", table, Btn(71f) with { Height = 46f }, size: 16);

            float x = -24f;
            float y = -130f;
            foreach (Team team in System.Enum.GetValues(typeof(Team))) if (team <= max)
            {
                if (team > 0 && (int)team % 5 == 0) { x = -24f; y -= 64f; }

                UIB.TeamButton(team, table, new(x += 64f, y, 56f, 56f, new(0f, 1f)), () =>
                {
                    Networking.LocalPlayer.Team = team;
                    Events.OnTeamChanged.Fire();

                    Rebuild();
                });
            }
        });

        if (LobbyController.Online)
        {
            float height = LobbyController.Lobby.Value.MemberCount * 48f + 48f;
            UIB.Table("List", "#player-list.list", transform, Tlw(teamListHeight + 32 + height / 2f, height), table =>
            {
                float y = 20f;
                foreach (var member in LobbyController.Lobby?.Members)
                {
                    if (LobbyController.LastOwner == member.Id)
                    {
                        UIB.ProfileButton(member, table, Stn(y += 48f, -48f));
                        UIB.IconButton("★", table, Icon(140f, y), new(1f, .7f, .1f), new(1f, 4f), () => Bundle.Hud("player-list.owner"));
                    }
                    else
                    {
                        if (LobbyController.IsOwner)
                        {
                            UIB.ProfileButton(member, table, Stn(y += 48f, -96f));
                            UIB.IconButton("K", table, Icon(92f, y), YAJF_yellow, clicked: () => Administration.YAJF_Kick(member.Id.AccountId));
                            UIB.IconButton("B", table, Icon(140f, y), red, clicked: () => Administration.Ban(member.Id.AccountId));
                        }
                        else UIB.ProfileButton(member, table, Btn(y += 48f));
                    }
                }
            });
        }
    }
}
