namespace WreFigDemo;

/// <summary>
/// Application-level display settings bound from appsettings.json → "App" section.
/// Change Title / FullTitle there and restart — no code changes needed.
/// </summary>
public class AppConfig
{
    /// <summary>Short name used in page &lt;title&gt; tags, e.g. "FIG".</summary>
    public string Title    { get; set; } = "FIG";

    /// <summary>Full name shown in the top navigation bar, e.g. "FIG (Field Information Guide)".</summary>
    public string FullTitle { get; set; } = "FIG (Field Information Guide)";
}
