using System;
namespace ContosoTides
{
	public class EventArgsT<T>
	{
		public T Value { get;  }

		public EventArgsT(T val)
		{
			this.Value = val;
		}
	}
}
