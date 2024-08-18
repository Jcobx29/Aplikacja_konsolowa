using Moq;

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue(currentPassword); // User inputs the correct old password
            consoleInput.Enqueue(newPassword); // User inputs the new password

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword"); // User inputs the wrong old password
            consoleInput.Enqueue("e"); // User exits

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("wrongPassword"); // User inputs the wrong old password
            consoleInput.Enqueue(currentPassword); // User then inputs the correct old password
            consoleInput.Enqueue(newPassword); // User inputs the new password

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

            // Mockowanie konsoli
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

            // Mockowanie konsoli
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

            // Mockowanie konsoli
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

            // Mockowanie konsoli
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

            // Mockowanie konsoli
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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("1"); // Option 1: Show passwords
            consoleInput.Enqueue(correctPassword); // Provide correct password
            consoleInput.Enqueue("4"); // Exit after showing passwords

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("2"); // Option 2: Add new password
            consoleInput.Enqueue("NewPassword");
            consoleInput.Enqueue("MyApp");
            consoleInput.Enqueue("4"); // Exit after adding password

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("3"); // Option 3: Remove password
            consoleInput.Enqueue(correctPassword); // Provide correct password
            consoleInput.Enqueue("2"); // Choose to remove second password
            consoleInput.Enqueue("4"); // Exit after removing password

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("4"); // Option 4: Exit

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            deviceTemplate.UseOfDevice(filePath, 0, string.Empty, correctSecurePassword, devicePasswords, mockFileOperator.Object);

            // Assert
            // No specific assertions needed since we expect the method to simply exit
            // However, we can assert that no other operations (show, add, remove) were performed
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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("invalid"); // Invalid option
            consoleInput.Enqueue("4"); // Exit

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

            // Mockowanie konsoli
            var consoleInput = new Queue<string>();
            consoleInput.Enqueue("invalid"); // Invalid option
            consoleInput.Enqueue("3"); // Exit program

            var consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new System.IO.StringReader(string.Join(Environment.NewLine, consoleInput)));

            // Act
            app.Run(string.Empty, 0, 0, mockFileOperator.Object, mockDeviceTemplate.Object, 0, 0, 0, string.Empty, new List<string>(), mockSecurePassword.Object);

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("You have choose a wrong option. Try to put one from above.", output);
        }
    }
}
