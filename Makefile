MONO_OPTIONS=--runtime=v4.0
LIBS=-r:Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll -r:RestSharp.104.1/lib/net4/RestSharp.dll -r:System.Web.dll
test: build_test_suite
	mono $(MONO_OPTIONS) TestSuite.exe

build_test_suite: TestSuite.exe

bin/UserVoice.dll: UserVoice/Client.cs UserVoice/Collection.cs UserVoice/SSO.cs
	dmcs $(LIBS) UserVoice/Client.cs UserVoice/Collection.cs UserVoice/SSO.cs -t:library -out:bin/UserVoice.dll
TestSuite.exe: bin/UserVoice.dll UserVoice.Test/Test.cs UserVoice.Test/ClientTest.cs UserVoice.Test/SSOTest.cs
	dmcs $(LIBS) -r:bin/UserVoice.dll UserVoice.Test/Test.cs UserVoice.Test/ClientTest.cs UserVoice.Test/SSOTest.cs -out:TestSuite.exe
lib: packages.config
	nuget install packages.config && ln -s Newtonsoft.Json.4.5.10/lib/net40/Newtonsoft.Json.dll && ln -s RestSharp.104.1/lib/net4/RestSharp.dll

clean:
	rm -f *.exe bin/*.*