using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.relics;

namespace wylder.Scripts.powers;

public class CrucibleFeatherPower : CustomTemporaryDexPower
{
    public override AbstractModel OriginModel => ModelDb.Relic<CrucibleFeather>();

    protected override bool IsPositive => true;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/crucible_feather_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/crucible_feather_power.png";
}