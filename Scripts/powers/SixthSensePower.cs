using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class SixthSensePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private static bool isActive = true;

    private static int hp = 90;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 0)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/sixth_sense_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/sixth_sense_power.png";

    public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack() && dealer.IsEnemy && isActive && target == Owner)
        {
            hp = Owner.CurrentHp;
        }
        return Task.CompletedTask;
    }
    
    public override bool ShouldDieLate(Creature creature)
    {
        if (creature != base.Owner)
        {
            return true;
        }
        if (!isActive)
        {
            return true;
        }
        return false;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        DynamicVars["count"].UpgradeValueBy(1);
        isActive = false;
        decimal amount = Math.Max(1m, hp);
        await CreatureCmd.Heal(creature, amount);
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Owner, 1, Owner, null);
    }
    
    public override async Task AfterCombatEnd(CombatRoom _)
    {
        isActive = true;
    }
}