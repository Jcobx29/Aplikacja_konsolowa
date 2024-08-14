public class DeviceTemplate
{

    public void UseOfDevice(string filePath, int optionDevice, string securePassword, List<string> correctSecurePassword, List<string> devicePasswords, ITextualRepository fileOperator)
    {
        var device = new DeviceUsing();
        do
        {
            Console.WriteLine("What do you want to do with your passwords: ");
            Console.WriteLine("1. Show all passwords");
            Console.WriteLine("2. Add new password");
            Console.WriteLine("3. Remove password");
            Console.WriteLine("4. Exit to previous page");

            var chooseDevice = Console.ReadLine();
            var IsParsableDevice = int.TryParse(chooseDevice, out optionDevice);
            Console.WriteLine();
            if (IsParsableDevice)
            {
                if (optionDevice == 1)
                {
                    device.Show(securePassword, correctSecurePassword, devicePasswords);
                }
                else if (optionDevice == 2)
                {
                    device.Add(devicePasswords, fileOperator, filePath);
                }
                else if (optionDevice == 3)
                {
                    device.Remove(securePassword, correctSecurePassword, devicePasswords, fileOperator, filePath);
                }
                else
                {
                    Console.WriteLine("You have put wrong option. Try again.");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("You have put wrong option. Try again.");
                Console.WriteLine();
            }

        } while (optionDevice != 4);

    }
}

