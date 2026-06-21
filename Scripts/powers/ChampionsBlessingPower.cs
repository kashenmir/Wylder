using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace wylder.Scripts.powers;

public class ChampionsBlessingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/champions_blessing_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/champions_blessing_power.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    private int count = 0;
    
    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == Owner && Owner.Player!=null && Owner.Player.PlayerCombatState!=null && Owner.Player.PlayerCombatState.Energy < 1  && count<Amount)
        {
            count++;
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && Owner.Player!=null && Owner.Player.PlayerCombatState!=null && Owner.Player.PlayerCombatState.Energy < 1  && count<Amount)
        {
            count++;
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            count = 0;
        }
    }
}