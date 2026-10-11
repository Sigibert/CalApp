using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CalApp.Updates
{
    internal sealed class UpdateResult
    {
        internal ReleaseVersion Version { get; }
        internal Uri Download { get; }
        internal UpdateResult(ReleaseVersion version, Uri download) { Version = version; Download = download; }
    }

    internal sealed class UpdateLookupException : Exception
    {
        internal string MessageKey { get; }
        internal UpdateLookupException(string key) { MessageKey = key; }
    }

    internal sealed class UpdateChecker : IDisposable
    {
        internal static readonly Uri ReleasesPage = new Uri("https://github.com/Sigibert/CalApp/releases");
        private readonly HttpClient client;

        internal UpdateChecker(HttpClient client = null)
        {
            this.client = client ?? new HttpClient();
            this.client.Timeout = TimeSpan.FromSeconds(10);
            this.client.MaxResponseContentBufferSize = 1024 * 1024;
        }

        internal async Task<UpdateResult> CheckAsync(ReleaseVersion current, CancellationToken cancellation)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/Sigibert/CalApp/releases?per_page=100"))
            {
                request.Headers.UserAgent.ParseAdd("WeekTrayInfo/" + current);
                request.Headers.Accept.ParseAdd("application/vnd.github+json");
                using (HttpResponseMessage response = await client.SendAsync(request, cancellation).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) throw new UpdateLookupException("UpdatePrivate");
                    if (response.StatusCode == HttpStatusCode.Forbidden || (int)response.StatusCode == 429) throw new UpdateLookupException("UpdateLimited");
                    if (!response.IsSuccessStatusCode) throw new UpdateLookupException("UpdateUnavailable");
                    using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Release[]));
                        var releases = serializer.ReadObject(stream) as Release[] ?? new Release[0];
                        cancellation.ThrowIfCancellationRequested();
                        var candidates = releases.Where(release => release != null && !release.Draft)
                            .Select(release => new { Release = release, Version = Parse(release.Tag) })
                            .Where(item => item.Version != null && (current.IsPrerelease || (!item.Release.Prerelease && !item.Version.IsPrerelease)))
                            .OrderByDescending(item => item.Version).ToArray();
                        if (candidates.Length == 0) return new UpdateResult(null, ReleasesPage);
                        var latest = candidates[0];
                        string[] names = { "WeekTrayInfo-Setup-" + latest.Version + ".exe",
                            "CalApp-Setup-" + latest.Version + ".exe", "WeekTrayInfo-" + latest.Version + ".zip",
                            "CalendarWeekInfo-" + latest.Version + ".zip" };
                        Asset[] assets = latest.Release.Assets ?? new Asset[0];
                        Asset asset = names.Select(name => assets.FirstOrDefault(a => a != null && a.Name == name && SafeReleaseUri(a.Url) != null))
                            .FirstOrDefault(a => a != null);
                        Uri download = asset == null ? SafeReleaseUri(latest.Release.Url) ?? ReleasesPage : SafeReleaseUri(asset.Url);
                        return new UpdateResult(latest.Version, download);
                    }
                }
            }
        }

        internal static Uri SafeReleaseUri(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uri) || uri.Scheme != "https" ||
                !uri.IsDefaultPort || !string.IsNullOrEmpty(uri.UserInfo) ||
                !string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase)) return null;
            string path = uri.AbsolutePath;
            return path.Equals("/Sigibert/CalApp/releases", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Sigibert/CalApp/releases/", StringComparison.OrdinalIgnoreCase) ? uri : null;
        }

        private static ReleaseVersion Parse(string text) => ReleaseVersion.TryParse(text, out ReleaseVersion version) ? version : null;
        public void Dispose() => client.Dispose();

        [DataContract]
        private sealed class Release
        {
            [DataMember(Name = "tag_name")] public string Tag { get; set; }
            [DataMember(Name = "html_url")] public string Url { get; set; }
            [DataMember(Name = "draft")] public bool Draft { get; set; }
            [DataMember(Name = "prerelease")] public bool Prerelease { get; set; }
            [DataMember(Name = "assets")] public Asset[] Assets { get; set; }
        }

        [DataContract]
        private sealed class Asset
        {
            [DataMember(Name = "name")] public string Name { get; set; }
            [DataMember(Name = "browser_download_url")] public string Url { get; set; }
        }
    }
}
