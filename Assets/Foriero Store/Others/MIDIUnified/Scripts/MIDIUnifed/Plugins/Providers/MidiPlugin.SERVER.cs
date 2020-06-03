/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/
#if UDP_SERVER

using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace ForieroEngine.MIDIUnified.Plugins
{

	
	public static class UdpServer{
		static Socket server;
		static IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
		static EndPoint epServer = new IPEndPoint(IPAddress.Any,0);
		static byte[] data = new byte[1024];
		static string dataStr = "";
		static public ArrayList messageList = new ArrayList();
		
		static public  void Create(){
			if(server == null){
	//if UNITY_WEBPLAYER
				if(Security.PrefetchSocketPolicy(IPAddress.Loopback.ToString(),7777)){
	
				}
	//endif
				server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				//server.Bind(epServer);
				SendMessage("REGISTERME");
				data = new byte[1024];
	  			server.BeginReceiveFrom (data, 0, data.Length, SocketFlags.None, ref epServer, new AsyncCallback(OnReceive), null);
			}
		}
		
		static public void Destroy(){
			if(server != null){
				server.Close();
			}
		}
		
		static public void SendMessage(string aMessage){
			byte[] sendData = Encoding.ASCII.GetBytes(aMessage);
	        //server.BeginSendTo(sendData,0, sendData.Length, SocketFlags.None, ipep, new AsyncCallback(OnSend), null);
			server.SendTo(sendData,0, sendData.Length, SocketFlags.None, ipep);
		}
		
		static void OnSend(IAsyncResult ar){
			server.EndSendTo(ar);	
		}
		
		static void OnReceive(IAsyncResult ar) {            
	        try
	        {                
	            server.EndReceiveFrom(ar, ref epServer);
				if(data.Length > 0){
					dataStr = Encoding.UTF8.GetString(data);
					messageList.Add(dataStr);
				}
				data = new byte[1024];
	  			server.BeginReceiveFrom (data, 0, data.Length, SocketFlags.None, ref epServer, new AsyncCallback(OnReceive), null);
	        }
	        catch (Exception ex)
	        {
	            Debug.Log(ex.Message);
	        }
	    }
	}
	
	public static class MidiInPlugin {

		public static List<string> deviceNames = new List<string>();
		public static Queue<MidiMessage> midiMessages = new Queue<MidiMessage>(100);

		public static string ExceptionStr;
	
		public static int Create(){
			int result = 0;
			UdpServer.Create();
			result = 1;
			return result;
		}
		
		public static int Dispose(){
			int result = 0;
			UdpServer.Destroy();
			result = 1;
	 		return result;
		}
		
		public static int ConnectDevice(int aDeviceIndex){
			int result = 0;
			result = 1;
			return result;
		}
		
		public static int DisconnectDevice(){
			int result = 0;
			result = 1;
			return result;
		}
						
		public static string GetDeviceName(int aDeviceIndex){
			string result = "";
			
			return result;	
		}
		
		public static int GetDeviceCount(){
			int result = 0;
			return result;	
		}
		
		public static int PopMessage(out MidiMessage aMidiMessage){
		 	int result = 0;
			aMidiMessage = new MidiMessage();
			if(UdpServer.messageList.Count > 0){
				//Debug.Log(((string)UdpServer.messageList[0]));
				string[] values = ((string)UdpServer.messageList[0]).Split(";".ToCharArray());
				aMidiMessage.m_Command = byte.Parse(values[1]);
				aMidiMessage.m_Data0 = byte.Parse(values[2]);
				aMidiMessage.m_Data1 = byte.Parse(values[3]);
				UdpServer.messageList.RemoveAt(0);
				result = 1;
			}
			return result;
		}
		
		
		
	}
	
	public static class MidiOutPlugin{

		public static List<string> deviceNames = new List<string>();

		static UdpClient u;
				
		public static int Init(){
			int result = 0;
			UdpServer.Create();
			result = 1;
			return result;
		}
		
		public static int Close(){
			int result = 0;
			result = 1;
			UdpServer.Destroy();
			return result;
		}
		
		public static int ConnectDevice(int aDeviceIndex){
			int result = 0;
			result = 1;
			return result;
		}
		
		public static int DisconnectDevice(){
			int result = 0;
			result = 1;
			return result;
		}
				
		public static string GetDeviceName(int aDeviceIndex){
			string result = "GENERAL MIDI";
			return result;	
		}
		
		public static int GetDeviceCount(){
			int result = 1;	
			return result;	
		}
		
		public static int SendMessage(uint aByte1, uint aByte2, uint aByte3){
			UdpServer.SendMessage("MIDI;" + aByte1.ToString() + ";" + aByte2.ToString() + ";" + aByte3.ToString());		
			return 1;
		}
	}

}
#endif	
