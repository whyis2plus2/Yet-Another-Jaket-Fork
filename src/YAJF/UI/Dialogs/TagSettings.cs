namespace YAJF.UI.Dialogs;

using System;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

using Jaket.UI;
using Jaket.World;

using static Jaket.UI.Rect;
using static Jaket.UI.Pal;

public class TagSettings: CanvasSingleton<TagSettings>
{
    InputField field, Hex;
    Slider Red, Green, Blue;
    byte r = 255, g = 255, b = 255;

    private void Start()
    {
        if (Prefs.tagColor.Length >= 7)
        {
            r = Convert.ToByte("0x" + Prefs.tagColor.Substring(1, 2), 16);
            g = Convert.ToByte("0x" + Prefs.tagColor.Substring(3, 2), 16);
            b = Convert.ToByte("0x" + Prefs.tagColor.Substring(5, 2), 16);
        }

        UIB.Table("Content", "---TAG SETTINGS---", transform, Size(640, 395), table =>
        {
            UIB.IconButton("X", table, Icon(292f, 24f), red, clicked: Toggle);

            field = UIB.Field("TAG", table, new(320f, -68f, 350f, 40f, new(0f, 1f)), cons: text =>
            {
                Prefs.tagColor = $"#{r:X2}{g:X2}{b:X2}";
                Prefs.tag = Regex.Replace(text.Trim(), "<*.?>", "").Replace("[", "\\[");
            });
            field.characterLimit = Prefs.MAX_TAG_LEN;

            UIB.Text("Red", table, Btn(108), red, align: TextAnchor.MiddleLeft);
            var RText = UIB.Text("0", table, Btn(108), red, align: TextAnchor.MiddleRight);
            Red = UIB.Slider("Red", table, Sld(138) with {Width = 350}, 255, value =>
            {
                r = (byte)value;
                RText.text = $"{value/255f:n2}";
                Rebuild();
            });

            UIB.Text("Green", table, Btn(158), green, align: TextAnchor.MiddleLeft);
            var GText = UIB.Text("0", table, Btn(158), green, align: TextAnchor.MiddleRight);
            Green = UIB.Slider("Green", table, Sld(188) with {Width = 350}, 255, value =>
            {
                g = (byte)value;
                GText.text = $"{value/255f:n2}";
                Rebuild();
            });

            UIB.Text("Blue", table, Btn(208), blue, align: TextAnchor.MiddleLeft);
            var BText = UIB.Text("0", table, Btn(208), blue, align: TextAnchor.MiddleRight);
            Blue = UIB.Slider("Blue", table, Sld(238) with {Width = 350}, 255, value =>
            {
                b = (byte)value;
                BText.text = $"{value/255f:n2}";
                Rebuild();
            });

            Hex = UIB.Field("Hex", table, new(320f, -283f, 350f, 40f, new(0f, 1f)), cons: text =>
            {
                while (text.Length < 6 && text.Length % 2 == 0) text += "00";
                if (text.Length == 3)
                {
                    r = (byte)(Convert.ToByte("0x" + text.Substring(0, 1), 16) * 0x11);
                    g = (byte)(Convert.ToByte("0x" + text.Substring(1, 1), 16) * 0x11);
                    b = (byte)(Convert.ToByte("0x" + text.Substring(2, 1), 16) * 0x11);

                    Prefs.tagColor = $"#{r:X2}{g:X2}{b:X2}";
                    Rebuild();
                    return;
                }
                if (text.Length != 6)
                {
                    r = g = b = 0xFF;
                    Prefs.tagColor = $"#FFFFFF";
                    Rebuild();
                    return;
                }

                r = Convert.ToByte("0x" + text.Substring(0, 2), 16);
                g = Convert.ToByte("0x" + text.Substring(2, 2), 16);
                b = Convert.ToByte("0x" + text.Substring(4, 2), 16);

                Prefs.tagColor = $"#{r:X2}{g:X2}{b:X2}";
                Rebuild();
            });
            Hex.characterLimit = 6;

            UIB.IconButton("CONFIRM", table, Btn(335), green, clicked: () =>
            {
                Prefs.tagColor = $"#{r:X2}{g:X2}{b:X2}";
                Prefs.tag = Regex.Replace(field.text.Trim(), "<*.?>", "").Replace("[", "\\[");

                if (string.IsNullOrEmpty(Prefs.tag)) HudMessageReceiver.Instance?.SendHudMessage($"Succesfully Removed Tag");
                else HudMessageReceiver.Instance?.SendHudMessage($"Succesfully Set Tag to \"<color={Prefs.tagColor}>{Prefs.tag}</color>\"");
                
                Toggle();
            });
        });

        Rebuild();
    }

    private void Rebuild()
    {
        field.textComponent.color = new Color32(r, g, b, 0xFF);
        field.text = Prefs.tag;
        Hex.text = $"{r:X2}{g:X2}{b:X2}";

        if (r != 0) Red.value = r;
        if (g != 0) Green.value = g;
        if (b != 0) Blue.value = b;
    }

    public void Toggle()
    {
        if (!Shown) UI.HideCentralGroup();

        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();

        if (Shown && transform.childCount > 0) Rebuild();
    }
}