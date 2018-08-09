using System;

namespace StateZ
{
	public interface IValidatable
	{
		Func<bool> Validation
		{
			get;
			set;
		}
	}
}
