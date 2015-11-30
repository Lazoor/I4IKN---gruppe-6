using System;
using Linklaget;
using Transportlaget;
using System.IO;

namespace main
{
	public class test_main
	{
		public test_main ()
		{
			Link myLink;
			Transport myTrans;

			byte A = new byte[3];
			A[0] = (byte)'A';
			A[1] = (byte)'B';
			A[2] = (byte)'C';

			myTrans.send (A, 3);
		}
	}
}

