public class DeviceUsing
{
    public void Show(string securePassword, List<string> correctSecurePassword, List<string> devicePasswords)
    {
        do
        {
            Console.WriteLine("To see passwords, at first you have to put your secure password:");
            securePassword = Console.ReadLine();
            Console.WriteLine();

            if (securePassword == correctSecurePassword[0])
            {
                int passwordId = 1;
                foreach (var devicePassword in devicePasswords)
                {
                    Console.WriteLine($"{passwordId}. {devicePassword}");
                    passwordId++;
                }
                Console.WriteLine();
                break;
            }
            else if (securePassword != correctSecurePassword[0] && securePassword != "e" && securePassword != "E")
            {
                Console.WriteLine("Password was incorrect. Try again or type E to exit to previous menu.");
            }
        } while (securePassword != "e" && securePassword != "E");
    }

    public void Add(List<string> devicePasswords, ITextualRepository fileOperator, string filePath)
    {
        Console.WriteLine("Write a Password you want to add:");
        var newPassword = Console.ReadLine();
        Console.WriteLine("Write a category, website, app for what this password is");
        var newPasswordCategory = Console.ReadLine();
        newPassword = $"{newPassword} ({newPasswordCategory})";
        devicePasswords.Add(newPassword);
        fileOperator.Write(filePath, devicePasswords);
    }

    public void Remove(string securePassword, List<string> correctSecurePassword, List<string> devicePasswords, ITextualRepository fileOperator, string filePath)
    {
        do
        {
            Console.WriteLine("To see passwords, at first you have to put your secure password:");
            securePassword = Console.ReadLine();

            if (securePassword == correctSecurePassword[0])
            {
                Console.WriteLine("Your passwords list:");
                int passwordId = 1;
                foreach (var devicePassword in devicePasswords)
                {
                    Console.WriteLine($"{passwordId}. {devicePassword}");
                    passwordId++;
                }
                Console.WriteLine("Type a id of password you want to delete:");
                var passwordToDelete = Console.ReadLine();
                var passwordToDeleteId = int.Parse(passwordToDelete);
                devicePasswords.RemoveAt(passwordToDeleteId - 1);
                fileOperator.Write(filePath, devicePasswords);
                break;
            }
            else if (securePassword != correctSecurePassword[0] && securePassword != "e" && securePassword != "E")
            {
                Console.WriteLine("Password was incorrect. Try again or type E to exit to previous menu.");
            }
        } while (securePassword != "e" && securePassword != "E");
    }
}