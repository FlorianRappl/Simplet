using System;

namespace Simplet.Utils
{
    internal static class VersionUtils
    {
        public static string GetVersion(string version)
        {
            switch (version)
            {
                case "":
                case "auto":
                case null:
                    return GetAutoVersion();
                default:
                    return version;
            }
        }

        public static string GetAutoVersion() => $"1.0.0-pre.{DateTime.Now.ToString("yyyy.M.d")}.{GetBuildId()}";

        private static string GetBuildId() =>
            Environment.GetEnvironmentVariable("BUILD_BUILDID") ??
            Environment.GetEnvironmentVariable("BUILD_VERSION") ??
            Environment.GetEnvironmentVariable("BUILDID") ??
            Environment.GetEnvironmentVariable("BUILD_NUMBER") ??
            Environment.GetEnvironmentVariable("BUILD_ID") ??
            "0";
    }
}
