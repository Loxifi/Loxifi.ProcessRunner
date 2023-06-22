namespace Loxifi.Data
{
    [Flags]
    internal enum HANDLE_FLAGS : uint
    {
        None = 0,

        INHERIT = 1,

        PROTECT_FROM_CLOSE = 2
    }
}