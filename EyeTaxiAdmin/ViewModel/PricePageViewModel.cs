using EyeTaxi.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EyeTaxiAdmin.ViewModel
{
    public class PricePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public RelayCommand UpdateButtonClickCommand { get; set; }
        private string _priceText;

        public string PriceText
        {
            get { return _priceText; }
            set { _priceText = value; OnPropertyRaised(); }
        }

        private string _textBoxText;

        public string TextBoxText
        {
            get { return _textBoxText; }
            set { _textBoxText = value; OnPropertyRaised(); }
        }




        public PricePageViewModel()
        {
            PriceText = File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\PricePer-KM.json");

            UpdateButtonClickCommand = new RelayCommand(s =>
            {
                if (!string.IsNullOrWhiteSpace(TextBoxText))
                    if (double.TryParse(TextBoxText, out double result))
                    {
                        File.WriteAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\PricePer-KM.json", TextBoxText);
                        PriceText = TextBoxText;
                        HandyControl.Controls.Growl.SuccessGlobal("Price Per KM Value Changed");
                    }
            });
        }

    }
}
