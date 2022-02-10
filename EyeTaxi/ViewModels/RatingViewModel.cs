using EyeTaxi.Command;
using EyeTaxi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EyeTaxi.ViewModels
{
    public class RatingViewModel : Window
    {
        public RelayCommand DoneButtonCommand { get; set; }
        static public Driver SelectedDriver { get; set; }


        public int RatingValue
        {
            get { return (int)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int), typeof(RatingViewModel));



        public string CommentText
        {
            get { return (string)GetValue(CommentTextProperty); }
            set { SetValue(CommentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentTextProperty =
            DependencyProperty.Register("CommentText", typeof(string), typeof(RatingViewModel));


        public RatingViewModel()
        {
            DoneButtonCommand = new RelayCommand(s =>
            {
                if (s is Window window)
                {
                    SelectedDriver.TotalRating = SelectedDriver.TotalRating + RatingValue;
                    SelectedDriver.Rating = SelectedDriver.TotalRating / SelectedDriver.CountTravel;
                    RatingValue = 0;
                    CommentText = "";
                    window.Close();
                }

            });
        }

    }
}
