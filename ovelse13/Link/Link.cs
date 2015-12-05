using System;
using System.IO.Ports;
using System.Text;

namespace Linklaget
{
	public class Link
	{
		const byte DELIMITER = (byte)'A';
		private byte[] buffer;
		SerialPort serialPort;


		public Link (int BUFSIZE)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)+2];
		}

		public void send (byte[] inputbuf, int size)
		{
			Array.Clear (buffer, 0, buffer.Length);
			int tempCount = 1;

			string testStr = Encoding.ASCII.GetString (inputbuf);
			Console.WriteLine ("[Link] Send method received: " + inputbuf);

			for (int i=0; i < size; ++i) {
				if (inputbuf [i] == (byte)'A') {
					buffer [tempCount] = (byte)'B';
					buffer [tempCount+1] = (byte)'C';
					tempCount += 2;
				} else if (inputbuf [i] == (byte)'B') {
					buffer [tempCount] = (byte)'B';
					buffer [tempCount+1] = (byte)'D';
					tempCount += 2;
				} else {
					buffer [tempCount] = inputbuf [i];
					tempCount++;
				} 
			}
			buffer [0] = DELIMITER;
			buffer [tempCount] = DELIMITER;
			tempCount++;

			testStr = Encoding.ASCII.GetString (buffer);
			Console.WriteLine ("[Link] Sending: " + testStr);
			//#####################################################

			//buffer [tempCount+1] = (byte)'\r';	// DEBUGGING REASONS TO READ ON TTYS1
			//#######################################################

			serialPort.Write (buffer, 0, tempCount);
		}


		public int receive (ref byte[] buf)
		{
			Array.Clear (buffer, 0, buffer.Length);
			// Total size counter
			int bufReadSize = 0;
			int byteRead = 0;
			// i = temporary buffer iterator, bufCount = buf iterator
			int bufCount = 0;
			bool exit = false;

			// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
			Console.WriteLine ("[Link] Receive started.");
			//#############################################################

			while (!exit){
				byteRead = serialPort.ReadByte ();

				if (byteRead == (byte)'A' && bufReadSize > 1) {
					exit = true;
				} else if (byteRead < 0) {
					exit = true;
				} else {
					buffer [bufReadSize] = (byte)byteRead;
					bufReadSize++;
				}
			}

			byte[] temp = new byte[bufReadSize];

			for (int i = 0; i < bufReadSize; i++) {
				temp [i] = buffer [i];
			}


			string testStr = Encoding.ASCII.GetString (temp);
			Console.WriteLine ("[Link] Received from /dev/ttyS1: " + testStr);

			//DEBUG ###############################
			//Console.WriteLine ("Buffer received (nothing done to it): " + Encoding.ASCII.GetString (buffer));
			//#################################
			for (int j = 1; j < bufReadSize; j++){
				if (j < bufReadSize) {
					if (temp[j] == (byte)'B' && temp [j+1] == (byte)'C') {	// Replace 'BC' with 'A'
						buf [bufCount] = (byte)'A';
						j++;
					} else if (buffer [j] == (byte)'B' && temp [j+1] == (byte)'D') { // Replace 'BD' with 'B'
						buf [bufCount] = (byte)'B';
						j++;
					} else {									// Anything else 
						buf [bufCount] = temp [j];
					}
				} 
				bufCount++;										
			}
			testStr = Encoding.ASCII.GetString (buf);
			Console.WriteLine ("[Link] Translated: " + testStr);
			return (bufCount);
		}
	}
}
