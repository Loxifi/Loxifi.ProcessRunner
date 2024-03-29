﻿namespace Loxifi.Data
{
    [Flags]
    internal enum PROCESS_CREATION_FLAGS : uint
    {
        DEBUG_PROCESS = 1 << 0,

        DEBUG_ONLY_THIS_PROCESS = 1 << 1,

        CREATE_SUSPENDED = 1 << 2,

        DETACHED_PROCESS = 1 << 3,

        CREATE_NEW_CONSOLE = 1 << 4,

        CREATE_NEW_PROCESS_GROUP = 1 << 9,

        CREATE_UNICODE_ENVIRONMENT = 1 << 10,

        CREATE_SEPARATE_WOW_VDM = 1 << 11,

        CREATE_SHARED_WOW_VDM = 1 << 12,

        INHERIT_PARENT_AFFINITY = 1 << 16,

        CREATE_PROTECTED_PROCESS = 1 << 18,

        EXTENDED_STARTUPINFO_PRESENT = 1 << 19,

        CREATE_SECURE_PROCESS = 1 << 22,

        CREATE_BREAKAWAY_FROM_JOB = 1 << 24,

        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 1 << 25,

        CREATE_DEFAULT_ERROR_MODE = 1 << 26,

        CREATE_NO_WINDOW = 1 << 27
    }
}