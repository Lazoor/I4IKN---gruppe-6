using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

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
			Console.WriteLine ("Link object created.");
			Console.WriteLine ("Input some text: ");
			Console.ReadLine ();
			Transport myTrans = new Transport (BUFSIZE);
			byte[] A = new byte[3];

			// Filling up sendBuffer
			A [0] = (byte)'A';
			A [1] = (byte)'B';
			A [2] = (byte)'C';

			for (int i = 0; i < 10; i++) {
				myTrans.send (A, A.Length);
				Console.WriteLine ("Output: ");
				Console.WriteLine (Encoding.ASCII.GetString(A));
			}


		}

		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			/*Transport myTrans = new Transport (BUFSIZE);
			Console.WriteLine ("Transport object created.");*/
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
