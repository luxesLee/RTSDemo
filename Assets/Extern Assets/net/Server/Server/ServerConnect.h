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

	ClientSocket* sock = new ClientSocket[ConnectN];	// �洢ÿһ�����ӵ�socket


public:
	Server() {
		server_sock = INVALID_SOCKET;
	}
	virtual ~Server() {
		Close();
	}

	// ��ʼ��socket
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


	// �󶨶˿�
	SOCKET Bind() {
		struct sockaddr_in server_addr;	// ��ַ
		memset(&server_addr, 0, sizeof(server_addr));
		int len = sizeof(server_addr);

		server_addr.sin_family = AF_INET;	// sin_familyЭ���

		server_addr.sin_addr.s_addr = htonl(INADDR_ANY);

		server_addr.sin_port = htons(port);	// ���ü����׽��ֶ˿�

		// �󶨼����׽��ֵ����ص�ַ�Ͷ˿ڣ�����Э�顢��ַ���˿ڣ�ռ���ڴ��С
		if (bind(server_sock, (struct sockaddr*)&server_addr, sizeof(sockaddr)) == -1) {
			cout << "bind error" << endl;
			exit(-1);
		}

		return server_sock;
	}

	// ����
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


	// ���տͻ�������
	SOCKET Accept() {

		struct sockaddr_in server_addr;	// ��ַ
		memset(&server_addr, 0, sizeof(server_addr));
		int len = sizeof(server_addr);

		server_addr.sin_family = AF_INET;	// sin_familyЭ���

		server_addr.sin_addr.s_addr = htonl(INADDR_ANY);

		server_addr.sin_port = htons(port);	// ���ü����׽��ֶ˿�
		SOCKET new_sock = INVALID_SOCKET;
		new_sock = accept(server_sock, (struct sockaddr*)&server_addr, &len);

		if (new_sock == -1) {
			cout << "accept error" << endl;
		}
		else {
			cout << "new accept" << endl;
			// �����������ӣ��ҵ�һ������λ��
			for (int i = 1; i < ConnectN; i++) {
				// ==0δʹ��
				if (sock[i].sock() == INVALID_SOCKET) {
					
					FD_SET(new_sock, &fd[0]);	// ���׽��ּ���set
					ConnectCount++;	// ������++
					sock[i] = *(new ClientSocket(new_sock));
					cout << "playerInto:__" << ConnectCount << "__playersocket:__" << sock[i].sock() << endl;
					break;
				}
			}
		}
		return new_sock;
	}


	// �ر�����Socket
	void Close() {
		for (int i = 0; i < ConnectN; i++) {
			if (sock[i].sock() != INVALID_SOCKET) {
				closesocket(sock[i].sock());	// �رտͻ���socket
			}
		}

		closesocket(server_sock);	// �ر�����socket
		WSACleanup();
	}

	// ѭ������������Ϣ
	void OnRun() {
		if (server_sock != INVALID_SOCKET) {
			struct timeval tv = { 5, 0 };	// ��ʱʱ��
			FD_ZERO(&fd[1]);
			fd[1] = fd[0];	// select���δ��Ӧ��������������

			int ret = select(0, &fd[1], NULL, NULL, &tv);
			if (ret < 0) {
				// ����
				cout << "select error" << endl;
			}
			else if (ret == 0) {
				// ��ʱ
				cout << "time out" << endl;
			}
			else {
				// ����׽����Ƿ���set�� && ���Ӳ���������
				if (FD_ISSET(sock[0].sock(), &fd[1])) {
					Accept();
				}


				for (size_t i = 1; i < ConnectN; i++) {
					// ����ÿ����set�е�����
					if (FD_ISSET(sock[i].sock(), &fd[1])) {
						cout << "RecvData" << endl;
						RecvData(i);
					}
				}
			}
		}
	}

	// �������ݣ�����ճ������ְ�
	// ****************************
	void RecvData(int i) {
		int tmp = recv(sock[i].sock(), sock[i].recvbuf(), 1000, 0);

		if (sock[i].recvint() > 0) {
			// �½��յķ��ڶ��������
			memcpy(sock[i].recvbuf2() + sock[i].recvint(), sock[i].recvbuf(), RE_BUFFSIZE - sock[i].recvint());

			tmp += sock[i].recvint();

			// ת�Ƶ�һ��������
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
				//������ź��ж��ˣ����������recv����
				cout << "recv data interrupted by signal." << endl;
			}
			else if (errno == EAGAIN)  //linux�µ�EWOULDBLOCK
			{
				cout << "recv errno EAGAIN." << endl;
			}
			else
			{
				//��ĳ�����
				cout << "recv error" << endl;
				cout << "playerQuit1:__" << sock[i].sock() << "__ConnectCount__" << ConnectCount << endl;

				FD_CLR(sock[i].sock(), &fd[0]);//��set�������ǰ�׽���
				closesocket(sock[i].sock());//�ر�����
				sock[i] = *(new ClientSocket(INVALID_SOCKET));//�׽�����������
				ConnectCount--;//������-1

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
		else {	// �ɹ�������Ϣ
			
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
					memcpy(&recvint1, sock[i].recvbuf() + startIndex, 4);//charתbyte      ƫ�Ƹ��Ű�����
					memcpy(&recvint2, sock[i].recvbuf() + 4 + startIndex, 4);//charתbyte,����ƫ��4λ
					memcpy(&recvint3, sock[i].recvbuf() + 8 + startIndex, 4);//charתbyte,����ƫ��8λ
					memcpy(&recvint4, sock[i].recvbuf() + 12 + startIndex, 4);//charתbyte,����ƫ��12λ
					//memcpy(&recvint5, sock[i].recvbuf() + 16 + startIndex, 4);//charתbyte,����ƫ��16λ
				
				
					int packsize = ntohl(bytesToInt(recvint1, 4));
					int stringsize = packsize - 16;
				
					// ��������
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

							// ����
							/*Parser();*/
							cout << "Parser" << endl;

							delete[] recvString;
							recvString = NULL;

							delete[] m_pack;
							m_pack = NULL;
						}

						// ���һ�β��
						startIndex += packsize;
						sock[i].set_recvLen(sock[i].recvLen() - packsize);
					}
					else {	// ������
						int offset = allrecvLen - sock[i].recvLen();
						sock[i].set_recvint(sock[i].recvLen());

						memcpy(sock[i].recvbuf2(), sock[i].recvbuf() + offset, sock[i].recvLen());

						cout << "break1" << endl;
						break;
					}
				}
				else {	// �����ͷ
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

	// ������
	void Parser(float x, float y, int unit) {

	}

};

#endif