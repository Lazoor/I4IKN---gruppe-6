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
			tempCount++;
			//#####################################################

			//buffer [tempCount+1] = (byte)'\r';	// DEBUGGING REASONS TO READ ON TTYS1
			//#######################################################

			serialPort.Write (buffer, 0, tempCount);
		}


		public int receive (ref byte[] buf)
		{
			// Total size counter
			int bufReadSize = 0;
			// i = temporary buffer iterator, bufCount = buf iterator
			int i = 0, bufCount = 0;

			// ################  DEBUG  OUTCOMMENT IF NEEDED ##############
			Console.WriteLine ("Receive function started...");
			//#############################################################

			bufReadSize = serialPort.Read (buffer,i,MAXFILESIZE);
			Console.WriteLine ("Buffer received (nothing done to it): " + Encoding.ASCII.GetString (buffer));

			for (int j = 1; j < bufReadSize-1; ++j) {			// Compressing readBuf to actual size buf
				if (j < bufReadSize) {
					if (buffer [j] == 'B' && buffer [j+1] == 'C') {	// Replace 'BC' with 'A'
						buf [bufCount] = (byte)'A';
						j++;
					} else if (buffer [j] == 'B' && buffer [j+1] == 'D') { // Replace 'BD' with 'B'
						buf [bufCount] = (byte)'B';
						j++;
					} else {									// Anything else 
						buf [bufCount] = buffer [j];
					}
				} 
				bufCount++;										
			}
			return (bufReadSize);										
		}
	}
}
