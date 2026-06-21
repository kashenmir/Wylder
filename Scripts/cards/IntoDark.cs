using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class IntoDark : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public IntoDark() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.Player is not null)
        {
            foreach (CardModel item in PileType.Hand.GetPile(base.Owner.Creature.Player).Cards.Where((CardModel c) => c.Enchantment == null))
            {
                CardModel select = CardFactory.GetDistinctForCombat(base.Owner, generateNightCards(), 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (item.Type == CardType.Attack)
                {
                    if (select is FireGrease)
                    {
                        CardCmd.Enchant<Inky>(item, 1m);
                    } else if (select is FreezingGrease)
                    {
                        CardCmd.Enchant<Corrupted>(item, 1m);
                    } else if (select is PoisonGrease)
                    {
                        CardCmd.Enchant<Sharp>(item, 4m);
                    }  else if (select is SleepGrease)
                    {
                        CardCmd.Enchant<Vigorous>(item, 10m);
                    } else if (select is BloodGrease)
                    {
                        CardCmd.Enchant<Swift>(item, 2);
                    } else if (select is Crab)
                    {
                        CardCmd.Enchant<Instinct>(item, 1);
                    } else if (select is StarSharp)
                    {
                        CardCmd.Enchant<Glam>(item, 1);
                    } else if (select is TurtleNeck)
                    {
                        CardCmd.Enchant<Adroit>(item, 4);
                    } else if (select is Boluses)
                    {
                        CardCmd.Enchant<Sharp>(item, 5);
                    } else
                    {
                        CardCmd.Enchant<Momentum>(item, 4);
                    }
                }
                if (item.Type is CardType.Power)
                {
                    if (select is FireGrease || select is FreezingGrease || select is PoisonGrease)
                    {
                        CardCmd.Enchant<Glam>(item, 1);
                    } else if (select is SleepGrease || select is BloodGrease || select is Crab)
                    {
                        CardCmd.Enchant<Sown>(item, 1);
                    } else
                    {
                        CardCmd.Enchant<Swift>(item, 3);
                    } 
                }
                if (item.Type is CardType.Skill)
                {
                    if (select is FireGrease || select is FreezingGrease)
                    {
                        CardCmd.Enchant<Swift>(item, 2);
                    } else if (select is PoisonGrease || select is SleepGrease)
                    {
                        if (item is SacredFlask)
                        {
                            CardCmd.Enchant<Adroit>(item, 4);
                        }
                        else
                        {
                            CardCmd.Enchant<Glam>(item, 1);
                        }
                    } else if (select is BloodGrease || select is Crab)
                    {
                        CardCmd.Enchant<Sown>(item, 1);
                    } else if (select is StarSharp || select is TurtleNeck)
                    {
                        if (item.Keywords.Contains(CardKeyword.Retain))
                        {
                            CardCmd.Enchant<Sown>(item, 1);
                        }
                        else
                        {
                            CardCmd.Enchant<Steady>(item, 1);
                        }
                    } else
                    {
                        if (item.GainsBlock)
                        {
                            CardCmd.Enchant<Nimble>(item, 6);
                        }
                        else
                        {
                            CardCmd.Enchant<Adroit>(item, 4);
                        }
                    }
                }
            }
        }
        CardModel? cardModel = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<CurseCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null) 
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Discard, null));
        }
        cardModel = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<CurseCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Draw, null));
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
    
    private IEnumerable<CardModel> generateNightCards()
    {
        return (IEnumerable<CardModel>)new CardModel[]
        {
            ModelDb.Card<FireGrease>(),
            ModelDb.Card<FreezingGrease>(),
            ModelDb.Card<PoisonGrease>(),
            ModelDb.Card<SleepGrease>(),
            ModelDb.Card<BloodGrease>(),
            ModelDb.Card<Flesh>(),
            ModelDb.Card<Crab>(),
            ModelDb.Card<TurtleNeck>(),
            ModelDb.Card<Boluses>(),
            ModelDb.Card<StarSharp>(),
        };
    }
}