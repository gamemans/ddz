using System;
using System.Runtime.InteropServices;

public struct Stest{
public 	int	age;
public 	string	name;
};

//================================================
//[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
//public	struct PT_ENTER_GAME_REQUEST_INFO
//{
//	
//	public	int gameid;
//	public	int userid;
//	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
//	public string charname;
//	public	Int64 guid;
//	
//};

//====================================================

//RakNet::BitStream ws;
//ws.Write((unsigned char) PT_ENTER_GAME_REQUEST);
//ws.Write((unsigned int) serverid);
//ws.Write((unsigned int) gameid);
//ws.Write((unsigned int) uid);
//ws.Write((unsigned long long) 0);
//SendData(&ws, m_ServerAddr);


enum msg_id
{
	ID_CONNECTION_REQUEST_ACCEPTED = 16,

	ID_CONNECTION_ATTEMPT_FAILED,
	/// RakPeer - Sent a connect request to a system we are currently connected to.
	ID_ALREADY_CONNECTED,
	/// RakPeer - A remote system has successfully connected.
	ID_NEW_INCOMING_CONNECTION,
	/// RakPeer - The system we attempted to connect to is not accepting new connections.
	ID_NO_FREE_INCOMING_CONNECTIONS,
	/// RakPeer - The system specified in Packet::systemAddress has disconnected from us.  For the client, this would mean the
	/// server has shutdown. 
	ID_DISCONNECTION_NOTIFICATION,
	/// RakPeer - Reliable packets cannot be delivered to the system specified in Packet::systemAddress.  The connection to that
	/// system has been closed. 
	ID_CONNECTION_LOST,
	/// RakPeer - We are banned from the system we attempted to connect to.
	ID_CONNECTION_BANNED,
	/// RakPeer - The remote system is using a password and has refused our connection because we did not set the correct password.
	ID_INVALID_PASSWORD,

	PT_ENTER_GAME_REQUEST = 140,

	PT_ENTER_GAME_ACCEPT = 141,

	PT_HOST_MESSAGE = 180,

	PT_USER_INGAME_MSG = 255,
};

enum host_msg
{
	PT_MJ_OPT_FAIL = 72,				
	
	PT_MJ_DAPAI_REQ,
	PT_MJ_USER_DAPAI,
	
	PT_MJ_DASAIZAI_START,
	PT_MJ_DASAIZI_REQ,
	PT_MJ_USER_DASAIZI,
	
	
	PT_MJ_DASAIZI_AG_REQ,
	PT_MJ_USER_DASAIZI_AG,
	
	PT_MJ_GANG_REQ,
	PT_MJ_GANG_PASS_REQ,
	PT_MJ_USER_GANG,
	
	PT_MJ_PENG_REQ,
	PT_MJ_PENG_PASS_REQ,
	PT_MJ_USER_PENG,
	
	PT_MJ_HU_REQ,
	PT_MJ_USER_HU,
	
	PT_MJ_MOPAI_REQ,
	PT_MJ_USER_MOPAI,//89
	
	PT_MJ_MATCH_ACCEPT,
	
	PT_MJ_GAME_START,
	
	PT_MJ_ZHUOZHUANG_REQ,	//<92
	PT_MJ_USER_ZHUOZHUANG,
	
	PT_MJ_LAZHUANG_REQ,
	PT_MJ_USER_LAZHUANG,
	
	PT_MJ_FAPAI,

	PT_MJ_WAIT_USER_OPT,
	PT_MJ_END_GAME,
};
class CServerInfo
{
public static int SERVER_ID=101;//(300-302)
public static int GAME_ID = 2;
public static int m_nUserId = 10000888;
};


[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public	struct PT_ENTER_GAME_REQUEST_INFO
{
	public	int serverid;
	public	int gameid;
	public	int uid;
	public	Int64 guid;
	
};

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public	struct PT_ENTER_GAME_ACCEPT_INFO
{
	public byte id;				///<消息ID

};


