using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalApp.Updates
{
    internal sealed class ReleaseVersion : IComparable<ReleaseVersion>
    {
        private readonly Version core;
        private readonly string[] prerelease;
        private readonly string text;
        internal bool IsPrerelease => prerelease.Length != 0;

        private ReleaseVersion(Version core, string[] prerelease, string text)
        {
            this.core = core; this.prerelease = prerelease; this.text = text;
        }

        internal static bool TryParse(string text, out ReleaseVersion version)
        {
            version = null;
            if (text == null) return false;
            string value = text.StartsWith("v", StringComparison.Ordinal) ? text.Substring(1) : text;
            Match match = Regex.Match(value, @"^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?$", RegexOptions.CultureInvariant);
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out int major) ||
                !int.TryParse(match.Groups[2].Value, out int minor) || !int.TryParse(match.Groups[3].Value, out int patch)) return false;
            string[] ids = match.Groups[4].Success ? match.Groups[4].Value.Split('.') : new string[0];
            if (ids.Any(id => Numeric(id) && id.Length > 1 && id[0] == '0')) return false;
            version = new ReleaseVersion(new Version(major, minor, patch), ids, value);
            return true;
        }

        public int CompareTo(ReleaseVersion other)
        {
            if (other == null) return 1;
            int result = core.CompareTo(other.core);
            if (result != 0) return result;
            if (!IsPrerelease || !other.IsPrerelease) return IsPrerelease == other.IsPrerelease ? 0 : IsPrerelease ? -1 : 1;
            for (int i = 0; i < Math.Min(prerelease.Length, other.prerelease.Length); i++)
            {
                string left = prerelease[i], right = other.prerelease[i];
                bool numericLeft = Numeric(left), numericRight = Numeric(right);
                result = numericLeft && numericRight
                    ? (left.Length == right.Length ? string.CompareOrdinal(left, right) : left.Length.CompareTo(right.Length))
                    : numericLeft != numericRight ? (numericLeft ? -1 : 1) : string.CompareOrdinal(left, right);
                if (result != 0) return result;
            }
            return prerelease.Length.CompareTo(other.prerelease.Length);
        }

        private static bool Numeric(string value) => value.All(c => c >= '0' && c <= '9');
        public override string ToString() => text;
    }
}
