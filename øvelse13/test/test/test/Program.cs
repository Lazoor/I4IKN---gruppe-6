using System;
using System.IO;
using System.Text;
using Transportlaget;
using Linklaget;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			Link myLink = new Link (BUFSIZE);
			Console.WriteLine ("Link object created.");
			Transport myTrans = new Transport (BUFSIZE);
			Console.WriteLine ("Transport object created.");

			while (true) {
				Console.WriteLine ("Input some text: ");
				string input = Console.ReadLine ();
				byte[] A = new byte[3];

				// Filling up sendBuffer
				A [0] = (byte)input [0];
				A [1] = (byte)input [1];
				A [2] = (byte)input [2];

				myTrans.send (A, A.Length);

				Console.WriteLine ("Output: ");
				Console.WriteLine (A.ToString ());
			}
		}

		//private void sendFile(String fileName, long fileSize, Transport transport)
		//{

		//}

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

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			new file_server();
		}
	}
}
