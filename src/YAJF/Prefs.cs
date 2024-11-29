namespace YAJF;

public static class Prefs
{
    public static PrefsManager pm => PrefsManager.Instance;

    public static string tag
    {
        get => pm.GetString("YAJF.tag");
        set => pm.SetString("YAJF.tag", value);
    }
    public static bool labelSprays
    {
        get => pm.GetBool("YAJF.labelSprays");
        set => pm.SetBool("YAJF.labelSprays", value);
    }

    /// <summary> Copy a string from one pref to another </summary>
    public static void CopyString(string dest, string src)
    {
        // copy the value of the old key to the new key,
        // if the old key is not present then fall back to the value of
        // the new key, which is effectively a no-op
        pm.SetString(dest, pm.GetString(src, pm.GetString(dest)));
    }

    /// <summary> Copy a bool from one pref to another </summary>
    public static void CopyBool(string dest, string src)
    {
        pm.SetBool(dest, pm.GetBool(src, pm.GetBool(dest)));
    }
}