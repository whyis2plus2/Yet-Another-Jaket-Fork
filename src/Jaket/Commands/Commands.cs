namespace Jaket.Commands;

using System;
using UnityEngine;

using Jaket.Assets;
using Jaket.Content;
using Jaket.Net;
using Jaket.UI.Dialogs;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary> List of chat commands used by the mod. </summary>
public class Commands
{
    static Chat chat => Chat.Instance;

    /// <summary> Chat command handler. </summary>
    public static CommandHandler Handler = new('/');
    public static CommandHandler YAJF_Handler = new('!');

    /// <summary> Checks if the message is a valid command in either handler </summary>
    public static bool YAJF_CallCommand(string msg) => Handler.Handle(msg) || YAJF_Handler.Handle(msg);

    /// <summary> Registers all default mod commands. </summary>
    public static void Load()
    {
#region Jaket Commands
        Handler.Register("help", "Display the list of all commands", args =>
        {
            Handler.Commands.ForEach(command =>
            {
                chat.Receive($"[14]* /{command.Name}{(command.Args == null ? "" : $" [#BBBBBB]{command.Args}[]")} - {command.Desc}[]");
            });
        });
        Handler.Register("hello", "Resend the tips for new players", args => chat.Hello(true));

        Handler.Register("tts-volume", "\\[0-100]", "Set Sam's volume to keep your ears comfortable", args =>
        {
            if (args.Length == 0)
                chat.Receive($"[#FFA500]TTS volume is {Settings.TTSVolume}.");
            else if (int.TryParse(args[0], out int value))
            {
                int clamped = Mathf.Clamp(value, 0, 100);
                Settings.TTSVolume = clamped;

                chat.Receive($"[#32CD32]TTS volume is set to {clamped}.");
            }
            else
                chat.Receive("[#FF341C]Failed to parse value. It must be an integer in the range from 0 to 100.");
        });
        Handler.Register("tts-auto", "\\[on/off]", "Turn auto reading of all messages", args =>
        {
            bool enable = args.Length == 0 ? !chat.AutoTTS : (args[0] == "on" || (args[0] == "off" ? false : !chat.AutoTTS));
            if (enable)
            {
                Settings.AutoTTS = chat.AutoTTS = true;
                chat.Receive("[#32CD32]Auto TTS enabled.");
            }
            else
            {
                Settings.AutoTTS = chat.AutoTTS = false;
                chat.Receive("[#FF341C]Auto TTS disabled.");
            }
        });

        Handler.Register("plushies", "Display the list of all dev plushies", args =>
        {
            string[] plushies = (string[])GameAssets.PlushiesButReadable.Clone();
            Array.Sort(plushies); // sort alphabetically for a more presentable look

            chat.Receive(string.Join(", ", plushies));
        });
        Handler.Register("plushy", "<name>", "Spawn a plushy by name", args =>
        {
            string name = args.Length == 0 ? null : args[0].ToLower();
            int index = Array.FindIndex(GameAssets.PlushiesButReadable, plushy => plushy.ToLower() == name);

            if (index == -1)
                chat.Receive($"[#FF341C]Plushy named {name} not found.");
            else
                Tools.Instantiate(Items.Prefabs[EntityType.PlushyOffset + index - EntityType.ItemOffset].gameObject, NewMovement.Instance.transform.position);
        });

        Handler.Register("level", "<layer> <level> / sandbox / cyber grind / credits museum", "Load the given level", args =>
        {
            if (args.Length == 1 && args[0].Contains("-")) args = args[0].Split('-');

            if (!LobbyController.IsOwner)
                chat.Receive($"[#FF341C]Only the lobby owner can load levels.");

            else if (args.Length >= 1 && (args[0].ToLower() == "sandbox" || args[0].ToLower() == "sand"))
            {
                Tools.Load("uk_construct");
                chat.Receive("[#32CD32]Sandbox is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("cyber") || args[0].ToLower().Contains("grind") || args[0].ToLower() == "cg"))
            {
                Tools.Load("Endless");
                chat.Receive("[#32CD32]The Cyber Grind is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("credits") || args[0].ToLower() == "museum"))
            {
                Tools.Load("CreditsMuseum2");
                chat.Receive("[#32CD32]The Credits Museum is loading.");
            }
            else if (args.Length < 2)
                chat.Receive($"[#FF341C]Insufficient number of arguments.");
            else if
            (
                int.TryParse(args[0], out int layer) && layer >= 0 && layer <= 7 &&
                int.TryParse(args[1], out int level) && level >= 1 && level <= 5 &&
                (level == 5 ? layer == 0 : true) && (layer == 3 || layer == 6 ? level <= 2 : true)
            )
            {
                Tools.Load($"Level {layer}-{level}");
                chat.Receive($"[#32CD32]Level {layer}-{level} is loading.");
            }
            else if (args[1].ToUpper() == "S" && int.TryParse(args[0], out level) && level >= 0 && level <= 7 && level != 3 && level != 6)
            {
                Tools.Load($"Level {level}-S");
                chat.Receive($"[#32CD32]Secret level {level}-S is loading.");
            }
            else if (args[0].ToUpper() == "P" && int.TryParse(args[1], out level) && level >= 1 && level <= 2)
            {
                Tools.Load($"Level P-{level}");
                chat.Receive($"[#32CD32]Prime level P-{level} is loading.");
            }
            else
                chat.Receive("[#FF341C]Layer must be an integer from 0 to 7. Level must be an integer from 1 to 5.");
        });

        Handler.Register("authors", "Display the list of the mod developers", args =>
        {
            void Msg(string msg) => chat.Receive($"[14]{msg}[]");

            Msg("Leading developers:");
            Msg("* [#0096FF]xzxADIxzx[] - the main developer of this mod");
            Msg("* [#8A2BE2]Sowler[] - owner of the Discord server and just a good friend");
            Msg("* [#FFA000]Fumboy[] - textures and a part of animations");

            Msg("Contributors:");
            Msg("* [#00E666]Rey Hunter[] - really cool icons for emotions");
            Msg("* [#00E666]Ardub[] - invaluable help with The Cyber Grind [12][#cccccc](he did 90% of the work)");
            Msg("* [#00E666]Kekson1a[] - Steam Rich Presence support");

            Msg("Translators:");
            Msg("[#cccccc]NotPhobos - Spanish, sSAR - Italian, Theoyeah - French, Sowler - Polish,");
            Msg("[#cccccc]Ukrainian, Poyozit - Portuguese, Fraku - Filipino, Iyad - Arabic");

            Msg("Testers:");
            Msg("[#cccccc]Fenicemaster, AndruGhost, Subjune, FruitCircuit");

            chat.Receive("0096FF", Chat.BOT_PREFIX + "xzxADIxzx", "Thank you all, I couldn't have done it alone ♡");
        });
        Handler.Register("support", "Support the author by buying him a coffee", args => Application.OpenURL("https://www.buymeacoffee.com/adidev"));
        Handler.Register("yajf", "show a list of all custom commands", args => chat.Send("!help"));
#endregion Jaket Commands
#region YAJF Commands
        YAJF_Handler.Register("help", "show a list of all custom commands", args =>
        {
            YAJF_Handler.Commands.ForEach(command =>
            {
                chat.Receive($"[14]* !{command.Name}{(command.Args == null ? "" : $" [#BBBBBB]{command.Args}[]")} - {command.Desc}[]");
            });
        });

        YAJF_Handler.Register("clear", "clear chat locally", args =>
        {
            for (int i = 0; i < Chat.MESSAGES_SHOWN; ++i)
            {
                chat.Receive("");
            }
        });

        YAJF_Handler.Register("whisper", "<player> <message>", "send a message to ONLY the specified player", args =>
        {
            if (!LobbyController.YAJF_Modded) 
            {
                chat.Receive("[#FF341C]This command can only be ran in a modded-only lobby");
                return;
            }

            var player = args[0];
            var message = Regex.Replace(string.Join(" ", args.Skip(1)), "<*.?>", "").Replace("[", "\\[");
            uint id = 0;

            if (args.Length < 2)
            {
                chat.Receive("[#FF341C]This command requires at least 2 args: <player> <message>");
                return;
            }

            if (Tools.Name(Tools.AccId).StartsWith(player)) id = Tools.AccId;
            else Networking.EachPlayer(con =>
            {
                if (con.name.StartsWith(player)) id = con.Id;
            });

            if (id == 0)
            {
                chat.Receive($"[#FF341C]\"{player}\" is not a valid user");
                return;
            }

            LobbyController.Lobby?.SendChatString($"#/w{id} {message}");
        });

        YAJF_Handler.Register("difficulty", "\\[value]", "get/set the difficulty, setting the difficulty restarts the level", args =>
        {
            if (args.Length == 0)
            {
                chat.Receive($"Current difficulty is {YAJF.Prefs.difficulty}");
                return;
            }

            bool valid = int.TryParse(args[0], out int difficulty) && difficulty >= 0 && difficulty <= 4;
            if (!valid)
            {
                chat.Receive($"[#FF341C]value must be a number from 0 to 4, \"{args[0]}\" is not valid");
                return;
            }

            YAJF.Prefs.difficulty = difficulty;
            chat.Receive($"Set difficulty to {YAJF.Prefs.difficulty}");
            Tools.Load(Tools.Scene); // restart the current level
        });

        YAJF_Handler.Register("fishies", "Get the list of all fishies/baits", args =>
        {
            var fishies = GameAssets.YAJF_FishiesButReadable.ToList();
            fishies.Sort(); // sort alphabetically for a more presentable look

            // add baits to the printed list
            fishies.Add("Apple Bait");
            fishies.Add("Maurice Bait");

            chat.Receive(string.Join(", ", fishies));
        });
        YAJF_Handler.Register("fishy", "<name>", "spawn a fish/bait by name", args =>
        {
            bool isBait = false;
            string[] baits = new[] { "Apple Bait", "Maurice Bait" };

            string name = args.Length == 0 ? null : string.Join(" ", args).ToLower();
            int index = Array.FindIndex(GameAssets.YAJF_FishiesButReadable, fishy => fishy.ToLower().StartsWith(name));

            if (index == -1)
            {
                isBait = true;
                index = Array.FindIndex(baits, bait => bait.ToLower().StartsWith(name));
            }

            // if the index is still not found, then we know it's invalid
            if (index == -1) chat.Receive($"[#FF341C]Fishy/Bait named {name} not found.");
            else if (isBait) Tools.Instantiate(Items.Prefabs[index].gameObject, NewMovement.Instance.transform.position);
            else
            {
                // fishy prefabs have no collision by themselves, so we have to use a separate prefab to give them collision and to
                // allow them to be picked up
                // var pickup = Tools.Instantiate(GameAssets.YAJF_FishTemplate(), NewMovement.Instance.transform.position);

                // use a v1 plush to allow for pickups because it makes a squeaky sound :) (and it makes fish face the player)
                var pickup = Tools.Instantiate(Items.Prefabs[EntityType.V1 - EntityType.ItemOffset].gameObject, NewMovement.Instance.transform.position);
                Tools.Instantiate(Items.Prefabs[Items.YAJF_FishyOffset + index].gameObject, pickup.transform).transform.position = pickup.transform.position;
                pickup.GetComponentInChildren<Renderer>().enabled = false; // make the v1 plush invisible because it's only purpose is as a hitbox and a sound source
            }
        });
#endregion YAJF Commands
    }
}
