using EyeTaxi.Command;
using EyeTaxi.Models;
using EyeTaxi.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PasswordBox = HandyControl.Controls.PasswordBox;

namespace EyeTaxi.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        public RelayCommand LogBtnClickCommand { get; set; }
        public RelayCommand LoginBtnClickCommand { get; set; }
        public RelayCommand CloseButtonClickCommand { get; set; }
        public RelayCommand LogPassCommand { get; set; }
        public List<User> Users { get; set; } = JsonSerializer.Deserialize<List<User>>(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Users.json"));


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private Visibility logViewVisibility = Visibility.Visible;

        public Visibility LogViewVisibility
        {
            get { return logViewVisibility; }
            set { logViewVisibility = value; OnPropertyRaised(); }
        }

        #region Register Fields

        private Visibility regViewVisibility = Visibility.Collapsed;

        public Visibility RegViewVisibility
        {
            get { return regViewVisibility; }
            set { regViewVisibility = value; OnPropertyRaised(); }
        }

        public RelayCommand RegBtnClickCommand { get; set; }
        public RelayCommand RegisterButtonClickedCommand { get; set; }
        public RelayCommand RegPassChangedCommand { get; set; }
        public RelayCommand RegConfirmPassChangedCommand { get; set; }
        public RelayCommand RegPasswordLoadedCommand { get; set; }
        public RelayCommand RegConfirmPasswordLoadedCommand { get; set; }


        private string _emailText = "";

        public string EmailText
        {
            get { return _emailText; }
            set { _emailText = value; OnPropertyRaised(); }
        }



        private string _regUsernameText = "";

        public string RegUsernameText
        {
            get { return _regUsernameText; }
            set { _regUsernameText = value; OnPropertyRaised(); }
        }

        private string _regPassText = "";



        public HandyControl.Controls.PasswordBox RegPasswordBox { get; set; }
        public HandyControl.Controls.PasswordBox RegConfirmPasswordBox { get; set; }

        #endregion Register Fields

        private string _logUsernameText = "";

        public string LogUsernameText
        {
            get { return _logUsernameText; }
            set { _logUsernameText = value; OnPropertyRaised(); }
        }

        public PasswordBox LogPasswordBox { get; set; }

        public static User CurrentUserLoggedIn;

        public static RegistrationView MyView { get; set; }
        public RegistrationViewModel()
        {

            CloseButtonClickCommand = new RelayCommand(s =>
            {
                if (s is Window window)
                {
                    window.Close();
                }
            });
            #region Register 

            LogBtnClickCommand = new RelayCommand(s =>
            {
                //var TextJson = JsonSerializer.Serialize("String", new JsonSerializerOptions() { WriteIndented = true });
                //File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Users.json");

                RegViewVisibility = Visibility.Collapsed;
                LogViewVisibility = Visibility.Visible;
            });
            RegBtnClickCommand = new RelayCommand(s =>
            {
                RegViewVisibility = Visibility.Visible;
                LogViewVisibility = Visibility.Collapsed;
            });
            RegConfirmPasswordLoadedCommand = new RelayCommand(s =>
            {
                RegConfirmPasswordBox = s as PasswordBox;
            });

            RegPasswordLoadedCommand = new RelayCommand(s =>
            {
                RegPasswordBox = s as PasswordBox;
            });

            RegisterButtonClickedCommand = new RelayCommand(s =>
            {
                if (RegUsernameText.Length >= 5)
                {
                    if (RegPasswordBox.Password.Length >= 5)
                    {
                        if (RegConfirmPasswordBox.Password == RegPasswordBox.Password)
                        {
                            if (MailAddress(EmailText))
                            {
                                bool UserNameAlReadyHave = false;

                                //Searching Json File For Same Username Already Have?
                                for (int i = 0; i < Users.Count; i++)
                                {

                                    if (Users[i].Username == RegUsernameText)
                                    {
                                        UserNameAlReadyHave = true;
                                        break;
                                    }
                                }

                                if (!UserNameAlReadyHave)
                                {
                                    var NewUser = new User(RegUsernameText, RegPasswordBox.Password, EmailText);
                                    Users.Add(NewUser);

                                    var TextJson = JsonSerializer.Serialize(Users, new JsonSerializerOptions() { WriteIndented = true });
                                    File.WriteAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Users.json", TextJson);

                                    LogUsernameText = RegUsernameText;
                                    LogPasswordBox.Password = "";

                                    RegUsernameText = "";
                                    RegPasswordBox.Password = "";
                                    RegConfirmPasswordBox.Password = "";

                                    RegConfirmPasswordBox.Password = "";
                                    RegPasswordBox.Password = "";


                                    EmailText = "";

                                    LogViewVisibility = Visibility.Visible;
                                    RegViewVisibility = Visibility.Collapsed;


                                    //throw Growly Notification Succes  HandyControl Your Account Has Been Created
                                    HandyControl.Controls.Growl.SuccessGlobal("Your Account Has Been Created");
                                }
                                else
                                    //throw Growly Notification WarningGlobal HandyControl Your Username AlReady Have
                                    HandyControl.Controls.Growl.WarningGlobal("Your Username already Have");
                            }
                            else
                                //throw Growly Notification WarningGlobal HandyControl Email Is Not Correct
                                HandyControl.Controls.Growl.WarningGlobal("Email Is Not Correct");
                        }
                        else
                            //throw Growly Notification WarningGlobal HandyControl Passwords are not the same
                            HandyControl.Controls.Growl.WarningGlobal("Passwords are not the same");
                    }
                    else
                        //throw Growly Notification WarningGlobal HandyControl Your Password Length Must be Greater 5
                        HandyControl.Controls.Growl.WarningGlobal("Your Password Length Must be Greater 5");
                }
                else
                    //throw Growly Notification WarningGlobal HandyControl Your Username Length Must be Greater 5
                    HandyControl.Controls.Growl.WarningGlobal("Your Username Length Must be Greater 5");

            });
            #endregion

            LogPassCommand = new RelayCommand(s =>
            {
                LogPasswordBox = s as PasswordBox;
            });
            LoginBtnClickCommand = new RelayCommand(s =>
            {

                if (!string.IsNullOrWhiteSpace(LogUsernameText))
                {

                    if (!string.IsNullOrWhiteSpace(LogPasswordBox.Password))
                    {

                        int IndexHolder = -1;
                        for (int i = 0; i < Users.Count; i++)
                        {
                            if (Users[i].Username == LogUsernameText)
                            {
                                IndexHolder = i;
                                break;
                            }
                        }
                        if (IndexHolder != -1)
                        {
                            if (Users[IndexHolder].Password == LogPasswordBox.Password)
                            {
                                HandyControl.Controls.Growl.SuccessGlobal($"Welcome {Users[IndexHolder].Username}");
                                //Create here New Window Example
                                CurrentUserLoggedIn = Users[IndexHolder];
                                NavigateRouteViewModel.CurrentUser=CurrentUserLoggedIn;
                                var NewWindow = new NavigateRouteView();

                                MyView.Hide();

                                LogUsernameText = "";
                                LogPasswordBox.Password = "";

                                NewWindow.ShowDialog();
                                MyView.Show();
                            }
                            else
                                //throw Growly Notification WarningGlobal  HandyControl Username Or Password Is Wrong
                                HandyControl.Controls.Growl.WarningGlobal("Username Or Password Is Wrong");
                        }
                        else
                            //throw Growly Notification WarningGlobal  HandyControl Username Or Password Is Wrong
                            HandyControl.Controls.Growl.WarningGlobal("Username Or Password Is Wrong");
                    }
                    else
                        //throw Growly Notification WarningGlobal  HandyControl Password Is Empty 
                        HandyControl.Controls.Growl.WarningGlobal("Password Is Empty ");
                }
                else
                    //throw Growly Notification WarningGlobal  HandyControl Username Is Empty 
                    HandyControl.Controls.Growl.WarningGlobal("Username Is Empty");
            });

        }
        bool MailAddress(string mail)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
