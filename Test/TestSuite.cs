namespace Test
{
    public class TestSuite {
        public static void Main() {
            new ClientTest();
            System.Console.WriteLine("Assertions failed: " + Test.AssertionsFailed() + " / " + Test.AssertionsTotal());
        }
    }
}