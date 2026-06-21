using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class BalanceTheWorldPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override bool IsInstanced => true;

    private static int count = 0; 
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("card", 0)];
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/balance_the_world_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/balance_the_world_power.png";
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner)
        {
            return 1m;
        }
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        if (Owner.Player == null)
        {
            return 1m;
        }

        if (cardSource != null && (cardSource is AshWarModel || cardSource.Tags.Contains(CardTag.Strike)))
        {
            return 1.4m;
        }
        return 1m;
    }
    
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner.Player!=null && Owner==target && dealer != null && props.IsPoweredAttack())
        {
            TempIntangiblePower? tempIntangiblePower = Owner.GetPower<TempIntangiblePower>();
            if (tempIntangiblePower != null)
            {
                await PowerCmd.Remove(tempIntangiblePower);
            }
        }
    }

    public override async Task BeforeAttack(AttackCommand command)
    {
        if (Owner.Player!=null && command.TargetSide==CombatSide.Player && command.Attacker != null)
        {
            if (count < 1)
            {
                count++;
                InvokeDisplayAmountChanged();
                CardModel? card = (await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player))
                    .FirstOrDefault();
                if (card != null && (card.Rarity == CardRarity.Rare || card.Type == CardType.Curse))
                {
                    Flash();
                    await PowerCmd.Apply<TempIntangiblePower>(new ThrowingPlayerChoiceContext(), Owner, 1, Owner, null);
                }
            } else
            {
                TempIntangiblePower? tempIntangiblePower = Owner.GetPower<TempIntangiblePower>();
                if (tempIntangiblePower != null)
                {
                    await PowerCmd.Remove(tempIntangiblePower);
                }
            }
        }
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        count=0;
        InvokeDisplayAmountChanged();
    }
}