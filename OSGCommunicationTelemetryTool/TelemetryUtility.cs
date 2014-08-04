using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OSGCommunicationTelemetryTool
{
    internal static class TelemetryUtility
    {
        private const string script = @"
<iframe width=""1"" height=""1"" name=""invisibleFrame"" style=""display:none""></iframe>
<form id=""telemetryForm"" action=""http://eslabs/telemetryproxy/submit"" method=""POST"" style=""display:none"" target=""invisibleFrame"">
    <input id=""appId"" type=""hidden"" name=""appId"" value=""14"">
    <input id=""dataPointId"" type=""hidden"" name=""dataPointId"" value=""116"">
    <input id=""markerData"" type=""hidden"" name=""data"" value=""none"">
</form>
<script type=""text/javascript"">
function onPageVisit(name) {
    document.getElementById(""dataPointId"").value = 115;
    document.getElementById(""markerData"").value = name;
    document.getElementById(""telemetryForm"").submit();
};
function onLinkClick(url) {
    document.getElementById(""dataPointId"").value = 116;
    document.getElementById(""markerData"").value = url;
    document.getElementById(""telemetryForm"").submit();
};
onPageVisit('PAGE_NAME');
</script>
";

            public static void Instrument(string input, string output, string pageName)
            {
                var text = File.ReadAllText(input, Encoding.GetEncoding(1252));
                var builder = new StringBuilder(text);
                var pattern = new Regex(@"\s+href=""(?<url>.*?)""", RegexOptions.Singleline);
                var matches = pattern.Matches(text).Cast<Match>().ToArray();
                for (int i = matches.Length - 1; i >= 0; i--)
                {
                    var index = matches[i].Index + matches[i].Length;
                    var url = matches[i].Groups["url"].Value;
                    builder.Insert(index, @" onclick=""javascript:onLinkClick('" + url + @"')""");
                }

                text = builder.ToString();
                var pos = text.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                builder.Insert(pos, script.Replace("PAGE_NAME", pageName));
                File.WriteAllText(output, builder.ToString(), Encoding.GetEncoding(1252));
            }
        }
}
