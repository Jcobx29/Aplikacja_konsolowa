public class SecurePassword
{
    public void SecurePasswordChange(ITextualRepository fileOperator, string filePath)
    {
        var securePassword = fileOperator.Read(filePath);

        Console.WriteLine(@"To change your secure password, first you have to
type present secure password:");
        string securePasswordByUser;
        do
        {
            securePasswordByUser = Console.ReadLine();
            if (securePasswordByUser == securePassword[0])
            {
                Console.WriteLine("Password is correct!");
                Console.WriteLine("Type your new secure password:");
                var newSecurePassword = Console.ReadLine();
                securePassword[0] = newSecurePassword;
                fileOperator.Write(filePath, securePassword);
                break;
            }
            else if (securePasswordByUser != securePassword[0] && securePasswordByUser != "E" && securePasswordByUser != "e")
            {
                Console.WriteLine("You put wrong password");
                Console.WriteLine($"Try again or press E to back to previous menu");
            }
        } while (securePasswordByUser != "E" && securePasswordByUser != "e");
    }
}