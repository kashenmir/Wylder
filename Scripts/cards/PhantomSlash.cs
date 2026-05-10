using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;

namespace wylder.Scripts.cards;

[Pool(typeof(WylderCardPool))]
public class PhantomSlash : TestCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move)];
    
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    public PhantomSlash() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        CardModel selection = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), context: choiceContext, player: base.Owner, filter: delegate(CardModel c)
        {
            CardType type = c.Type;
            return type == CardType.Attack ? true : false;
        }, source: this)).FirstOrDefault();
        if (selection != null)
        {
            decimal damage = default(decimal);
            if (selection.DynamicVars.ContainsKey("CalculatedDamage"))
            {
                damage = selection.DynamicVars.CalculatedDamage.Calculate(null);
            }
            else if (selection.DynamicVars.ContainsKey("Damage"))
            {
                damage = selection.DynamicVars.Damage.BaseValue;
            }
            else if (selection.DynamicVars.ContainsKey("OstyDamage"))
            {
                damage = selection.DynamicVars.OstyDamage.BaseValue;
            }
            else
            {
                Log.Warn(base.Id.Entry + " exhausted attack card " + selection.Id.Entry + " that did not have an appropriate damage var!");
            }
            damage = Hook.ModifyDamage(base.Owner.RunState, base.Owner.Creature.CombatState, null, base.Owner.Creature, damage, ValueProp.Move, selection, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await CardCmd.Exhaust(choiceContext, selection);
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}