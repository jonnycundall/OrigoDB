﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveDomain.Core
{
	internal interface ICommandLog : IEnumerable<LogItem>
	{
		void Open();
		void Write(Command command);
		void Close();
		void Truncate();
	}
}