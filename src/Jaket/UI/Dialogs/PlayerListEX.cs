namespace Jaket.UI.Dialogs;

using Jaket.Assets;
using Jaket.Content;
using Jaket.Net;
using Jaket.World;

using static Pal;
using static Rect;

/// <summary> List of all players and teams. </summary>
public class PlayerListEX : CanvasSingleton<PlayerListEX>
{    private void Start()
    {
        UIB.Shadow(transform);
        UIB.Table("Teams", "#player-list.team", transform, Tlw(16f + 230f / 2f, 230f, 136f + 64f * (float)System.Math.Round(Tools.EnumMax<Team>()/2d)), table =>
        {
            UIB.Text("#player-list.info", table, Btn(71f) with { Height = 46f }, size: 16);

            float x = 8f;
            foreach (Team team in System.Enum.GetValues(typeof(Team)))
            {
                if ((int)team <= Tools.EnumMax<Team>()/2) UIB.TeamButton(team, table, new(x += 64f, -130f, 56f, 56f, new(0f, 1f)), () =>
                {
                    Networking.LocalPlayer.Team = team;
                    Events.OnTeamChanged.Fire();

                    Rebuild();
                });
                else
                {
                    if ((int)team == Tools.EnumMax<Team>()/2 + 1) x = 8f;
                    UIB.TeamButton(team, table, new(x += 64f, -194f, 56f, 56f, new(0f, 1f)), () =>
                    {
                        Networking.LocalPlayer.Team = team;

                        Events.OnTeamChanged.Fire();

                        Rebuild();
                    });
                }
            }
        });

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
        if (transform.childCount > 3) Destroy(transform.GetChild(3).gameObject);
        if (LobbyController.Offline) return;

        float height = LobbyController.Lobby.Value.MemberCount * 48f + 48f;
        UIB.Table("List", "#player-list.list", transform, Tlw(262f + height / 2f, height), table =>
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
                        UIB.IconButton("X", table, Icon(92f, y),  red,    clicked: () => Administration.Ban(member.Id.AccountId));
                        UIB.IconButton("Y", table, Icon(140f, y), yellow, clicked: () => Administration.Kick(member.Id.AccountId));
                    }
                    else UIB.ProfileButton(member, table, Btn(y += 48f));
                }
            }
        });
    }
}
