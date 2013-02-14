C-Sharp-Voip-Manager-Teamspeak-3
================================

Eventually, I plan on supporting Teamspeak 3, Ventrillo, and Mumble, but don't hold your breath waiting for me to support them. Currently, Teamspeak 3 is fully supported.

== Teamspeak 3 ==
Under VoipManager.Connections, you'll find Teamspeak3Conection.  There are 3 methods, Connect, Disconnect, and Send (and their Async equivalents).  You build Teamspeak3Requests to send to the server and receive Teamspeak3Messages in turn.  If you prefer to send and receive stuff asynchronously, there are several events you can subscribe to: Connected, Disconnected, Sent, Received, Notified, Banned.  The library was built using .Net 4.0, but it's possible to use combine it with .Net 4.5 in order to use async and await.  I'll add an example on how to use Teamspeak3Connection to the code-base later on.

- Imisnew2