using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		private Transport myTrans;
		private const int BUFSIZE = 1000;
		private byte[] Buffer;
		private const string clientPath= "/root/Documents/I4IKN---gruppe-6/ovelse13/file_client/";

		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_client (string[] args)
		{
			Buffer = new byte[BUFSIZE];
			myTrans = new Transport (BUFSIZE);
			Console.WriteLine ("[Client] Client_server started.");
			Console.WriteLine ("[Client] Transport object created.");
			string filePath = "/root/Documents/I4IKN---gruppe-6/ovelse13/file_server/bin/Debug/hey.txt";
			while (true) {
				nextTry:
					//Console.WriteLine ("[Client] Input filepath on serverside...");
					//string filePath = Console.ReadLine ();

					int length = filePath.Length;
				fail:
					Console.WriteLine ("[Client] Try again? Y/N");
				string ch = Console.ReadLine ();

				if (ch == "y" || ch == "Y") {
					goto nextTry;
				} else if (ch == "n" || ch == "N") {
					receiveFile (filePath, myTrans);
				} else {
					goto fail;
				}
				Console.WriteLine ("[Client] File Received! \n[Client] Starting over...");
			}

		}

		private void receiveFile(String filePath, Transport transport)
		{

			// Getting filename for placement in client.
			string fileName =	LIB.extractFileName(filePath);
			string altPath = clientPath + fileName;

			// Sending to server
			int length = filePath.Length;
			byte[] fileMan = new byte[length];
			fileMan = Encoding.ASCII.GetBytes (filePath);

			myTrans.send (fileMan, fileMan.Length);

			Array.Clear (fileMan, 0, fileMan.Length);

			myTrans.receive (ref Buffer);
			int fileSize = BitConverter.ToInt32 (Buffer,0);
			int bytesReceived = 0;
			int bytesLeft = fileSize;
			int bytesWriten = 0;
			FileStream file =  new FileStream(altPath, FileMode.Create, FileAccess.ReadWrite);

			while (bytesLeft > 0) {
				Array.Clear (Buffer, 0, Buffer.Length);
				myTrans.receive (ref Buffer);
				string test = Encoding.ASCII.GetString (Buffer);
				Console.WriteLine ("[Client] Writing:" + test);
				bytesReceived += Buffer.Length;
				file.Write (Buffer, bytesWriten, bytesReceived); 
				bytesWriten += bytesReceived;
				bytesLeft -= bytesReceived;

			}
			file.Close ();
			Console.WriteLine ("File is received!");
		}


		public static void Main (string[] args)
		{
			new file_client(args);
		}
	}
}