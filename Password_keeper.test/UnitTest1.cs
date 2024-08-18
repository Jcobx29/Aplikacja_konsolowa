using Moq;

namespace Password_keeper.test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var testList = new List<string>() {"abcd1234"};
            var mock = new Mock<ITextualRepository>();
            mock.Setup(s => s.Read("testFile.txt")).Returns(testList);

            var repo = new StringsTextualRepository();

            var actualList = repo.Read("testFile.txt");

            Assert.Equal(testList, actualList);
        }
    }
}