string choose = "";
int option = 0;
int optionPhone = 0;
int optionDevice = 0;
int optionComputer = 0;
int optionOthers = 0;
var deviceTemplate = new DeviceTemplate();
var securePasswordCheck = new SecurePassword();
var fileOperator = new StringsTextualRepository();
var fileOperator2 = new StringsJsonRepository();
var correctSecurePassword = fileOperator.Read("securePassword.txt");
string securePassword = "";
var App = new App();

App.Run(choose, option, optionDevice, fileOperator2, deviceTemplate, optionPhone, optionComputer, optionOthers, securePassword, correctSecurePassword, securePasswordCheck);