using System;
using System.IO;
using Linklaget;
using System.Text;

// Transportlaget
namespace Transportlaget
{
	// Transportklasse
	public class Transport
	{
		//Link object
		private Link link;
		/// The 1' complements checksum.
		private Checksum checksum;
		private byte[] buffer;
		/// The seq no.
		private byte seqNo = 0;
		/// The old_seq no.
		private byte old_seqNo;
		private int errorCount;
		private const int DEFAULT_SEQNO = 2;

		// Constructor
		public Transport (int BUFSIZE)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
		}

		/// Receives the ack.
		private bool receiveAck()
		{
			byte[] buf = new byte[(int)TransSize.ACKSIZE];
			int size = link.receive(ref buf);
			if (size != (int)TransSize.ACKSIZE) return false;
			if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) ||
			   buf[(int)TransCHKSUM.SEQNO] != seqNo ||
			   buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				return false;

			seqNo = (byte)((buf[(int)TransCHKSUM.SEQNO]+1)%2);

			return true;
		}

		// Sends the ack.
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		// Send the specified buffer and size.
		public void send(byte[] buf, int size)
		{
			int tempCount = 0;
			int j;
			for (j=0; j < size+4; j++){ // THIS MIGHT BE A PROBLEM
				if (j < 4) {
					buffer [j] = 0;
				} else {
					buffer [j] = buf [tempCount];
					tempCount = tempCount+1;
				}
			}

			buffer[2] = seqNo;
			buffer[3] = (byte)'0';

			checksum.calcChecksum (ref buffer, buffer.Length);

			link.send (buffer, buffer.Length);

			while(!receiveAck ()) {
				Console.WriteLine ("Something went wrong, trying again!");
				link.send (buffer, buffer.Length);
			} 

			Console.WriteLine ("Acknowledge received, continueing...");

			Console.WriteLine("File sent!");
	}

		// HER ER TESTEN TIL SEND..
		/* 

do {
errorCount++;
if (errorCount == 5){
	buffer[0]++;
}
link.send(buffer,buffer.Length);
if (errorCount == 5){
	buffer[0]--;
}
while(!receiveAck())

		*/

		// Receive the specified buffer.
		public int receive (ref byte[] buf)
		{
			bool ackType = false;
			link.receive (ref buffer);
			int size = buffer.Length;

			if (checksum.checkChecksum (buffer, size)) {
				for (int i = 0; i <= size-4; i++) {
					buf [i] = buffer[i+4];
				} 
				ackType = true;
			}
			sendAck (ackType);

			return 0;
		}
	}
}

