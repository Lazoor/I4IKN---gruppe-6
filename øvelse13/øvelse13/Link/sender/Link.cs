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

		/*		public void send (byte[] buf, int size)
		{
			byte[] temp = new byte[10001];
			int tempCount = 0;

			for (int i=0; i < size; ++i) {
				if (i > 0 && i < size) {
					if (buf [i] == (byte)'A') {
						temp [tempCount] = (byte)'B';
						temp [tempCount + 1] = (byte)'C';
					} else if (buf [i] == (byte)'B') {
						temp [tempCount] = (byte)'B';
						temp [tempCount + 1] = (byte)'D';
					} else {
						temp [tempCount] = buf [i];
					}
				} else {
					temp [tempCount] = DELIMITER;
				}
				++tempCount;
			}
			temp [tempCount] = (byte)'\n';
			serialPort.Write (temp, 0, tempCount);
		}
		*/

		public void send (byte[] buf, int size)
		{
			//Incrementing bufSize because of wrapping
			int bufSize = size+2;		
			bool ABflag = false;
			int AB = 0;

			// If 'A' present, bufSize inc with 2 'A' = 'BC'
			for (int i = 0; i < size; i++) {	

				if (buf [i] == DELIMITER) {
					bufSize = bufSize + 2;
				} else if (buf [i] == (byte)'B') {	
					bufSize = bufSize + 2;
				} // In any case other then AB, bufSize is already the same.
			}	
		

			//################## DEBUG #######################
			Console.WriteLine ("bufsize " + bufSize);
			//#################################################

			byte[] sendBuffer = new byte[bufSize+1];  // +1 is only for DEBUG
			sendBuffer[0] = DELIMITER;   
			if (buf [0] == DELIMITER) {  
				sendBuffer [1] = (byte)'B'; 
				sendBuffer [2] = (byte)'C'; 
				ABflag = true; 

				//################## DEBUG #######################
				Console.WriteLine ("buf0a " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize)); 
				//#################################################

			} else if (buf [0] == (byte)'B') { 
				sendBuffer [1] = (byte)'B';
				sendBuffer [2] = (byte)'D';
				ABflag = true; 

				//################## DEBUG #######################
				Console.WriteLine ("buf0b " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
				//#################################################
			} else { 
				sendBuffer [1] = buf [0];
				//################## DEBUG #######################
				Console.WriteLine ("buf0 " + Encoding.ASCII.GetString (sendBuffer, 0, bufSize));
				//#################################################
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
			//sendBuffer[bufSize] = (byte)'\r';  // DEBUG, FJERNES UDEN FOR TEST
			serialPort.Write(Encoding.ASCII.GetString(sendBuffer,0, bufSize+1));
			//	// Jeppes kode
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
