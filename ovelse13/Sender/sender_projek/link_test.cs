using System;
using Linklaget;
using System.Text;

namespace sender
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			Link zelda = new Link();

			Console.WriteLine ("Write somehting...");
			string read = Console.ReadLine ();

			Console.WriteLine ("Written line: " + read);

			byte[] buffer = Encoding.ASCII.GetBytes(read);

			zelda.send (buffer, buffer.Length);

			return 0;
		}
	}
}
