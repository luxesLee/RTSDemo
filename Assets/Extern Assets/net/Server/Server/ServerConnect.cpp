#include "ServerConnect.h"

int main() {
	Server server;
	server.InitSocket();
	server.Bind();
	server.Listen();

	while (true) {
		server.OnRun();
	}
	return 0;
}