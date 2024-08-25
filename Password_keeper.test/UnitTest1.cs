using Moq;
using System.Text.Json;

namespace Password_keeper.test
{
    public class UnitTest1
    {
        
        private readonly StringsTextualRepository _repository;

        public UnitTest1()
        {
            _repository = new StringsTextualRepository();
        }

        [Fact]
        public void Read_ShouldReturnListOfStrings_WhenFileContainsMultipleLines()
        {
            // Arrange
            var filePath = Path.GetTempFileName();
            var expectedContents = new List<string> { "Line1", "Line2", "Line3" };
            File.WriteAllText(filePath, string.Join(Environment.NewLine, expectedContents));

            // Act
            var result = _repository.Read(filePath);

            // Assert
            Assert.Equal(expectedContents, result);
        }
        [Fact]
        public void Write_ShouldCreateFileWithCorrectContents_WhenListIsNotEmpty()
        {
            // Arrange
            var filePath = Path.GetTempFileName();
            var contentsToWrite = new List<string> { "Line1", "Line2", "Line3" };

            // Act
            _repository.Write(filePath, contentsToWrite);

            // Assert
            var fileContents = File.ReadAllText(filePath);
            var expectedContents = string.Join(Environment.NewLine, contentsToWrite);
            Assert.Equal(expectedContents, fileContents);
        }
        [Fact]
        public void SecurePasswordChange_ShouldChangePassword_WhenCorrectOldPasswordIsProvided()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var filePath = "testFilePath";
            var currentPassword = "oldPassword";
            var newPassword = "newPassword";

            mockFileOperator.Setup(f => f.Read(filePath)).Returns(new List<string> { currentPassword });

            var securePassword = new SecurePassword();            
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue(currentPassword);
            consoleInput.Enqueue(newPassword); 

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            securePassword.SecurePasswordChange(mockFileOperator.Object, filePath);

            // Assert
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => l[0] == newPassword)), Times.Once);
        }
        
        [Fact]
        public void SecurePasswordChange_ShouldNotChangePassword_WhenWrongPasswordIsProvidedAndUserExits()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var filePath = "testFilePath";
            var currentPassword = "oldPassword";

            mockFileOperator.Setup(f => f.Read(filePath)).Returns(new List<string> { currentPassword });

            var securePassword = new SecurePassword();
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword");
            consoleInput.Enqueue("e");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            securePassword.SecurePasswordChange(mockFileOperator.Object, filePath);

            // Assert
            mockFileOperator.Verify(f => f.Write(filePath, It.IsAny<List<string>>()), Times.Never);
        }

        [Fact]
        public void SecurePasswordChange_ShouldRetry_WhenWrongPasswordIsProvidedThenCorrectPassword()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var filePath = "testFilePath";
            var currentPassword = "oldPassword";
            var newPassword = "newPassword";

            mockFileOperator.Setup(f => f.Read(filePath)).Returns(new List<string> { currentPassword });

            var securePassword = new SecurePassword();
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword");
            consoleInput.Enqueue(currentPassword);
            consoleInput.Enqueue(newPassword);

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            securePassword.SecurePasswordChange(mockFileOperator.Object, filePath);

            // Assert
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => l[0] == newPassword)), Times.Once);
        }
        [Fact]
        public void Show_ShouldDisplayPasswords_WhenCorrectSecurePasswordIsProvided()
        {
            // Arrange
            var deviceUsing = new DeviceUsing();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2" };

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue(correctPassword);

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));
            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            deviceUsing.Show(string.Empty, correctSecurePassword, devicePasswords);

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("1. Password1", output);
            Assert.Contains("2. Password2", output);
        }

        [Fact]
        public void Show_ShouldNotDisplayPasswords_WhenIncorrectSecurePasswordIsProvidedAndUserExits()
        {
            // Arrange
            var deviceUsing = new DeviceUsing();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2" };

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword");
            consoleInput.Enqueue("e");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));
            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            deviceUsing.Show(string.Empty, correctSecurePassword, devicePasswords);

            // Assert
            var output = consoleOutput.ToString();
            Assert.DoesNotContain("Password1", output);
            Assert.DoesNotContain("Password2", output);
        }

        [Fact]
        public void Add_ShouldAddPasswordToDevicePasswordsListAndSaveToFile()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceUsing = new DeviceUsing();
            var devicePasswords = new List<string>();
            var filePath = "testFilePath";
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("NewPassword");
            consoleInput.Enqueue("MyApp");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceUsing.Add(devicePasswords, mockFileOperator.Object, filePath);

            // Assert
            Assert.Contains("NewPassword (MyApp)", devicePasswords);
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => l.Contains("NewPassword (MyApp)"))), Times.Once);
        }

        [Fact]
        public void Remove_ShouldRemovePasswordFromDevicePasswordsListAndSaveToFile_WhenCorrectPasswordAndValidIdIsProvided()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceUsing = new DeviceUsing();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2", "Password3" };
            var filePath = "testFilePath";
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue(correctPassword);
            consoleInput.Enqueue("2");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceUsing.Remove(string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object, filePath);

            // Assert
            Assert.DoesNotContain("Password2", devicePasswords);
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => !l.Contains("Password2"))), Times.Once);
        }

        [Fact]
        public void Remove_ShouldNotRemoveAnyPassword_WhenIncorrectPasswordIsProvidedAndUserExits()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceUsing = new DeviceUsing();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2", "Password3" };
            var filePath = "testFilePath";
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword");
            consoleInput.Enqueue("e");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceUsing.Remove(string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object, filePath);

            // Assert
            Assert.Contains("Password1", devicePasswords);
            Assert.Contains("Password2", devicePasswords);
            Assert.Contains("Password3", devicePasswords);
            mockFileOperator.Verify(f => f.Write(filePath, It.IsAny<List<string>>()), Times.Never);
        }
        [Fact]
        public void UseOfDevice_ShouldShowPasswords_WhenOptionIs1AndCorrectPasswordIsProvided()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceTemplate = new DeviceTemplate();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2" };
            var filePath = "testFilePath";
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("1");
            consoleInput.Enqueue(correctPassword);
            consoleInput.Enqueue("4");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));
            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("1. Password1", output);
            Assert.Contains("2. Password2", output);
        }

        [Fact]
        public void UseOfDevice_ShouldAddPassword_WhenOptionIs2()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceTemplate = new DeviceTemplate();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string>();
            var filePath = "testFilePath";

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("2"); 
            consoleInput.Enqueue("NewPassword");
            consoleInput.Enqueue("MyApp");
            consoleInput.Enqueue("4");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            Assert.Contains("NewPassword (MyApp)", devicePasswords);
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => l.Contains("NewPassword (MyApp)"))), Times.Once);
        }

        [Fact]
        public void UseOfDevice_ShouldRemovePassword_WhenOptionIs3AndCorrectPasswordIsProvided()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceTemplate = new DeviceTemplate();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2", "Password3" };
            var filePath = "testFilePath";

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("3");
            consoleInput.Enqueue(correctPassword); 
            consoleInput.Enqueue("2"); 
            consoleInput.Enqueue("4"); 

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            Assert.DoesNotContain("Password2", devicePasswords);
            mockFileOperator.Verify(f => f.Write(filePath, It.Is<List<string>>(l => !l.Contains("Password2"))), Times.Once);
        }

        [Fact]
        public void UseOfDevice_ShouldExit_WhenOptionIs4()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceTemplate = new DeviceTemplate();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2", "Password3" };
            var filePath = "testFilePath";

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("4");

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            mockFileOperator.Verify(f => f.Write(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [Fact]
        public void UseOfDevice_ShouldHandleInvalidOption()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var deviceTemplate = new DeviceTemplate();
            var correctPassword = "correctPassword";
            var correctSecurePassword = new List<string> { correctPassword };
            var devicePasswords = new List<string> { "Password1", "Password2", "Password3" };
            var filePath = "testFilePath";

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("invalid"); 
            consoleInput.Enqueue("4"); 

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));
            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("You have put wrong option. Try again.", output);
        }       
        [Fact]
        public void Run_ShouldHandleInvalidOption()
        {
            // Arrange
            var mockFileOperator = new Mock<ITextualRepository>();
            var mockDeviceTemplate = new Mock<DeviceTemplate>();
            var mockSecurePassword = new Mock<SecurePassword>();

            var app = new App();

            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("invalid"); 
            consoleInput.Enqueue("3"); 

            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            app.Run(string.Empty, 0, 0, mockFileOperator.Object, mockDeviceTemplate.Object, 0, 0, 0, string.Empty, new List<string>(), mockSecurePassword.Object);

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("You have choose a wrong option. Try to put one from above.", output);
        }
        
        [Fact]
        public void Read_ShouldReturnListOfStrings_WhenJsonFileIsValid()
        {
            // Arrange
            var repository = new StringsJsonRepository();
            var expectedStrings = new List<string> { "string1", "string2", "string3" };
            var tempFilePath = Path.GetTempFileName();           
            File.WriteAllText(tempFilePath, JsonSerializer.Serialize(expectedStrings));

            // Act
            var result = repository.Read(tempFilePath);

            // Assert
            Assert.Equal(expectedStrings, result);

            // Clean up
            File.Delete(tempFilePath);
        }
        
        [Fact]
        public void Write_ShouldCreateJsonFileWithCorrectContent()
        {
            // Arrange
            var repository = new StringsJsonRepository();
            var stringsToWrite = new List<string> { "string1", "string2", "string3" };
            var tempFilePath = Path.GetTempFileName();

            // Act
            repository.Write(tempFilePath, stringsToWrite);

            // Assert
            var fileContents = File.ReadAllText(tempFilePath);
            var deserializedStrings = JsonSerializer.Deserialize<List<string>>(fileContents);
            Assert.Equal(stringsToWrite, deserializedStrings);

            // Clean up
            File.Delete(tempFilePath);
        }
    }
}
