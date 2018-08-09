using GTA;
using System;

namespace StateZ
{
	[Serializable]
	public class FoodInventoryItem : InventoryItemBase, IFood, IAnimatable, ICraftable
	{
		public string AnimationDict
		{
			get;
			set;
		}

		public string AnimationName
		{
			get;
			set;
		}

		public AnimationFlags AnimationFlags
		{
			get;
			set;
		}

		public int AnimationDuration
		{
			get;
			set;
		}

		public FoodType FoodType
		{
			get;
			set;
		}

		public float RestorationAmount
		{
			get;
			set;
		}

		public CraftableItemComponent[] RequiredComponents
		{
			get;
			set;
		}

		public NearbyResource NearbyResource
		{
			get;
			set;
		}

		public FoodInventoryItem(int amount, int maxAmount, string id, string description, string animationDict, string animationName, AnimationFlags animationFlags, int animationDuration, FoodType foodType, float restorationAmount)
			: base(amount, maxAmount, id, description)
		{
			AnimationDict = animationDict;
			AnimationName = animationName;
			AnimationFlags = animationFlags;
			AnimationDuration = animationDuration;
			FoodType = foodType;
			RestorationAmount = restorationAmount;
		}
	}
}
