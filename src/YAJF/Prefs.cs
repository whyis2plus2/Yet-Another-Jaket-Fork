namespace YAJF;

public static class Prefs
{
    static PrefsManager pm => PrefsManager.Instance;

    public const int MAX_TAG_LEN = 24;
    public static string tag
    {
        get 
        {
            string value = pm.GetString("YAJF.tag");
            value = value.Length > MAX_TAG_LEN? value.Substring(0, MAX_TAG_LEN) : value;
            return value;
        }
        set 
        {
            value = value.Length > MAX_TAG_LEN? value.Substring(0, MAX_TAG_LEN) : value;
            pm.SetString("YAJF.tag", value);
        }
    }
    public static string tagColor
    {
        get => pm.GetString("YAJF.tagColor");
        set => pm.SetString("YAJF.tagColor", value);
    }
    public static bool labelSprays
    {
        get => pm.GetBool("YAJF.labelSprays");
        set => pm.SetBool("YAJF.labelSprays", value);
    }
}