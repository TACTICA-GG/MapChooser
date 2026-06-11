using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Core.Menus.OptionsBase;

namespace MapChooser.Menu;

/// <summary>
/// Shared themed image-menu helper for MapChooser. Renders the SwiftlyS2 center-HTML
/// menu with SVG footer buttons and a per-row select icon on the highlighted option.
/// </summary>
public class ThemedMenu
{
    // Base URL of the SVG button icons (Pisex cs2-menus assets).
    // jsDelivr CDN mirror of the GitHub repo: globally edge-cached with proper
    // cache headers, so CEF downloads each icon once and reuses it instantly.
    // (raw.githubusercontent.com has no CDN and rate-limits — much slower.)
    private const string MenuBtnUrl =
        "https://cdn.jsdelivr.net/gh/Pisex/cs2-menus@main/menu_buttons/site";

    // One <img> tag. Native SVG size (no width/height).
    private static string Img(string name) => $"<img src='{MenuBtnUrl}/{name}.svg'/>";

    // The image "footer" row (goes in the comment slot).
    private static readonly string FooterButtons =
        $"{Img("w")} {Img("s")} {Img("empty")} {Img("f")}";

    // Select icon appended only to the row the player is currently navigating.
    private static readonly string SelectIcon = " " + Img("e");

    private readonly ISwiftlyCore _core;

    public ThemedMenu(ISwiftlyCore core)
    {
        _core = core;
    }

    /// <summary>
    /// Creates a builder with the shared MapChooser theme (image footer, white cursor/guides).
    /// </summary>
    public IMenuBuilderAPI CreateBuilder(string title)
    {
        var builder = _core.MenusAPI.CreateBuilder();
        var design = builder.Design;
        design.SetMenuTitle(title);
        design.SetMenuTitleVisible(true);          // title + auto ─── line under it
        design.SetMenuTitleItemCountVisible(true); // "[2/7]" counter
        design.SetMenuFooterVisible(false);        // hide built-in TEXT footer
        design.SetCommentVisible(true);            // comment slot holds image buttons
        design.SetDefaultComment(FooterButtons);   // SVG buttons rendered as <img>
        design.SetNavigationMarkerColor("#FFFFFF");// white ► cursor
        design.SetVisualGuideLineColor("#FFFFFF"); // white ─── guide lines
        design.SetDisabledColor("#808080");        // grey disabled options
        design.SetMaxVisibleItems(3);              // rows per page (1–5)
        return builder;
    }

    /// <summary>
    /// Creates a ButtonMenuOption that shows the select icon only on the highlighted row.
    /// </summary>
    public ButtonMenuOption SelectableOption(string text)
    {
        var opt = new ButtonMenuOption(text);
        opt.AfterFormat += (_, args) =>
        {
            var menu = _core.MenusAPI.GetCurrentMenu(args.Player);
            if (menu != null && ReferenceEquals(menu.GetCurrentOption(args.Player), args.Option))
                args.CustomText += SelectIcon;     // e.svg only on highlighted row
        };
        return opt;
    }
}
