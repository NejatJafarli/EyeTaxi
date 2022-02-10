using Esri.ArcGISRuntime.Geometry;
using EyeTaxi.Command;
using EyeTaxi.Models;
using EyeTaxiAdmin.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PasswordBox = HandyControl.Controls.PasswordBox;

namespace EyeTaxiAdmin.ViewModel
{
    public class AdminLoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public static AdminLoginView MyView { get; set; }
        private string usernameText;

        public string UsernameText
        {
            get { return usernameText; }
            set { usernameText = value; OnPropertyRaised(); }
        }

        public PasswordBox PasswordBoxText { get; set; }

        public RelayCommand CloseButtonClickCommand { get; set; }
        public RelayCommand LoadPasswordBox { get; set; }
        public RelayCommand ClosingCommand { get; set; }
        public RelayCommand LoginBtnClickCommand { get; set; }

        public AdminLoginViewModel()
        {

            ClosingCommand = new RelayCommand(s =>
            {
                System.Environment.Exit(0);
            });

            CloseButtonClickCommand = new RelayCommand(s =>
            {
                if (s is Window window)
                {
                    window.Close();
                }
            });

            LoadPasswordBox = new RelayCommand(s =>
            {
                PasswordBoxText = s as PasswordBox;
            });

            LoginBtnClickCommand = new RelayCommand(s =>
            {
                if (!string.IsNullOrWhiteSpace(UsernameText))
                {
                    if (!string.IsNullOrWhiteSpace(PasswordBoxText.Password))
                    {
                        var temp1 = ConfigurationManager.AppSettings["AdminUsername"];
                        var temp2 = ConfigurationManager.AppSettings["AdminPassword"];
                        if (UsernameText == temp1  &&
                        PasswordBoxText.Password == temp2
                        )
                        {
                            //throw Growly Notification SuccessGlobal  HandyControl Entered
                            HandyControl.Controls.Growl.SuccessGlobal("Success Entered Admin Panel");

                            UsernameText = "";
                            PasswordBoxText.Password = "";

                            MyView.Hide();
                            var AdminPanel = new AdminPanelView();
                            AdminPanel.ShowDialog();
                            MyView.Show();

                        }
                        else
                            //throw Growly Notification WarningGlobal  HandyControl Username Or Password Wrong
                            HandyControl.Controls.Growl.WarningGlobal("Username Or Password Wrong");
                    }
                    else
                        //throw Growly Notification WarningGlobal  HandyControl Password is Empty
                        HandyControl.Controls.Growl.WarningGlobal("Password is Empty");
                }
                else
                    //throw Growly Notification WarningGlobal  HandyControl Username is Empty
                    HandyControl.Controls.Growl.WarningGlobal("Username is Empty");
            });

        }

    }
}
