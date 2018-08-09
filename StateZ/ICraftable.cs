namespace StateZ
{
	public interface ICraftable
	{
		CraftableItemComponent[] RequiredComponents
		{
			get;
			set;
		}
	}
}
