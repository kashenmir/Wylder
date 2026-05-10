using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class IntegrationOfIntelligencePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override bool IsInstanced => true;
    
    public override int DisplayAmount => DynamicVars["count"].IntValue;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/integration_of_intelligence_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/integration_of_intelligence_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 100)];
    
    private static readonly Random _random = new Random();
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && DynamicVars["count"].IntValue > 0 && Owner.Player != null && Check(25))
        {
            Flash();
            Vector2? monsterPos = null;
            if (TestMode.IsOff)
            {
                monsterPos = NCombatRoom.Instance.GetCreatureNode(target)?.VfxSpawnPosition;
            }
            if (monsterPos.HasValue)
            {
                VfxCmd.PlayVfx(monsterPos.Value, "vfx/vfx_coin_explosion_regular");
            }
            await PlayerCmd.GainGold(20, Owner.Player);
            DynamicVars["count"].UpgradeValueBy(-20);
            InvokeDisplayAmountChanged();
        }
    }
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource == null || cardSource is not IntegrationOfIntelligence)
        {
            return;
        }

        if (cardSource.IsUpgraded)
        {
            DynamicVars["count"].UpgradeValueBy(20);
            InvokeDisplayAmountChanged();
        }
    }
    
    /// <summary>
    /// 检查是否命中指定概率
    /// </summary>
    /// <param name="percent">概率百分比 (0-100)</param>
    /// <returns>命中返回 true，否则返回 false</returns>
    public static bool Check(int percent)
    {
        return _random.Next(0, 100) < percent;
    }
}