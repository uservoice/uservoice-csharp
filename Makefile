run: UserVoiceClient.exe
	mono UserVoiceClient.exe
UserVoiceClient.exe: UserVoice/Client.cs UserVoice/Main.cs
	dmcs -r:Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll -r:RestSharp.104.1/lib/net4/RestSharp.dll UserVoice/Client.cs  UserVoice/Main.cs -out:UserVoiceClient.exe
lib: packages.config
	nuget install packages.config