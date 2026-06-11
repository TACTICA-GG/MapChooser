using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using SwiftlyS2.Core.Menus.OptionsBase;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using System.Threading.Tasks;

namespace MapChooser.Menu;

public class SetNextMapMenu
{
    private readonly ISwiftlyCore _core;
    private readonly MapLister _mapLister;
    private readonly ThemedMenu _themed;

    public SetNextMapMenu(ISwiftlyCore core, MapLister mapLister)
    {
        _core = core;
        _mapLister = mapLister;
        _themed = new ThemedMenu(core);
    }

    public void Show(IPlayer player, Action<IPlayer, string> onSelect)
    {
        var localizer = _core.Translation.GetPlayerLocalizer(player);
        var currentMapName = _core.ConVar.FindAsString("mapname")?.ValueAsString;
        string title = "Set Next Map:";
        try {
            title = localizer["map_chooser.setnextmap.title"] ?? "Set Next Map:";
        } catch { /* Ignore missing key */ }

        var builder = _themed.CreateBuilder(title);

        foreach (var map in _mapLister.Maps)
        {
            if (!string.IsNullOrEmpty(currentMapName) && map.Name.Equals(currentMapName, StringComparison.OrdinalIgnoreCase)) continue;

            var option = _themed.SelectableOption($"<font color='lightgreen'>{map.Name}</font>");
            option.Click += (sender, args) =>
            {
                _core.Scheduler.NextTick(() => {
                    onSelect(args.Player, map.Name);
                    var currentMenu = _core.MenusAPI.GetCurrentMenu(args.Player);
                    if (currentMenu != null)
                    {
                        _core.MenusAPI.CloseMenuForPlayer(args.Player, currentMenu);
                    }
                });
                return ValueTask.CompletedTask;
            };

            builder.AddOption(option);
        }

        var menu = builder.Build();
        _core.MenusAPI.OpenMenuForPlayer(player, menu);
    }
}
