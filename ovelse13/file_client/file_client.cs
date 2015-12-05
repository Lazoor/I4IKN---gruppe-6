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
		private const string clientPath= "/root/i4ikn/I4IKN---gruppe-6/øvelse13/file_client/received/";

		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_client ()
		{
			Buffer = new byte[BUFSIZE];
			myTrans = new Transport (BUFSIZE);
			Console.WriteLine ("Client_server started.");
			Console.WriteLine ("Transport object created.");

			while (true) {
				Console.WriteLine ("Write filepath...");
				string fileName = Console.ReadLine ();

				myTrans.send (fileName, fileName.Length);

				if 

				receiveFile (fileName, myTrans);

				Console.WriteLine ("File Received! \n Starting over...");
			}

		}

		private void receiveFile(String filePath, Transport transport)
		{
			byte[] dataReceive = new byte[BUFSIZE];
			string fileName =	LIB.extractFileName(filePath);
			string altPath = clientPath + fileName;
			dataReceive = Encoding.ASCII.GetBytes (fileName);

			myTrans.send (dataReceive, dataReceive.Length);

			myTrans.receive (ref Buffer);

			if (Encoding.ASCII.GetString (Buffer) != "0") {
				int bytesReceived = 0;
				int bytesLeft = Convert.ToInt32(Buffer.Length);

				FileStream file = File.OpenWrite(altPath);

				while (bytesLeft > 0) {
					bytesReceived += myTrans.receive (ref Buffer);
					file.Write (Buffer, bytesReceived, bytesReceived);
					bytesLeft -= bytesReceived;
				}

				Console.WriteLine ("File is received!");

			} else {
				Console.WriteLine ("File doesn't exist...");	
			}
		}


		public static void Main (string[] args)
		{
			new file_client();
		}
	}
}
