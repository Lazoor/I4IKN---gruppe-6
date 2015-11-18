using System;
using System.Text;
using System.IO;

namespace send
{
	class MainClass
	{
		const byte DELIMITER = (byte)'A';
		public static byte[] buffer;


		public static void Main (string[] args)
		{
			buffer = new byte[(1*2)+2];
			buffer = Encoding.ASCII.GetBytes (Console.ReadLine ());
			send (buffer, buffer.Length);
			Console.ReadKey ();
		}

		public static void send (byte[] buf, int size)
		{
			int bufSize = size+2;
			bool ABflag = false;
			int AB = 0;
			for (int i=0; i < size; i++) {
				if (buf [i] == DELIMITER) {
					bufSize=bufSize+2;
				} else { 
					if (buf [i] == (byte)'B') {
						bufSize=bufSize+2;
					}
				}

			}
			Console.WriteLine ("bufsize " + bufSize);
			byte[] sendBuffer = new byte[bufSize];
			sendBuffer[0] = DELIMITER;
			if (buf [0] == DELIMITER) {
				sendBuffer [1] = (byte)'B';
				sendBuffer [2] = (byte)'C';
				ABflag = true;
				Console.WriteLine ("buf0a " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
			} else {
				if (buf [0] == (byte)'B') {
					sendBuffer [1] = (byte)'B';
					sendBuffer [2] = (byte)'D';
					ABflag = true;
					Console.WriteLine ("buf0b " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
				} else {
					sendBuffer [1] = buf [0];
					Console.WriteLine ("buf0 " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
				}
			}

			for (int i =0; i < size; i++) {
				if (ABflag==true) {
					AB=2;
					i++;
					ABflag = false;
				}
				if (buf [i] == DELIMITER) {
					sendBuffer [1+i+AB] = (byte)'B';
					AB++;
					sendBuffer [1+i+AB] = (byte)'C';
					AB++;
					Console.WriteLine("bufia "+Encoding.ASCII.GetString(sendBuffer,0, bufSize));
				} 
				else { 
					if (buf [i] == (byte)'B') {
						sendBuffer [1 + i + AB] = (byte)'B';
						AB++;
						sendBuffer [1 + i + AB] = (byte)'D';
						AB++;
						Console.WriteLine ("bufib " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
					} else {
						sendBuffer [1 + i + AB] = buf [i];
						Console.WriteLine ("bufi " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
					}
				}
			}
			sendBuffer[bufSize-1] = DELIMITER; 
			Console.WriteLine(Encoding.ASCII.GetString(sendBuffer,0, bufSize));
			//	// TO DO Your own code
		}
	}
}
