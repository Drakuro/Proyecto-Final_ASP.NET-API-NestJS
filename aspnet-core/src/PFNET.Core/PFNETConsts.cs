using PFNET.Debugging;

namespace PFNET;

public class PFNETConsts
{
    public const string LocalizationSourceName = "PFNET";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = false;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "069258d1c8694e259ce5720ee018c827";
}

