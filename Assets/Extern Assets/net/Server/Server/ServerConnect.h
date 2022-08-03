#ifndef SERVERCONNECT_H
#define SERVERCONNECT_H

#ifndef FD_SETSIZE
#define FD_SETSIZE	1024
#endif // !FD_SETSIZE

#ifndef ConnectN
#define ConnectN 1024
#endif

#ifndef RE_BUFFSIZE
#define RE_BUFFSIZE  10240
#endif /* RE_BUFFSIZE */

#include <iostream>
#include <WinSock2.h>
#include <mstcpip.h>
#include <string>
#pragma comment(lib, "ws2_32.lib")

using namespace std;

class ClientSocket {
public:
	ClientSocket(SOCKET Input = INVALID_SOCKET) {
		_sock = Input;
		memset(_recvbuf, 0, sizeof(_recvbuf));
		memset(_recvbuf2, 0, sizeof(_recvbuf));
		_recvLen = 0;
		_recvint = 0;
	}
	~ClientSocket() {

	}

	SOCKET sock() {
		return _sock;
	}
	char* recvbuf() {
		return _recvbuf;
	}
	char* recvbuf2() {
		return _recvbuf2;
	}
	int recvLen() {
		return _recvLen;
	}
	void set_recvLen(int input) {
		_recvLen = input;
	}
	int recvint() {
		return _recvint;
	}
	void set_recvint(int input) {
		_recvint = input;
	}
	void set_sock(SOCKET sock) {
		_sock = sock;
	}

private:
	SOCKET _sock;

	char _recvbuf[RE_BUFFSIZE];
	
	int _recvLen = 0;	// 一级缓冲区存储长度
	
	char _recvbuf2[RE_BUFFSIZE];
	
	int _recvint;	// 二级缓冲区存储的长度
};


class Server
{
private:
	SOCKET server_sock;

	int port = 7777;

	int ConnectCount = 0;

	fd_set fd[2];

	ClientSocket* sock = new ClientSocket[ConnectN];	// 存储每一个连接的socket

	int Room[10][2] = { {0,0}, {0,0}, {0,0}, {0,0},{0,0},{0,0}, {0,0}, {0,0}, {0,0},{0,0} };

	// 每个连接储存两个信息
	// 1.连接哪个Room  2.坐0、1哪个位子
	int Site[ConnectN][2] = {};

public:
	Server() {
		server_sock = INVALID_SOCKET;
	}
	virtual ~Server() {
		Close();
	}

	// 初始化socket
	SOCKET InitSocket() {

		WSAData wsaData;

		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			cout << "WSAStartup error" << endl;
			return 1;
		}

		if ((server_sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1) {
			cout << "Socket error" << endl;
			exit(-1);
		}

		return server_sock;
	}


	// 绑定端口
	SOCKET Bind() {
		struct sockaddr_in server_addr;	// 地址
		memset(&server_addr, 0, sizeof(server_addr));
		int len = sizeof(server_addr);

		server_addr.sin_family = AF_INET;	// sin_family协议簇

		server_addr.sin_addr.s_addr = htonl(INADDR_ANY);

		server_addr.sin_port = htons(port);	// 设置监听套接字端口

		// 绑定监听套接字到本地地址和端口，定义协议、地址、端口，占用内存大小
		if (bind(server_sock, (struct sockaddr*)&server_addr, sizeof(sockaddr)) == -1) {
			cout << "bind error" << endl;
			exit(-1);
		}

		return server_sock;
	}

	// 监听
	SOCKET Listen() {

		if (listen(server_sock, 100) == -1) {
			cout << "listen error" << endl;
			exit(-1);
		}

		FD_ZERO(&fd[0]);
		FD_SET(server_sock, &fd[0]);


		sock[0] = *(new ClientSocket(server_sock));

		cout << "sock[0]:" << sock[0].sock() << endl;
		
		return server_sock;
	}


	// 接收客户端连接
	SOCKET Accept() {

		struct sockaddr_in server_addr;	// 地址
		memset(&server_addr, 0, sizeof(server_addr));
		int len = sizeof(server_addr);

		server_addr.sin_family = AF_INET;	// sin_family协议簇

		server_addr.sin_addr.s_addr = htonl(INADDR_ANY);

		server_addr.sin_port = htons(port);	// 设置监听套接字端口
		SOCKET new_sock = INVALID_SOCKET;
		new_sock = accept(server_sock, (struct sockaddr*)&server_addr, &len);

		if (new_sock == -1) {
			cout << "accept error" << endl;
		}
		else {
			cout << "new accept" << endl;
			// 遍历所有连接，找到一个合适位置
			for (int i = 1; i < ConnectN; i++) {
				// ==0未使用
				if (sock[i].sock() == INVALID_SOCKET) {
					
					FD_SET(new_sock, &fd[0]);	// 新套接字加入set
					ConnectCount++;	// 连接者++
					sock[i] = *(new ClientSocket(new_sock));
					cout << "playerInto:__" << ConnectCount << "__playersocket:__" << sock[i].sock() << endl;
					break;
				}
			}
		}
		return new_sock;
	}


	// 关闭所有Socket
	void Close() {
		for (int i = 0; i < ConnectN; i++) {
			if (sock[i].sock() != INVALID_SOCKET) {
				closesocket(sock[i].sock());	// 关闭客户端socket
			}
		}

		closesocket(server_sock);	// 关闭自身socket
		WSACleanup();
	}

	// 循环处理网络消息
	void OnRun() {
		if (server_sock != INVALID_SOCKET) {
			struct timeval tv;	// 超时时间
			tv.tv_sec = 5; tv.tv_usec = 0;

			FD_ZERO(&fd[1]);
			fd[1] = fd[0];	// select清空未响应的描述符，备份

			int ret = select(0, &fd[1], NULL, NULL, &tv);
			if (ret < 0) {
				// 错误
				cout << "select error" << endl;
			}
			else if (ret == 0) {
				// 超时
				cout << "time out" << endl;
			}
			else {
				// 检查套接字是否在set中 && 连接不超过上限
				if (FD_ISSET(sock[0].sock(), &fd[1])) {
					Accept();
				}


				for (size_t i = 1; i < ConnectN; i++) {
					// 对于每个在set中的连接
					if (FD_ISSET(sock[i].sock(), &fd[1])) {
						cout << "RecvData" << endl;
						RecvData(i);
					}
				}
			}
		}
	}

	// 接收数据，处理粘包，拆分包
	// ****************************
	void RecvData(int i) {
		// socket、缓冲区、缓冲区长度、0
		// 返回实际copy的字节数
		int tmp = recv(sock[i].sock(), sock[i].recvbuf(), 1000, 0);

		if (sock[i].recvint() > 0) {
			// 新接收的先放在二级缓存后
			memcpy(sock[i].recvbuf2() + sock[i].recvint(), sock[i].recvbuf(), RE_BUFFSIZE - sock[i].recvint());

			tmp += sock[i].recvint();

			// 转移到一级缓存中
			memcpy(sock[i].recvbuf(), sock[i].recvbuf2(), RE_BUFFSIZE);
		}

		sock[i].set_recvint(0);

		if (tmp < 0) {
			if (errno == EWOULDBLOCK)
			{
				cout << "There is no data available now." << endl;
			}
			else if (errno == EINTR)
			{
				// 信号中断，则继续重试recv函数
				cout << "recv data interrupted by signal." << endl;
			}
			else if (errno == EAGAIN)  //linux下的EWOULDBLOCK
			{
				cout << "recv errno EAGAIN." << endl;
			}
			else
			{
				//真的出错了
				cout << "recv error" << endl;
				cout << "playerQuit1:__" << sock[i].sock() << "__ConnectCount__" << ConnectCount << endl;

				FD_CLR(sock[i].sock(), &fd[0]);//在set中清除当前套接字
				closesocket(sock[i].sock());//关闭连接
				sock[i] = *(new ClientSocket(INVALID_SOCKET));//套接字数组清零
				ConnectCount--;//连接数-1


				Room[Site[i][0]][Site[i][1]] = 0;	// 清空房间
				
				// 清空连接
				Site[i][0] = 0;
				Site[i][1] = 0;
			}
		}
		else if (tmp == 0) {
			cout << "recv = 0" << endl;
			cout << "playerQuit2:__" << sock[i].sock() << "__ConnectCount__" << ConnectCount << endl;
			cout << sock[i].sock() << endl;


			FD_CLR(sock[i].sock(), &fd[0]);
			closesocket(sock[i].sock());
			sock[i] = *(new ClientSocket(INVALID_SOCKET));
			ConnectCount--;
			
			Room[Site[i][0]][Site[i][1]] = 0;	// 清空房间

			// 清空连接
			Site[i][0] = 0;
			Site[i][1] = 0;

		}
		else {	// 成功接受信息
			
			sock[i].set_recvLen(tmp);

			int allrecvLen = sock[i].recvLen();
			int startIndex = 0;
			while (sock[i].recvLen() > 0) {

				if (sock[i].recvLen() >= 16) {
					// 根据预定义的包内容拆包
					//byte recvint1[4];
					//byte recvint2[4];
					//byte recvint3[4];
					//byte recvint4[4];
					//float recvint5;
					//memcpy(&recvint1, sock[i].recvbuf() + startIndex, 4);
					//memcpy(&recvint2, sock[i].recvbuf() + 4 + startIndex, 4);
					//memcpy(&recvint3, sock[i].recvbuf() + 8 + startIndex, 4);
					//memcpy(&recvint4, sock[i].recvbuf() + 12 + startIndex, 4);
					//memcpy(&recvint5, sock[i].recvbuf() + 16 + startIndex, 4);//char转byte,向右偏移16位
					//
					//
					//int packsize = ntohl(bytesToInt(recvint1, 4));	// 包整体的大小
					//int stringsize = packsize - 20;	// 不定长string的传输=总包-固定长

					/*
					* 这里只通过网络实现最简单的功能，demo需要，因此仅使用定长包
					*/
					float recvint1;
					float recvint2;
					byte recvint3[4];
					byte recvint4[4];

					memcpy(&recvint1, sock[i].recvbuf() + startIndex, 4);
					memcpy(&recvint2, sock[i].recvbuf() + 4 + startIndex, 4);
					memcpy(&recvint3, sock[i].recvbuf() + 8 + startIndex, 4);
					memcpy(&recvint4, sock[i].recvbuf() + 12 + startIndex, 4);

					// 由于客户端没转网络字节序，服务器也不转
					int root = (bytesToInt(recvint3, 4));
					int unit = (bytesToInt(recvint4, 4));

					char* pack = new char[16];
					if (pack != NULL) {
						memcpy(pack, sock[i].recvbuf() + startIndex, 16);
					}

					// 解析
					Parser(sock[i].sock(), i, recvint1, recvint2, root, unit, pack);

					cout << "receive data " << recvint1 << " " << recvint2 << " " << root << " " << unit << endl;

					startIndex += 16;
					sock[i].set_recvLen(sock[i].recvLen() - 16);
					// 完整接收
					//if (sock[i].recvLen() >= 16) {
						// 存在不定长string
						//if (stringsize > 0) {
						//
						//	char* recvString = new char[stringsize + 1];
						//	if (recvString != NULL) {
						//		memcpy(recvString, sock[i].recvbuf() + 20 + startIndex, stringsize);
						//		*(recvString + stringsize) = '\0';
						//	}
						//
						//	char* m_pack = new char[packsize];
						//	if (m_pack != NULL) {
						//		memcpy(m_pack, sock[i].recvbuf() + startIndex, packsize);
						//	}
						//
						//	// 解析
						//	/*Parser();*/
						//	cout << "Parser" << endl;
						//	cout << ntohl(bytesToInt(recvint1)) << " " << ntohl(bytesToInt(recvint2)) << " " << ntohl(bytesToInt(recvint3)) << " "\
						//		<< ntohl(bytesToInt(recvint4)) << endl;
						//	
						//
						//	delete[] recvString;
						//	recvString = NULL;
						//
						//	delete[] m_pack;
						//	m_pack = NULL;
						//}
						//
						//// 完成一次拆包
						//startIndex += packsize;
						//sock[i].set_recvLen(sock[i].recvLen() - packsize);
					//}
					//else {	// 不完整
					//	int offset = allrecvLen - sock[i].recvLen();
					//	sock[i].set_recvint(sock[i].recvLen());
					//
					//	// 存入二级缓冲区
					//	memcpy(sock[i].recvbuf2(), sock[i].recvbuf() + offset, sock[i].recvLen());
					//
					//	cout << "break1" << endl;
					//	break;
					//}
				}
				else {	// 不足包头
					int offset = allrecvLen - sock[i].recvLen();
					sock[i].set_recvint(sock[i].recvLen());

					memcpy(sock[i].recvbuf2(), sock[i].recvbuf() + offset, sock[i].recvLen());

					cout << "break2" << endl;

					break;
				}
			}
		}
	}


	int bytesToInt(byte* bytes, int size = 4) {
		int addr = bytes[0] & 0xFF;
		addr |= ((bytes[1] << 8) & 0xFF00);
		addr |= ((bytes[2] << 16) & 0xFF0000);
		addr |= ((bytes[3] << 24) & 0xFF000000);
		return addr;
	}

	// 解析包
	void Parser(int socket, int i, float x, float z, int room, int unit, char pack[]) {

		if (x == 0 && z == 0 && room == 0 && unit == 0) {	// 开启游戏

			// 房间中人满了才可以开始
			if (Room[Site[i][0]][Site[i][1]] != 0 && Room[Site[i][0]][!Site[i][1]] != 0) {
				cout << "Game Start" << endl;

				send(socket, pack, 16, 0);

				int opponent = Room[Site[i][0]][!Site[i][1]];
				send(sock[opponent].sock(), pack, 16, 0);
			}

		}
		else if (room != 0 && unit != 0) {	// 进入房间
			if (Site[i][0] != 0) {
				cout << socket << " has enter room" << Site[i][0] << endl;

				return;
			}


			if (Room[room][0] == 0) {	// 0号位空

				Room[room][0] = i;
				Site[i][0] = room;
				Site[i][1] = 0;

				cout << socket << " enter room:" << Site[i][0] << " site" << Site[i][1] << endl;

			}

			else if (Room[room][1] == 0) {	// 1号位空

					Room[room][1] = i;
					Site[i][0] = room;
					Site[i][1] = 1;

					cout << socket << " enter room:" << Site[i][0] << " site" << Site[i][1] << endl;

			}
			else {	// 0、1均满

				cout << "_________full_________" << room << endl;

			}
		}
		else if (room == -1) {	// 退出房间

			if (Room[Site[i][0]][Site[i][1]] != 0) {
				cout << socket << " exit room:" << Site[i][0] << " site" << Site[i][1] << endl;

				Room[Site[i][0]][Site[i][1]] = 0;	// 清空房间

				// 清空连接
				Site[i][0] = 0;
				Site[i][1] = 0;
			}

		}
		else if (x != 0 && z != 0 && room == 0 && unit != 0) {	// 移动单位

			// 获取对手socket的index
			int opponent = Room[Site[i][0]][!Site[i][1]];

			cout << sock[opponent].sock() << endl;

			send(sock[opponent].sock(), pack, 16, 0);

		}


	}



};

#endif