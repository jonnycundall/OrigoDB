﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace LiveDomain.Core
{

    /// <summary>
    /// A writer that waits for the disk write to complete before returning. Slower but reliable.
    /// </summary>
	internal class SynchronousJournalWriter : JournalWriter, IJournalWriter
	{
        public SynchronousJournalWriter(Stream stream, Serializer serializer)
        {
            _serializer = serializer;
            _stream = stream;
        }
		public void Write(JournalEntry item)
		{
            _serializer.Write(item, _stream);
            _stream.Flush(); //TODO: What if the file becomes corrupt and half of the command is written? Deserialization will crash.
		}

		public void Close()
		{
			if(_stream.CanRead || _stream.CanWrite)
				_stream.Close();
		}

    	void IDisposable.Dispose()
    	{
			Close();
    	}
	}
}