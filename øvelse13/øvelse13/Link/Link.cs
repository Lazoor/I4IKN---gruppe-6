using System;
using System.IO.Ports;

namespace Linklaget
{
	public class Link
	{
		const int MAXFILESIZE = 1000;
		const byte DELIMITER = (byte)'A';
		private byte[] buffer;
		SerialPort serialPort;


		public Link (int MAXFILESIZE)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(MAXFILESIZE*2)+2];
		}

		public void send (byte[] buf, int size)
		{
			byte[] temp;
			int tempCount = 0;

			for (int i=0; i == size; i++) {
				if (i > 0 && i < size) {
					if (buf [i] == 'A') {
						temp [tempCount] = (byte)'B';
						temp [tempCount + 1] = (byte)'C';
						tempCount++;
					} else if (buf [i] == 'B') {
						temp [bufCount] = (byte)'B';
						temp [bufCount + 1] = (byte)'D';
						tempCount++;
					} else {
						temp [tempCount] = buf [i];
					}
				} else {
					temp [tempCount] = DELIMITER;
				}
				tempCount++;
			}
			serialPort.Write (buf, 0, size);
		}

		public int receive (ref byte[] buf)
		{
			bool wait = false;
			// Total size counter
			int bufReadSize = 0;
			// i = temporary buffer iterator, bufCount = buf iterator
			int i = 0, bufCount = 0;
			byte[] temp;

			// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
			Console.WriteLine ("Receive function started...");
			//#############################################################

			// Receive while-loop
			while (wait != true) {								
				// serialPort.Read(buffer, offset, maxSizeBuffer)
				bufReadSize += serialPort.Read (temp[i], 0, 1); 

				// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
				Console.WriteLine ("Reading directly from link-layer:\n");
				Console.Write (temp [i]);
				//#############################################################

				// If not buf[0] == 'A' -> exit
				if (temp [0] != DELIMITER) {					
					Console.Error.WriteLine ("Forkert frame....!");
					return 1;
				} 
				// If buf[i] == 'A' -> reading done...
				if (i > 0 && buf [i] == DELIMITER) {			
					Console.WriteLine ("Reading done...");
					// exit while.
					wait = true;	
				} else if (i > MAXFILESIZE){
					// Timed out.
					return 2;									
				}
				// Incrementing for each byte read.
				i++;											
			}

			for (int j = 0; j < bufReadSize; ++j) {
				if (j > 0 && j < bufReadSize) {
					if (temp [j] == 'B' && temp [j+1] == 'C') {	// Replace 'BC' with 'A'
						buf [bufCount] = 'A';
						j++;
					} else if (temp [j] == 'B' && temp [j+1] == 'D') { // Replace 'BD' with 'B'
						buf [bufCount] = 'B';
						j++;
					} else {									// Anything else 
						buf [bufCount] = temp [j];
					}
				} else {										// Frame handling.
					buf [j] = temp [j];
				}
				bufCount++;										// Compressing readBuf to actual size buf
			}
			return 0;										
		}
	}
}
