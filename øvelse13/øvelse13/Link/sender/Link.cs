using System;
using System.IO.Ports;
using System.Text;

namespace Linklaget
{
	public class Link
	{
		const int MAXFILESIZE = 1000;
		const byte DELIMITER = (byte)'A';
		private byte[] buffer;
		SerialPort serialPort;


		public Link ()
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(MAXFILESIZE*2)+2];
		}

		public void send (byte[] inputbuf, int size)
		{
			int tempCount = 1;

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
			tempCount++;
			buffer [tempCount] = DELIMITER;

			//#####################################################
			tempCount++;
			buffer [tempCount+1] = (byte)'\r';	// DEBUGGING REASONS TO READ ON TTYS1
			//#######################################################
			serialPort.Write (Encoding.ASCII.GetString(buffer, 0, tempCount));
		}


		public int receive (ref byte[] buf)
		{
			bool wait = false;
			// Total size counter
			int bufReadSize = 0;
			// i = temporary buffer iterator, bufCount = buf iterator
			int i = 0, bufCount = 0;
			byte[] temp = new byte[100000];

			// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
			Console.WriteLine ("Receive function started...");
			//#############################################################



			// Receive while-loop
			while (wait != true) {								
				// serialPort.Read(buffer, offset, maxSizeRead)
				bufReadSize += serialPort.Read(temp,i,1); 

				// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
				//Console.WriteLine ("Reading directly from link-layer:\n");
				Console.WriteLine (temp[i]);
				//#############################################################

				// If not buf[0] == 'A' -> exit
				if (temp [0] != DELIMITER) {					
					Console.Error.WriteLine ("Forkert frame....!");
					return 1;
				} 
				// If buf[i] == 'A' -> reading done...
				if ((i > 0) && (buf [i] == DELIMITER)) {			
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
						buf [bufCount] = (byte)'A';
						++j;
					} else if (temp [j] == 'B' && temp [j+1] == 'D') { // Replace 'BD' with 'B'
						buf [bufCount] = (byte)'B';
						j++;
					} else {									// Anything else 
						buf [bufCount] = temp [j];
					}
				} 
				bufCount++;										// Compressing readBuf to actual size buf
			}
			return (bufCount-1);										
		}
	}
}
