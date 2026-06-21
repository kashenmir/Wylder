using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class UnendingHungerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/unending_hunger_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/unending_hunger_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Strength", 2), new DamageVar(5, ValueProp.Move)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player && Owner.Player != null)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
            CardPile pile = PileType.Discard.GetPile(Owner.Player);
            int num = Math.Min(Amount, pile.Cards.Count);
            if (num > 0)
            {
                IEnumerable<CardModel> cardModels = await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner.Player,
                    new CardSelectorPrefs(SelectionScreenPrompt, num));
                foreach (CardModel item in cardModels)
                {
                    await CardCmd.Exhaust(choiceContext, item);
                }
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, num * DynamicVars["Strength"].IntValue, Owner, null);
            }
            if (Amount-num > 0)
            {
                Flash();
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner,
                    (Amount-num) * DynamicVars.Damage.IntValue, ValueProp.Unblockable | ValueProp.Unpowered,
                    null, null);
            }
        }
    }
}