using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
	class file_server
	{
		const int PORT = 9000;
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
		/// </summary>
		private file_server ()
		{
			IPAddress ipAddress = IPAddress.Parse("10.0.0.1");
			TcpListener listenerSocket = new TcpListener (ipAddress, PORT);
			Socket serverSocket = listenerSocket.Server;

			int requestCount = 0;
			bool exit = true;
			TcpClient clientSocket = default(TcpClient);
			listenerSocket.Start ();
			Console.WriteLine (" >> server startet");

			while (exit) {
				requestCount = requestCount + 1;

				Console.WriteLine (" >> Venter paa request...");
				clientSocket = listenerSocket.AcceptTcpClient ();
				Console.WriteLine (" >> Accepteret forbindelse fra klient");

				//Opretter stream
				NetworkStream clientStream = clientSocket.GetStream (); 
			
				// Opretter midlertidig placering for klientdata.
				string filePath = LIB.readTextTCP (clientStream);
				Console.WriteLine(" >> Filsti er: {0}\n", filePath);

				// Tjekker om fil eksisterer og returnerer filstørrelse fra filen.
				long fileSize = LIB.check_File_Exists (filePath);
				string fileSizeString = fileSize.ToString();

				Console.WriteLine(" >> Fil størrelse er: {0} byte(s)\n", fileSize);

				// Sender filstørrelsen ud på klientstream.
				LIB.writeTextTCP(clientStream, fileSizeString);

				sendFile(filePath, fileSize, clientStream);

			} 
			clientSocket.Close ();
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (string filePath, long fileSize, NetworkStream clientStream)
		{
			if (fileSize > 0) {

				int bytesSent = BUFSIZE, bytesRead = 0, TotalLength = Convert.ToInt32 (fileSize), bytesLeft = TotalLength;
				byte[] dataArray = new byte[BUFSIZE];
				FileStream file = File.OpenRead (filePath);

				while (bytesLeft > 0) {
					if (bytesLeft > BUFSIZE) {
						bytesRead = file.Read (dataArray, 0, BUFSIZE);
						clientStream.Write (dataArray, 0, bytesRead);
						bytesLeft -= bytesSent;
					} else {
						file.Read (dataArray, 0, bytesLeft);
						clientStream.Write (dataArray, 0, bytesLeft);
						bytesLeft -= bytesSent;
					}
				}
				Console.WriteLine (" >> Fil er sendt!\n");
			} else {
				Console.WriteLine (" >> Fil eksisterer ikke...\n");
			}
		}
			
//		/ <summary>
//		/ The entry point of the program, where the program control starts and ends.
//		/ </summary>
//		/ <param name='args'>
//		/ The command-line arguments.
//		/ </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starter...");
			new file_server();
		}
	}
}
