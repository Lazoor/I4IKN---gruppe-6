using System;
using Linklaget;
using Transportlaget;
using System.IO;
using System.Text;

namespace main
{
	public class test_main
	{
		public test_main ()
		{
			int BUFSIZE = 1000;
			Transport myTrans = new Transport (BUFSIZE);
		
			Console.WriteLine ("Write some letters. ");
			string test = Console.ReadLine ();
			int length = test.Length;
			Console.Write (test, test.Length);
			byte[] A = new byte[3];

			for (int i = 0; i < 3; i++) {
				A [i] = (byte)'A';
			}

			myTrans.send (A,A.Length);

			Console.WriteLine ("Output: ");
			Console.WriteLine (A.ToString());
		}

		/*
while (bytesLeft != 0) {
				if (bytesLeft > BUFSIZE) {
					for (int i = 4; i <= BUFSIZE; i++) {
						buffer [i] = buf [bytesRead];
						bytesRead++;
					}
					bytesLeft -= BUFSIZE;
				} else {
					for (int i = 4; i <= bytesLeft; i++) {
						buffer[i] = buf [bytesRead];
						bytesRead++;
					}
					bytesLeft -= bytesLeft;
				}
		*/

		public static void Main (string[] args)
		{
			new test_main ();
		}
	}
}
