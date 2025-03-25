using System;
using System.Collections.Generic;

namespace Utils
{
	// This class holds all common enums used throughout the game.
	// This may need to be restructured if it gets too big.

	[Serializable]
	public enum Resource
	{
		None,
		Wood,
		Ore,
		Food,
		Gold,
		Recruit,
		Count
	}

	public enum Foliage
	{
		None,
		ShortGrass,
		TallGrass,
		Mushrooms,
		Pebbles,
		Count
	}

	[Serializable]
	public enum StorageStatus
	{
		Empty,
		HalfFull,
		Full
	}

	[System.Flags]
	[Serializable]
	public enum TargetMask
	{
		Nothing = 0,
		Player = 1,
		Tree = 2,
		Ore = 4,
		Bush = 8,
		Farm = 16,
		Fish = 32,
		Enemy = 64,
		Boss = 128,
		Building = 256,
		DamagedBuilding = 512,
		Construction = 1024,
		InjuredPlayer = 2048,
		DeadPlayer = 4096
	}

	[Serializable]
	public enum PlayerRole
	{
		Logger,
		Builder,
		Defender,
		Fisher,
		Ruler,
		Miner,
		Ranger,
		Farmer,
		Soldier,
		Gatherer,
		Priest,
		Wizard,
		Necromancer,
		Paladin,
		Blacksmith,
		Forester,
		Count
	}

	[System.Flags]
	[Serializable]
	public enum StationMask
	{
		Nothing = 0,
		Food = 1,
		Ore = 2,
		Wood = 4,
		Fish = 8,
		Combat = 16,
		Buildings = 32,
		EnemyCamp = 64,
		RaidStation = 128
	}

	[System.Flags]
	[Serializable]
	public enum PlayerRoleType
	{
		Nothing = 0,
		Resource = 1,
		Damage = 2,
		Healer = 4,
		Other = 8
	}

	[Serializable]
	public enum BuildingType
	{
		Townhall,
		House,
		Fishinghut,
		Windmill,
		Woodstorage,
		Orestorage,
		Farm,
		Lumbermill,
		Stonemason,
		Foodstorage,
		Tower,
		Barracks,
		Bowyard,
		Wall,
		Gate,
		Torch,
		Fountain,
		Statue1,
		Statue2,
		Forge,
		Marketplace,
		Monastery,
		Necrotower,
		Wizardtower,
		Castle,
		Statue3,
		Foresterhut,
		Count
	}

	[Serializable]
	public enum BuildingState
	{
		Construction,
		Building
	}

	[Serializable]
	public enum TimeOfDay
	{
		Midnight,
		Morning,
		Midday,
		Evening,
		Night,
		Count
	}

	[Serializable]
	public enum PLayerActivityStatus
	{
		LastTenMinutes,
		LastHour,
		Inactive
	}

	[Serializable]
	public enum BodyType
	{
		Slim,
		Bulk,
		Feminine,
		Count
	}

	[Serializable]
	public enum AnimationName
	{
		Action,
		WoodCutting,
		Build,
		BowShoot,
		Casting,
		Farming,
		Fishing,
		Gathering,
		HammerAttack,
		Heal,
		Mining,
		SpearAttack,
		StaffAttack,
		StaffAttackMagic,
		LongSword,
		CarryWood,
		CarryHip,
		GenericAction,
		Death,
		Revive
	}

	[Serializable]
	public enum RunAnimation
	{
		Generic,
		TwoHanded
	}

	public enum ReviveType
	{
		Self,
		Others
	}

	[Serializable]
	public enum WallType
	{
		Straight = 0,
		TCross = 1,
		Cross = 2,
		Corner = 3
	}

	[Serializable]
	public enum Selectable
	{
		Player,
		Building,
		Enemy,
		Resource,
		EnemyCamp
	}

	[Serializable]
	public enum StationUpdate
	{
		Update,
		Clear
	}

	[Serializable]
	public enum SurfaceType
	{
		Ground,
		Shoreline,
		Underwater,
		Undefined
	}

	[Serializable]
	public enum GameEventType
	{
		None,
		FishGod,
		NightRaid,
		BloodMoonRaid,
		AdventureLandNecro,
		AdventureLandFishGod,
		DragonFire,
		DragonForest,
		DragonIce,
		DragonTwoHeaded,
		DragonUndead,
		Subscription,
		BitsDonated
	}

	[Serializable]
	public enum Seasons
	{
		Summer,
		Autumn,
		Winter,
		Spring
	}

	[Serializable]
	public enum Weather
	{
		HeavyRain,
		LighRain

	}
	// Temp enums
	[Serializable]
	public enum SaveItem
	{
		Fish,
		Ore,
		Tree,
		Bush,

		EnemyCamp_Goblin,

		GrassLong1,
		GrassLong2,
		GrassLong3,
		GrassShort1,
		GrassShort2,
		GrassShort3,
		Flower1,
		Flower2,
		Flower3,
		Flower4,
		Flower5,
		Flower6,
		CoralBrown1,
		CoralBrown2,
		CoralOrange1,
		CoralOrange2,
		CoralPurple1,
		CoralPurple2,
		Seaweed1,
		Seaweed2,
		Seaweed3,

		Barracks,
		Bowyard,
		Farm,
		FishingHut,
		House,
		Lumbermill,
		Marketplace,
		Monastery,
		NecromancerTower,
		StoneMason,
		FoodStorage,
		OreStorage,
		WoodStorage,
		Wall,
		Gate,
		Tower,
		Windmill,
		WizardTower,
		Statue1,
		Statue2,
		Fountain,
        ForesterHut,
        Count
	}

	[Serializable]
	public enum ResourceType
	{
		Fish,
		Ore,
		Tree,
		Bush,
		Count
	}

	[Serializable]
	public enum FoliageSaveType
	{
		GrassLong1,
		GrassLong2,
		GrassLong3,
		GrassShort1,
		GrassShort2,
		GrassShort3,
		Flower1,
		Flower2,
		Flower3,
		Flower4,
		Flower5,
		Flower6,
		CoralBrown1,
		CoralBrown2,
		CoralOrange1,
		CoralOrange2,
		CoralPurple1,
		CoralPurple2,
		Seaweed1,
		Seaweed2,
		Seaweed3,
	
		Count
	}

	[Serializable]
	public enum Season
	{
		Summer,
		Autumn,
		Winter,
		Spring,
		Count
	}

	[Serializable]
	public enum StatType
	{
		Health,
		Defense,
		HealthRegen,
		ActionAmount,
		ActionRange,
		ActionSpeed,
		MovementSpeed,
		Count
	}

	[System.Serializable]
	public enum TechType
	{
		StatBoost,
		UnlockBuilding,
		BuildingCostReduction,
		StorageBoost,
		UpgradeBuilding,
		AgeUpBuilding
	}

	[Serializable]
	public enum Age
	{
		Age1,
		Age2,
		Age3
	}

	[Serializable]
	public enum EnemyType
	{
		Goblin,
		Blargul,
		GoblinBoss,
		Minotaur,
		NecroSlasher,
		NecroStalker,
		Skeleton,
		MinotaurBoss,
		BatteringRam,
		Count
	}
	
	[Serializable]
	public enum FoliageType
	{
		GrassLong1,
		GrassLong2,
		GrassLong3,
		GrassShort1,
		GrassShort2,
		GrassShort3,
		Flower1,
		Flower2,
		Flower3,
		Flower4,
		Flower5,
		Flower6,
		Count
	}

	[Serializable]
	public static class TargetFlagHelper
	{
		//TODO:: Find all flags that use reflection Enum.GetFlags
		private static List<TargetMask> _targetFlags;

		//Cached Index Data
		private static Dictionary<TargetMask, int> _cachedIndices;

		public static List<TargetMask> TargetFlags
		{
			get
			{
				if (_targetFlags == null)
				{
					int index = -1; //Starts at -1 to count for 'none'
					_targetFlags = new List<TargetMask>();
					_cachedIndices = new Dictionary<TargetMask, int>();
					foreach (int i in Enum.GetValues(typeof(TargetMask)))
					{
						TargetMask t = (TargetMask)i;

						_targetFlags.Add(t);
						_cachedIndices.Add(t, index++);
					}
				}

				return _targetFlags;
			}
		}

		public static int GetFlagCount(int mask)
		{
			int count = 0;
			do
			{
				int hasFlag = mask & 1;
				if (hasFlag == 1)
					count++;

			} while ((mask >>= 1) != 0);

			return count;

		}

		public static int GetIndexByFlag(TargetMask flag)
		{
			return _cachedIndices[flag];
		}

		public static int TargetFlagCount => TargetFlags.Count;
	}
}