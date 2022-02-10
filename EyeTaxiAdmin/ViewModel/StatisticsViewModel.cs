using EyeTaxi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace EyeTaxiAdmin.ViewModel
{
    public class StatisticsViewModel:Window
    {
        public ObservableCollection<Driver> Drivers { get; set; } = JsonSerializer.Deserialize<ObservableCollection<Driver>>(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Drivers.json"));




        public double AllIncomeDrivers
        {
            get { return (double)GetValue(AllIncomeDriversProperty); }
            set { SetValue(AllIncomeDriversProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllIncomeDrivers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllIncomeDriversProperty =
            DependencyProperty.Register("AllIncomeDrivers", typeof(double), typeof(StatisticsViewModel));





        public double AllBenefitDriver
        {
            get { return (double)GetValue(AllBenefitDriverProperty); }
            set { SetValue(AllBenefitDriverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllBenefitDriver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllBenefitDriverProperty =
            DependencyProperty.Register("AllBenefitDriver", typeof(double), typeof(StatisticsViewModel));



        public double AllBenefitCompany
        {
            get { return (double)GetValue(AllBenefitCompanyProperty); }
            set { SetValue(AllBenefitCompanyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllBenefitCompany.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllBenefitCompanyProperty =
            DependencyProperty.Register("AllBenefitCompany", typeof(double), typeof(StatisticsViewModel));


        private double SortDouble(double value)
        {
            var temp = value;
            //1.55636
            var temp2 = temp - (int)temp;
            //0.55636
            temp2 = temp2 * 100;
            //55.5636
            temp2 = (int)temp2;
            //55
            temp2 = temp2 / 100;
            //0.55

            temp = (int)temp;
            //1

            return temp + temp2;
        }

        public StatisticsViewModel()
        {
            foreach (var item in Drivers)
            {
                AllIncomeDrivers += item.Balance;
                AllBenefitDriver += item.DriverBenefit;
                AllBenefitCompany += item.CompanyBenefit;
            }
        
            AllIncomeDrivers = SortDouble(AllIncomeDrivers);
            AllBenefitDriver = SortDouble(AllBenefitDriver);
            AllBenefitCompany = SortDouble(AllBenefitCompany);

        }

    }
}
