using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;
using wylder.Scripts.dynamicVars;

namespace wylder.Scripts.powers;

public class FireChasePower : ChasePowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/basic_chase_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/basic_chase_power.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new MagicBuff(2)];
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
        {
            return 0m;
        }
        if (cardSource == null)
        {
            return 0m;
        }
        if (!cardSource.Tags.Contains(CardTag.Strike))
        {
            return 0m;
        }
        if (dealer != Owner)
        {
            return 0m;
        }
        return Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || isInActive)
        {
            isInActive = false;
            return;
        }

        if (cardPlay.Card is not Suibu && cardPlay.Card is not HuntStep) {
            await PowerCmd.Remove(this);
        }
    }
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && cardSource!=null && cardSource.Tags.Contains(CardTag.Strike))
        {
            Flash();
            await PowerCmd.Apply<FireGreasePower>(choiceContext, Owner, DynamicVars["MagicBuff"].IntValue, Owner, null);
            Frost? frost = target.GetPower<Frost>();
            if (frost != null)
            {
                await PowerCmd.Remove(frost);
            }
            await PowerCmd.Remove(this);
        }
    }
}