using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace wylder.Scripts.monsters;

public class LipulaBoss : CustomEncounterModel
{
    // 所有可能出现的怪物
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Lipula>()];

    // 这个遭遇在那些层级出现
    public override bool IsValidForAct(ActModel act) => act.ActNumber() == 1; // 只在第一幕出现

    public override RoomType RoomType => RoomType.Monster;
    
    // 这个遭遇是否是弱怪池
    public override bool IsWeak => false;

    public LipulaBoss() : base(RoomType.Monster) // 这个遭遇的房间类型，这里是普通怪物
    {
    }

    // 不要忘了这里的model需要调用ToMutable()，表示不是标准值而是战斗中的可变数据
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
        (ModelDb.Monster<Lipula>().ToMutable(), null) // 如果不想指定怪物生成在哪个槽位，可以直接传null，系统会自动分配
    ];
}