MONO_OPTIONS=--runtime=v4.0
LIBS=-r:Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll -r:RestSharp.104.1/lib/net4/RestSharp.dll
test: build_test_suite
	mono $(MONO_OPTIONS) TestSuite.exe

build_test_suite: TestSuite.exe

TestSuite.exe: UserVoice/Client.cs Test/TestSuite.cs
	dmcs $(LIBS) UserVoice/Client.cs Test/TestSuite.cs -out:TestSuite.exe
lib: packages.config
	nuget install packages.config

clean:
	rm -f Test/*.exe