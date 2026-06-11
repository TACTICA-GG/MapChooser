using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using SwiftlyS2.Core.Menus.OptionsBase;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;
using System.Threading.Tasks;

namespace MapChooser.Menu;

public class EndOfMapVoteMenu
{
    private readonly ISwiftlyCore _core;
    private readonly MapCooldown _mapCooldown;
    private readonly ThemedMenu _themed;

    public EndOfMapVoteMenu(ISwiftlyCore core, MapCooldown mapCooldown)
    {
        _core = core;
        _mapCooldown = mapCooldown;
        _themed = new ThemedMenu(core);
    }

    public IMenuAPI Show(IPlayer player, List<string> mapsInVote, Action<IPlayer, string> onVote)
    {
        var localizer = _core.Translation.GetPlayerLocalizer(player);
        var builder = _themed.CreateBuilder(localizer["map_chooser.vote.title"] ?? "Vote for the next map:");
        foreach (var map in mapsInVote)
        {
            string displayName = map;
            bool isExtend = map == "map_chooser.extend_option";
            if (isExtend)
            {
                displayName = localizer["map_chooser.extend_option"];
            }

            var option = _themed.SelectableOption($"{(isExtend ? "<font color='orange'>" : "<font color='lightgreen'>")}{displayName}</font>");
            option.Enabled = isExtend || !_mapCooldown.IsMapInCooldown(map);
            option.Click += (sender, args) =>
            {
                _core.Scheduler.NextTick(() => {
                    onVote(args.Player, map);
                });
                return ValueTask.CompletedTask;
            };

            builder.AddOption(option);
        }

        var menu = builder.Build();
        menu.Tag = "EofVoteMenu";
        _core.MenusAPI.OpenMenuForPlayer(player, menu);
        return menu;
    }
}
