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
	int _recvLen = 0;
	char _recvbuf2[RE_BUFFSIZE];
	int _recvint;
};


class Server
{
private:
	SOCKET server_sock;

	int port = 7777;

	int ConnectCount = 0;

	fd_set fd[2];

	ClientSocket* sock = new ClientSocket[ConnectN];	// 存储每一个连接的socket


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
			struct timeval tv = { 5, 0 };	// 超时时间
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
		int tmp = recv(sock[i].sock(), sock[i].recvbuf(), 1000, 0);

		if (sock[i].recvint() > 0) {
			// 新接收的放在二级缓存后
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
				//如果被信号中断了，则继续重试recv函数
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
			
		}
		else {	// 成功接受信息
			
			sock[i].set_recvLen(tmp);

			int allrecvLen = sock[i].recvLen();
			int startIndex = 0;
			while (sock[i].recvLen() > 0) {


				if (sock[i].recvLen() >= 16) {
					byte recvint1[4];
					byte recvint2[4];
					byte recvint3[4];
					byte recvint4[4];
					//float recvint5;
					memcpy(&recvint1, sock[i].recvbuf() + startIndex, 4);//char转byte      偏移跟着包个数
					memcpy(&recvint2, sock[i].recvbuf() + 4 + startIndex, 4);//char转byte,向右偏移4位
					memcpy(&recvint3, sock[i].recvbuf() + 8 + startIndex, 4);//char转byte,向右偏移8位
					memcpy(&recvint4, sock[i].recvbuf() + 12 + startIndex, 4);//char转byte,向右偏移12位
					//memcpy(&recvint5, sock[i].recvbuf() + 16 + startIndex, 4);//char转byte,向右偏移16位
				
				
					int packsize = ntohl(bytesToInt(recvint1, 4));
					int stringsize = packsize - 16;
				
					// 完整接收
					if (sock[i].recvLen() >= packsize) {
						if (stringsize > 0) {
							char* recvString = new char[stringsize + 1];
							if (recvString != NULL) {
								memcpy(recvString, sock[i].recvbuf() + 20 + startIndex, stringsize);
								*(recvString + stringsize) = '\0';
							}

							char* m_pack = new char[packsize];
							if (m_pack != NULL) {
								memcpy(m_pack, sock[i].recvbuf() + startIndex, packsize);
							}

							// 解析
							/*Parser();*/
							cout << "Parser" << endl;

							delete[] recvString;
							recvString = NULL;

							delete[] m_pack;
							m_pack = NULL;
						}

						// 完成一次拆包
						startIndex += packsize;
						sock[i].set_recvLen(sock[i].recvLen() - packsize);
					}
					else {	// 不完整
						int offset = allrecvLen - sock[i].recvLen();
						sock[i].set_recvint(sock[i].recvLen());

						memcpy(sock[i].recvbuf2(), sock[i].recvbuf() + offset, sock[i].recvLen());

						cout << "break1" << endl;
						break;
					}
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
	void Parser(float x, float y, int unit) {

	}

};

#endif