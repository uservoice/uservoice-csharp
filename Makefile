MONO_OPTIONS=--runtime=v4.0
LIBS=-r:Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll -r:RestSharp.104.1/lib/net4/RestSharp.dll
test: build_test_suite
	mono $(MONO_OPTIONS) TestSuite.exe

build_test_suite: TestSuite.exe

TestSuite.exe: UserVoice/Client.cs Test/TestSuite.cs Test/Test.cs Test/ClientTest.cs
	dmcs $(LIBS) UserVoice/Client.cs Test/TestSuite.cs Test/Test.cs Test/ClientTest.cs -out:TestSuite.exe
lib: packages.config
	nuget install packages.config && ln -s Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll && ln -s RestSharp.104.1/lib/net4/RestSharp.dll

clean:
	rm -f *.exe