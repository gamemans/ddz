using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class MsgManager : MonoBehaviour
{/*
	引用C 、C++中的方法
	[DllImport ("xuanyusong")]
	private static extern int addInt(int a,int b);
	[DllImport ("xuanyusong")]
	private static extern float addFloat(float a,float b);	
	*/



		[SerializeField, SetProperty("Number")]
		private float
				number;

		public float Number {
				get {
						return number;
				}
				private set {
						number = Mathf.Clamp01 (value);
				}
		}



		//===============================
//	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
//	public	struct PT_ENTER_GAME_REQUEST_INFO
//	{
//
//		public	int gameid;
//	  	public	int userid;
//		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
//		public string charname;
//		public	Int64 guid;
//
//	};
		//============================================


		/*
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	struct TGSUpdateLadder
	{
		public THeader h;
		public byte charlevel;
		public uint charexplow;
		public uint charexphigh;
		public byte charclass;
		public ushort charstatus;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string charname;
	}

*/


		public Int64 socketindex;


		public delegate int CSCallback (int Id,IntPtr data,int len,Int64 sockindex);

		string tstmsg = "";
		int i = 0;
		int j = 0;
		/// <summary>
		/// 这个属性很重要, 不要的话IOS报错, 回调方法都要加
		/// </summary>
		/// <returns>The callback function.</returns>
		/// <param name="msg">Message.</param>
		[AOT.MonoPInvokeCallbackAttribute(typeof(CSCallback))]
		int OnConnectCallFunc (int Id, IntPtr data, int len, Int64 sockindex)
		{
				//	GUILayout.Label ("=====" + msg);
				//msg1 = msg;

//		readPaket.IgnoreBytes(1);
//		readPaket.Read(nServerId);
//		readPaket.Read(nGameId);
//		readPaket.Read(nUserId);
//		readPaket.Read(nUserGuid);

				Debug.Log (string.Format ("======{0}", Id));

				switch (Id) {
				case (int)msg_id.ID_CONNECTION_REQUEST_ACCEPTED:
						{
								Debug.Log (string.Format ("==ID_CONNECTION_REQUEST_ACCEPTED===={0}", Id));
								socketindex = sockindex;
			
								SendEntergame (101, 2, 100891);
						}
						break;
		
				case (int)msg_id.PT_HOST_MESSAGE:
						{
								Debug.Log (string.Format ("==PT_HOST_MESSAGE===={0}", Id));

								int hostId = Marshal.ReadInt32 (data);
								Debug.Log (string.Format ("==host id===={0}", hostId));
								switch (hostId) {
								case (int)host_msg.PT_MJ_MATCH_ACCEPT:
										{

			
										}
									break;
								}
				
						}
						break;

				case (int)msg_id.PT_ENTER_GAME_ACCEPT:
						{

								Debug.Log (string.Format ("==PT_ENTER_GAME_ACCEPT===={0}", System.Runtime.InteropServices.Marshal.SizeOf (typeof(PT_ENTER_GAME_ACCEPT_INFO))));

								PT_ENTER_GAME_ACCEPT_INFO info1;
								//	byte[] by1 = new byte[len];

				
								//	Marshal.Copy ((IntPtr)data, by1, 0, len); 

								//跳过第一个字节,255
								info1 = (PT_ENTER_GAME_ACCEPT_INFO)(PtrToStruct (data, typeof(PT_ENTER_GAME_ACCEPT_INFO)));


								int t = 0;

								Debug.Log (string.Format ("==PT_ENTER_GAME_ACCEPT==11=={0}", info1.id));

						}
						break;

				default:
						{
								Debug.Log (string.Format ("==default===={0}", Id));
						}
						break;
						
				}

				/*
				if (Id == 16) {

				}

				if (Id == 255) {
//						PT_ENTER_GAME_REQUEST_INFO info1;
//
//						byte[] by1 = new byte[len];
//
//	
//						Marshal.Copy ((IntPtr)data, by1, 0, len); 
//
//						//跳过第一个字节,255
//		
//						info1 = (PT_ENTER_GAME_REQUEST_INFO)(BytesToStruct (by1, 1, len - 1, typeof(PT_ENTER_GAME_REQUEST_INFO)));
//
//						int gg = 0;
//
//						gg++;





			
				}
				*/
			
				return 1;
		}

		CSCallback OnConnectCall;

		[DllImport ("RakNet")]
		public static extern int Connect (string host, int remotePort, string passwordData, int passwordDataLength);
	
		[DllImport ("RakNet")]
		extern static int RegMsg (int msgid, CSCallback callback);
		
		[DllImport ("RakNet")]
		private static extern void MsgLoop ();

		[DllImport ("RakNet")]
		private static extern int MsgInit ();

		[DllImport ("RakNet")]
		private static extern int SendEx (int msgid, int msgid2, byte[] data, int len, Int64 sockindex);

		[DllImport ("RakNet")]
		private static extern int SendEx_1id (int msgid, byte[] data, int len, Int64 sockindex);

		public static byte[] StructToBytes (object structObj, int size)
		{
				byte[] bytes = new byte[size];
				IntPtr structPtr = Marshal.AllocHGlobal (size);
				//将结构体拷到分配好的内存空间
				Marshal.StructureToPtr (structObj, structPtr, false);
				//从内存空间拷贝到byte 数组
				Marshal.Copy (structPtr, bytes, 0, size);
				//释放内存空间
				Marshal.FreeHGlobal (structPtr);
				return bytes;
		}


		//=======================================

		public byte[] StructToBytes (object obj)
		{
				int rawsize = Marshal.SizeOf (obj);
				IntPtr buffer = Marshal.AllocHGlobal (rawsize);
				Marshal.StructureToPtr (obj, buffer, false);
				byte[] rawdatas = new byte[rawsize];
				Marshal.Copy (buffer, rawdatas, 0, rawsize);
				Marshal.FreeHGlobal (buffer);
				return rawdatas;
		}

		public object BytesToStruct (byte[] buf, int bgindex, int len, Type type)
		{
				object rtn;
				IntPtr buffer = Marshal.AllocHGlobal (len);
				Marshal.Copy (buf, bgindex, buffer, len);
				rtn = Marshal.PtrToStructure (buffer, type);
				Marshal.FreeHGlobal (buffer);
				return rtn;
		}

		public object PtrToStruct (IntPtr ptr, Type type)
		{
				object rtn;

				rtn = Marshal.PtrToStructure (ptr, type);

				return rtn;
		}

		public object BytesToStruct (byte[] buf, int len, Type type)
		{
				object rtn;
				IntPtr buffer = Marshal.AllocHGlobal (len);
				Marshal.Copy (buf, 0, buffer, len);
				rtn = Marshal.PtrToStructure (buffer, type);
				Marshal.FreeHGlobal (buffer);
				return rtn;
		}
	
		public void BytesToStruct (byte[] buf, int len, object rtn)
		{
				IntPtr buffer = Marshal.AllocHGlobal (len);
				Marshal.Copy (buf, 0, buffer, len);
				Marshal.PtrToStructure (buffer, rtn);
				Marshal.FreeHGlobal (buffer);
		}
	
		public void BytesToStruct (byte[] buf, object rtn)
		{
				BytesToStruct (buf, buf.Length, rtn);
		}
	
		public object BytesToStruct (byte[] buf, Type type)
		{
				return BytesToStruct (buf, buf.Length, type);
		}

		//========================================

		// Use this for initialization
		void Start ()
		{



		
				OnConnectCall = OnConnectCallFunc;
	
				j = MsgInit ();

	
				if (j != 0)
						return;
				Debug.Log ("=====msgmanager--=-==-");


				///注册连接成功消息
				RegMsg (16, OnConnectCall);

				RegMsg (255, OnConnectCall);

				RegMsg (141, OnConnectCall);
				Debug.Log ("=====msgmanager-1-=-==-");

	
				int ret = Connect ("192.168.1.108", 61000, "", 0);

				i = ret;

				return;
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void FixedUpdate ()
		{
				//真机打开,可看测试效果
				//	return;

				MsgLoop ();
		}

		void OnGUI ()
		{
				i = (int)Number;
				GUILayout.Label ("call back i=" + i);
				GUILayout.Label ("call back j=" + j);
		}
		/**  
    * 将int数值转换为占四个字节的byte数组，本方法适用于(低位在前，高位在后)的顺序。 和bytesToInt（）配套使用 
    * @param value  
    *            要转换的int值 
    * @return byte数组 
    */ 
		public static byte[] intToBytes (int value)
		{   
				byte[] src = new byte[4];  
				src [3] = (byte)((value >> 24) & 0xFF);  
				src [2] = (byte)((value >> 16) & 0xFF);  
				src [1] = (byte)((value >> 8) & 0xFF);    
				src [0] = (byte)(value & 0xFF);                  
				return src;   
		}  

		/**  
    * 将int数值转换为占四个字节的byte数组，本方法适用于(高位在前，低位在后)的顺序。  和bytesToInt2（）配套使用 
    */    
		public static byte[] intToBytes2 (int value)
		{   
				byte[] src = new byte[4];  
				src [0] = (byte)((value >> 24) & 0xFF);  
				src [1] = (byte)((value >> 16) & 0xFF);  
				src [2] = (byte)((value >> 8) & 0xFF);    
				src [3] = (byte)(value & 0xFF);       
				return src;  
		}  

		/**  
    * byte数组中取int数值，本方法适用于(低位在前，高位在后)的顺序，和和intToBytes（）配套使用 
    *   
    * @param src  
    *            byte数组  
    * @param offset  
    *            从数组的第offset位开始  
    * @return int数值  
    */    
		public static int bytesToInt (byte[] src, int offset)
		{  
				int value;    
				value = (int)((src [offset] & 0xFF)   
						| ((src [offset + 1] & 0xFF) << 8)   
						| ((src [offset + 2] & 0xFF) << 16)   
						| ((src [offset + 3] & 0xFF) << 24));  
				return value;  
		}  
		
		/**  
    * byte数组中取int数值，本方法适用于(低位在后，高位在前)的顺序。和intToBytes2（）配套使用 
    */  
		public static int bytesToInt2 (byte[] src, int offset)
		{  
				int value;    
				value = (int)(((src [offset] & 0xFF) << 24)  
						| ((src [offset + 1] & 0xFF) << 16)  
						| ((src [offset + 2] & 0xFF) << 8)  
						| (src [offset + 3] & 0xFF));  
				return value;  
		}

		public void Send_MJ_ZuoZhuang (int ZhuangNum, long socketindex)
		{


				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_ZHUOZHUANG_REQ, System.BitConverter.GetBytes (ZhuangNum), sizeof(int), socketindex);


		}

		public void SendLaZhuang (int ZhuangNum)
		{

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_LAZHUANG_REQ, System.BitConverter.GetBytes (ZhuangNum), sizeof(int), socketindex);
		}

		public void SendZuoZhuang (int zhuangNum)
		{

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_ZHUOZHUANG_REQ, System.BitConverter.GetBytes (zhuangNum), sizeof(int), socketindex);
		}
	
		public void SendDasaizi ()
		{


				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_DASAIZI_REQ, null, 0, socketindex);
		}
	
		public void SendDasaiziag ()
		{


				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_DASAIZI_AG_REQ, null, 0, socketindex);
		}
	
		public void SendDapai (int card)
		{
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_HOST_MESSAGE);
//		bsData.Write((unsigned int) PT_MJ_DAPAI_REQ);
//		bsData.Write((unsigned int) card);
//		SendData(&bsData, m_ServerAddr);

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_DAPAI_REQ, System.BitConverter.GetBytes (card), sizeof(int), socketindex);

		}

		public void SendGangpass ()
		{
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_HOST_MESSAGE);
//		bsData.Write((unsigned int) PT_MJ_PENG_PASS_REQ);
//		SendData(&bsData, m_ServerAddr);

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_PENG_PASS_REQ, null, 0, socketindex);
		}
	
		public void SendGangreq ()
		{
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_HOST_MESSAGE);
//		bsData.Write((unsigned int) PT_MJ_GANG_PASS_REQ);
//		SendData(&bsData, m_ServerAddr);

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_GANG_PASS_REQ, null, 0, socketindex);
		}

		public void SendMopai ()
		{
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_HOST_MESSAGE);
//		bsData.Write((unsigned int) PT_MJ_MOPAI_REQ);
//		SendData(&bsData, m_ServerAddr);

				SendEx ((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_MJ_MOPAI_REQ, null, 0, socketindex);
		}

		public void SendHeartjump ()
		{
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_USER_HEART_JUMP);
//		SendData(&bsData, m_ServerAddr);


				//	SendEx_1id ((int)msg_id.PT_USER_HEART_JUMP, 0, 0, sockindex);
		}

		public void SendReady ()
		{
		
//		RakNet::BitStream bsData;
//		bsData.Write((unsigned char) PT_HOST_MESSAGE);
//		bsData.Write((unsigned int) PT_HOST_READY_REQUEST);
//		SendData(&bsData, m_ServerAddr);

				//	SendEx((int)msg_id.PT_HOST_MESSAGE, (int)host_msg.PT_HOST_READY_REQUEST, null, 0, socketindex);
		}

		public void SendEntergame (int serverid, int gameid,
	                          int uid)
		{
//		RakNet::BitStream ws;
//		ws.Write((unsigned char) PT_ENTER_GAME_REQUEST);
//		ws.Write((unsigned int) serverid);
//		ws.Write((unsigned int) gameid);
//		ws.Write((unsigned int) uid);
//		ws.Write((unsigned long long) 0);
//		SendData(&ws, m_ServerAddr);

		// Allocate 1 byte of unmanaged memory.


		IntPtr hGlobal = Marshal.AllocHGlobal(20);
	
		Marshal.WriteInt32(hGlobal, 0, serverid);
		Marshal.WriteInt32(hGlobal, 4, gameid);
		Marshal.WriteInt32(hGlobal, 8, uid);
		Marshal.WriteInt64 (hGlobal, 12, 12345678);


		byte[] by = new byte[20];
		Marshal.Copy (hGlobal, by, 0, 20);


		Marshal.FreeHGlobal(hGlobal);


		PT_ENTER_GAME_REQUEST_INFO info;
				info.gameid = gameid;
				info.uid = uid;
				info.guid = 12345678;
				info.serverid = serverid;
			//info.charname="zxcvbf";

				byte[] by1 = StructToBytes (info, Marshal.SizeOf (info));

				
		int n = Marshal.SizeOf (info);

		int t = 0;
			
		
				SendEx_1id ((int)msg_id.PT_ENTER_GAME_REQUEST, by1, Marshal.SizeOf (info), socketindex);
		}
}

