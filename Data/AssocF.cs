namespace Loxifi.Data
{
    /// <summary>
    /// The assoc f.
    /// </summary>
    [Flags]
    public enum AssocF
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Init_NoRemapCLSID
        /// </summary>
        Init_NoRemapCLSID = 0x1,

        /// <summary>
        /// Init_ByExeName
        /// </summary>
        Init_ByExeName = 0x2,

        /// <summary>
        /// Init_DefaultToStar
        /// </summary>
        Init_DefaultToStar = 0x4,

        /// <summary>
        /// Init_DefaultToFolder
        /// </summary>
        Init_DefaultToFolder = 0x8,

        /// <summary>
        /// NoUserSettings
        /// </summary>
        NoUserSettings = 0x10,

        /// <summary>
        /// NoTruncate
        /// </summary>
        NoTruncate = 0x20,

        /// <summary>
        /// Verify
        /// </summary>
        Verify = 0x40,

        /// <summary>
        /// RemapRunDll
        /// </summary>
        RemapRunDll = 0x80,

        /// <summary>
        /// NoFixUps
        /// </summary>
        NoFixUps = 0x100,

        /// <summary>
        /// IgnoreBaseClass
        /// </summary>
        IgnoreBaseClass = 0x200,

        /// <summary>
        /// Init_IgnoreUnknown
        /// </summary>
        Init_IgnoreUnknown = 0x400,

        /// <summary>
        /// Init_Fixed_ProgId
        /// </summary>
        Init_Fixed_ProgId = 0x800,

        /// <summary>
        /// Is_Protocol
        /// </summary>
        Is_Protocol = 0x1000,

        /// <summary>
        /// Init_For_File
        /// </summary>
        Init_For_File = 0x2000
    }
}