using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using wylder.Scripts.acts;
using wylder.Scripts.relics;

namespace wylder.Scripts.ancient;

[RegisterActAncient(typeof(SoulTree))] 
public class Gallie : ModAncientEventTemplate
{
    // 选项按钮颜色
    public override Color ButtonColor => new(0.12f, 0.2f, 0.8f, 0.5f);
    // 对话框颜色
    public override Color DialogueColor => new(0.12f, 0.2f, 0.8f);
    
    // 自定义场景的路径
    public override EventAssetProfile AssetProfile => new(
        BackgroundScenePath: "res://wylder/scenes/ancient/gallie.tscn"
    );
    
    // 自定义地图图标和轮廓的路径
    public override AncientEventPresentationAssetProfile AncientPresentationAssetProfile => new(
        MapIconPath: "res://wylder/scenes/ancient/gallie_map_icon.png",
        MapIconOutlinePath: "res://wylder/scenes/ancient/gallie_map_icon_outline.png",
        RunHistoryIconPath: "res://wylder/scenes/ancient/gallie_icon.png",
        RunHistoryIconOutlinePath: "res://wylder/scenes/ancient/gallie_icon_outline.png"
    );
    
    // 固定池一和二
    private IReadOnlyList<EventOption> Pool1 => [
        CreateModRelicOption<StarShardFragment>(),
        CreateModRelicOption<PickledTurtleNeck>(),
        CreateModRelicOption<FirePotBundle>(),
    ];
    private IReadOnlyList<EventOption> Pool2 => [
        CreateModRelicOption<SmithingStone>(),
        CreateModRelicOption<ExaltedFlesh>(),
        CreateModRelicOption<BoiledCrab>()
    ];

    private IReadOnlyList<EventOption> Pool3 => [
        CreateModRelicOption<CrimsonwhorlBubbletearRelic>(),
        CreateModRelicOption<OpalineBubbletearRelic>(),
        CreateModRelicOption<ScarletDewBubbletearRelic>()
    ];

    // 所有可能的选项
    public override IEnumerable<EventOption> AllPossibleOptions => [.. Pool1, .. Pool2, .. Pool3];

    // 生成选项
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            Rng.NextItem(Pool1)!,
            Rng.NextItem(Pool2)!,
            Rng.NextItem(Pool3)!,
        ];
    }
}
