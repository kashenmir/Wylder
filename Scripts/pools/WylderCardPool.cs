using BaseLib.Abstracts;
using Godot;

namespace wylder.Scripts.pools;

public class WylderCardPool : CustomCardPoolModel
{
    // 卡池的ID。必须唯一防撞车。
    public override string Title => "wylder";

    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://wylder/images/energy_test.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => "res://wylder/images/energy_test_big.png";

    // 卡池的主题色。
    public override Color DeckEntryCardColor => new(0.1f, 0.1f, 0.3f);

    // 如果你使用默认的卡框，可以使用这个颜色来修改卡框的颜色。
    public override Color ShaderColor => new(0.1f, 0.1f, 0.3f);

    // 如果你使用自定义卡框图片，重写CustomFrame方法并返回你的卡框图片。
    // public override Texture2D? CustomFrame(CustomCardModel card)
    // {
    //     return card.Type switch
    //     {
    //         CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("res://test/images/card_frame_attack.png"),
    //         CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("res://test/images/card_frame_power.png"),
    //         _ => PreloadManager.Cache.GetAsset<Texture2D>("res://test/images/card_frame_skill.png"),
    //     };
    // }

    // 卡池是否是无色。例如事件、状态等卡池就是无色的。
    public override bool IsColorless => false;
}