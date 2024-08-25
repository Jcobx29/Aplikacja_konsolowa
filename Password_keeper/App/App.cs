public class App
{
    public void Run(string choose, int option, int optionDevice, ITextualRepository fileOperator, DeviceTemplate deviceTemplate, int optionPhone, int optionComputer, int optionOthers, string securePassword, List<string> correctSecurePassword, SecurePassword securePasswordCheck)
    {
        do
        {
            Console.WriteLine("***********************************************************");
            Console.WriteLine("              Welcome in your password keeper              ");
            Console.WriteLine("                Choose what you want to do:                ");
            Console.WriteLine("                    1. Choose device                       ");
            Console.WriteLine("              2. Change you secure password                ");
            Console.WriteLine("                    3. Exit program                        ");
            Console.WriteLine("***********************************************************");

            choose = Console.ReadLine();
            Console.WriteLine();
            var isParsable = int.TryParse(choose, out option);

            if (option == 1)
            {
                do
                {
                    Console.WriteLine("You have three connected devices:");
                    Console.WriteLine("1. Phone");
                    Console.WriteLine("2. Computer");
                    Console.WriteLine("3. Others");
                    Console.WriteLine("4. Exit to previous page");
                    Console.WriteLine("Which one you want to use?: ");

                    var chooseDevice = Console.ReadLine();
                    var isParsableDevice = int.TryParse(chooseDevice, out optionDevice);
                    Console.WriteLine();

                    if (optionDevice == 1)
                    {
                        var phonePasswords = fileOperator.Read("phone.json");
                        deviceTemplate.UseOfDevice("phone.json", optionPhone, securePassword, correctSecurePassword, phonePasswords, fileOperator);
                    }
                    else if (optionDevice == 2)
                    {
                        var computerPasswords = fileOperator.Read("computer.json");
                        deviceTemplate.UseOfDevice("computer.json", optionComputer, securePassword, correctSecurePassword, computerPasswords, fileOperator);
                    }
                    else if (optionDevice == 3)
                    {
                        var othersPasswords = fileOperator.Read("others.json");
                        deviceTemplate.UseOfDevice("others.json", optionOthers, securePassword, correctSecurePassword, othersPasswords, fileOperator);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                } while (optionDevice != 4);
            }

            else if (option == 2)
            {
                securePasswordCheck.SecurePasswordChange(fileOperator, "securePassword.txt");
            }

            else if (option != 1 && option != 2 && option != 3)
            {
                Console.WriteLine("You have choose a wrong option. Try to put one from above.");
            }
            Console.WriteLine();
        } while (option != 3);
    }
}