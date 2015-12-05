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
			Console.WriteLine ("[FileServer] Server started.");
			Console.WriteLine ("[FileServer] Transport object created.");

			while (true) {
				bool failFlag = false;
				Console.WriteLine ("[FileServer] Waiting to receive filename.");

				myTrans.receive (ref receiveBuffer);


				if (failFlag == false) {
					string temp = Encoding.ASCII.GetString(receiveBuffer);
					string filepath = "";

					for (int i = 0; i < temp.Length; i++) {
						char ch = temp[i];
						if (ch != '\0') {
							filepath += ch;
						}
					}

					Console.WriteLine ("[FileServer] Filepath received: ");
					Console.WriteLine (filepath);

					long fileSize;

					fileSize = LIB.check_File_Exists (filepath);

					if (fileSize > 0) {
						Console.WriteLine ("[FileServer] Sending file!");
						sendFile (filepath, fileSize, myTrans);
					} else {
						receiveBuffer = BitConverter.GetBytes(fileSize);
						myTrans.send (receiveBuffer, receiveBuffer.Length);
						Console.WriteLine ("[FileServer] File doesn't exist.");
					}
				}
				Console.WriteLine ("[FileServer] Starting over.");
			}
		}

		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			if (fileSize > 0) {

				int bytesSent = 0;
				int bytesRead = 0;
				int bytesLeft = Convert.ToInt32 (fileSize);
				byte[] dataArray = new byte[BUFSIZE];
				byte[] fileSizeByte = new byte[10];

				FileStream file = File.OpenRead (fileName);
				Console.WriteLine ("[FileServer] Opened file: " + fileName);

				fileSizeByte = BitConverter.GetBytes(fileSize);
				myTrans.send (fileSizeByte, fileSizeByte.Length);

				while (bytesLeft > 0) {
					if (bytesLeft > BUFSIZE) {
						bytesRead += file.Read (dataArray, bytesSent, BUFSIZE);
						myTrans.send (dataArray, bytesRead);
						bytesLeft -= bytesRead;
						bytesSent += bytesRead;
					} else {
						bytesRead += file.Read (dataArray, bytesRead, bytesLeft);	// EXEPTION POPS HERE???
						myTrans.send (dataArray, bytesRead);
						bytesLeft -= bytesLeft;
					}
				}
				Console.WriteLine ("[FileServer] File sent.");
			}
		}

		public static void Main (string[] args)
		{
			new file_server();
		}
	}
}
