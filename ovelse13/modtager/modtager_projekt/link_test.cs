using System;
using System.IO.Ports;
using System.Text;
using Linklaget;

namespace modtager
{
	class MainClass
	{
		public static int Main ()
		{
			Link modtager = new Link ();

			byte[] buffer = new byte[100001];

			int size = modtager.receive (ref buffer);

			Console.WriteLine("buffer containing: " + Encoding.ASCII.GetString(buffer,0, size));

			return 0;
		}
	}
}
