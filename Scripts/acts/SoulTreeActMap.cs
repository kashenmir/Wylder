using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace wylder.Scripts.acts;

/// <summary>
/// [SoulTree Mod] A linear map for Act 4 (SoulTree).
/// Layout: Start → Shop → RestSite → Boss
/// No second boss, no branching paths.
/// </summary>
public sealed class SoulTreeActMap : ActMap
{
	public override MapPoint BossMapPoint { get; }

	public override MapPoint StartingMapPoint { get; }

	/// <summary>
	/// [SoulTree Mod] SoulTree act never has a second boss.
	/// </summary>
	public override MapPoint? SecondBossMapPoint => null;

	protected override MapPoint?[,] Grid { get; }

	public SoulTreeActMap(RunState runState)
	{
		// [SoulTree Mod] Linear map: Start(row0) → Shop(row1) → RestSite(row2) → Boss(row3)
		// Grid dimensions: 1 column × 3 rows (rows 1-3 for shop/rest/boss path nodes).
		// StartingMapPoint (row 0) and BossMapPoint (row 3) are NOT stored in Grid —
		// they are handled separately by ActMap.GetPoint which checks them before Grid lookup.
		// This matches StandardActMap where Starting/Boss are outside Grid.
		// Grid[col, row] must match MapPoint.coord.col and MapPoint.coord.row for GetPoint/HasPoint lookups.
		Grid = new MapPoint[7, 3];

		StartingMapPoint = new MapPoint(3, 0);
		MapPoint restPoint = new MapPoint(3, 1);
		BossMapPoint = new MapPoint(3, 2);

		// Build the single linear path
		StartingMapPoint.AddChildPoint(restPoint);
		restPoint.AddChildPoint(BossMapPoint);

		// Assign point types
		StartingMapPoint.PointType = MapPointType.Ancient;
		StartingMapPoint.CanBeModified = false;
		restPoint.PointType = MapPointType.RestSite;
		restPoint.CanBeModified = false;
		BossMapPoint.PointType = MapPointType.Boss;
		BossMapPoint.CanBeModified = false;

		// Register start points (row 1 nodes that StartingMapPoint connects to)
		startMapPoints.Add(restPoint);

		// Store ONLY the middle path nodes in grid at their correct [col, row] positions.
		// StartingMapPoint and BossMapPoint are handled by ActMap.GetPoint's special-case checks,
		// so they must NOT be in Grid (otherwise NMapScreen.DrawPaths draws their edges twice).
		Grid[3, 0] = null; // StartingMapPoint is at (0,0) but NOT in Grid
		Grid[3, 1] = restPoint;
		Grid[3, 2] = null; // BossMapPoint is at (0,3) but NOT in Grid
	}
}
