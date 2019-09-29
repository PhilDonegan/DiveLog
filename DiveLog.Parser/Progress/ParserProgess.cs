using System;
using System.Collections.Generic;
using System.Text;

namespace DiveLog.Parser.Progress
{
	[Serializable]
	public class ParserProgess
	{
		public ParserProgess(int CurrentDive, int TotalDives)
		{
			this.CurrentDive = CurrentDive;
			this.TotalDives = TotalDives;
		}

		public int CurrentDive { get; }

		public int TotalDives { get; }
	}
}
