using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class NightFormPower : CustomPowerModel
{
    private CardModel? _mockSelectedCard;
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override bool IsInstanced => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("card", 0)];
        
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/night_form_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/night_form_power.png";
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource == null || cardSource is not NightForm)
        {
            return;
        }

        if (cardSource.IsUpgraded)
        {
            DynamicVars["card"].UpgradeValueBy(1);
        }
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (Owner.Player !=null && player == Owner.Player)
        {
            for (int i = 0; i < Amount; i++)
            {
                CardModel cardModel;
                if (_mockSelectedCard == null)
                {
                    List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner.Player, generateNightCards(), 3, Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
                    if (DynamicVars["card"].IntValue > 0)
                    {
                        foreach (CardModel item in cards)
                        {
                            CardCmd.Upgrade(item);
                        }   
                    }
                    cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner.Player, canSkip: true);
                }
                else
                {
                    cardModel = _mockSelectedCard;
                }
                if (cardModel != null)
                {
                    cardModel.SetToFreeThisTurn();
                    await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
                }
            }
        }
    }
    
    public void MockSelectedCard(CardModel card)
    {
        AssertMutable();
        _mockSelectedCard = card;
    }

    private IEnumerable<CardModel> generateNightCards()
    {
        return (IEnumerable<CardModel>)new CardModel[8]
        {
            ModelDb.Card<BeastsHunt>(),
            ModelDb.Card<IntegrationOfIntelligence>(),
            ModelDb.Card<DemonsPlating>(),
            ModelDb.Card<ChampionsBlessing>(),
            ModelDb.Card<ColdMirage>(),
            ModelDb.Card<UnendingHunger>(),
            ModelDb.Card<UnifyingFate>(),
            ModelDb.Card<ResentmentOfDregs>()
        };
    }
}
