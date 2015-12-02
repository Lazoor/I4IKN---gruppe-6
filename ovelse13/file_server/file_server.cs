using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		private Transport myTrans;
		private const int BUFSIZE = 1000;
		private byte[] receiveBuffer;
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			receiveBuffer = new byte[BUFSIZE];
			myTrans = new Transport (BUFSIZE);
			Console.WriteLine ("File_server started.");
			Console.WriteLine ("Transport object created.");

			while (true) {

				Console.WriteLine ("Waiting to receive filename.");

				myTrans.receive (ref receiveBuffer);

				string fileName, filepath;

				filepath = Encoding.ASCII.GetString (receiveBuffer);
				fileName = LIB.extractFileName (filepath);

				Console.WriteLine ("Filename received: ");
				Console.WriteLine (fileName);
				Console.WriteLine (filepath);

				long fileSize;

				fileSize = LIB.check_File_Exists (fileName);

				if (fileSize != 0) {
					Console.WriteLine ("Sending file!");
					sendFile (fileName, fileSize, myTrans);
				} else {
					Console.WriteLine ("Something wrong.");
				}
				Console.WriteLine ("Starting over...");
			}
		}

		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			if (fileSize > 0) {

				int bytesSent = BUFSIZE;
				int bytesRead = 0;
				int bytesLeft = Convert.ToInt32 (fileSize);
				byte[] dataArray = new byte[BUFSIZE];

				if (LIB.check_File_Exists (fileName) != 0) {
					FileStream file = File.OpenRead (fileName);

					while (bytesLeft > 0) {
						if (bytesLeft > BUFSIZE) {
							bytesRead = file.Read (dataArray, bytesRead, BUFSIZE);
							myTrans.send (dataArray, bytesRead);
							bytesLeft -= bytesSent;
						} else {
							file.Read (dataArray, bytesRead, bytesLeft);
							myTrans.send (dataArray, bytesLeft);
							bytesLeft -= bytesLeft;
						}
					}
					Console.WriteLine (" >> Fil er sendt!\n");
				} else {
					string fail = "0";
					Console.Write (fail);
					sendFile (fail, fail.Length, myTrans);
					Console.WriteLine ("File doesn't exist.");
				}
			}
		}

		public static void Main (string[] args)
		{
			new file_server();
		}
	}
}
