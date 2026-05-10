using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.powers;

public class StarCallerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/star_caller_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/star_caller_power.png";
    
    private bool isInActive = true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }

        if (cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }
        await PowerCmd.Apply<StarCallerStrenthPower>(base.CombatState.HittableEnemies, base.Amount, base.Owner, null);
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }
}