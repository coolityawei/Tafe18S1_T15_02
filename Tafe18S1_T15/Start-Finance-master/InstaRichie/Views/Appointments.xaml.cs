﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using StartFinance.ViewModels;
using Windows.UI.Popups;
using SQLite.Net;

// 1 The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLite.Net.SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            //DateStamp.Date = DateTime.Now; // gets current date and time
            //DateStamp1.Date = DateTime.Now;
            Results();
        }

        public void Results()
        {
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            ApptList1.ItemsSource = query.ToList();
            ApptList.ItemsSource = query.ToList();

            conn.CreateTable<Accounts>();
            var accupdate = conn.Table<Accounts>();
            AccountSelct.ItemsSource = accupdate.ToList();
        }

        private async void AddData(object sender, RoutedEventArgs e)
        {
            Checkout nnn = new Checkout();
            try
            {
                //string CDay = DateStamp.Date.Value.Day.ToString();
                //string CMonth = DateStamp.Date.Value.Month.ToString();
                //string CYear = DateStamp.Date.Value.Year.ToString();
                //string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;

                if (Desc.Text == "")
                {
                    MessageDialog dialog = new MessageDialog("Not entered Debt name");
                    await dialog.ShowAsync();
                }
                else
                {
                    double Money = Convert.ToDouble(MoneyIn.Text);
                    double Dmoney = 0 - Money;
                    conn.Insert(new Appointments()
                    {
                        ApptName = Desc.Text,
                        DebtAmount = Dmoney
                    });
                }
                Results();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Debt Name already exists, try different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection = ((Appointments)ApptList1.SelectedItem).ID;
                if (AccSelection == 0)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    var query3 = conn.Query<Appointments>("DELETE FROM Appointments WHERE ID ='" + AccSelection + "'");
                    ApptList1.ItemsSource = query1.ToList();
                }

                conn.CreateTable<Appointments>();
                var query = conn.Table<Appointments>();
                ApptList1.ItemsSource = query.ToList();

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void PayDebt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Checkout nnn = new Checkout();
                //string CDay = DateStamp1.Date.Value.Day.ToString();
                //string CMonth = DateStamp1.Date.Value.Month.ToString();
                //string CYear = DateStamp1.Date.Value.Year.ToString();
                //string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;
                double Money = Convert.ToDouble(MoneyIn1.Text);
                double DebtBalance = nnn.DebtCalculation();
                double Dmoney = DebtBalance + Money;

                string AccountSelection = ((Accounts)AccountSelct.SelectedItem).AccountName;
                if (AccountSelection.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Account Selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (AccountBalance() < Money)
                {
                    MessageDialog dialog = new MessageDialog("You're spending more than what you have", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (Dmoney > 0)
                {
                    MessageDialog dialog = new MessageDialog("You can't pay more than the Debt Amount : " + DebtBalance + "", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Appointments
                    {
                        ApptName = Desc1.Text,
                        DebtAmount = Money
                    });
                    double FinalAmount = AccountBalance() - Money;
                    var query3 = conn.Query<Accounts>("UPDATE Accounts SET InitialAmount = " + FinalAmount + " WHERE AccountName ='" + AccountSelection + "'");
                    MessageDialog Confirmed = new MessageDialog("Appointment Paid successfully");
                    await Confirmed.ShowAsync();
                    Results();

                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is NullReferenceException)
                {
                    MessageDialog dialog = new MessageDialog("Please enter the Appointment Details", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }
        public double AccountBalance()
        {
            string AccountSelection = ((Accounts)AccountSelct.SelectedItem).AccountName;
            conn.CreateTable<Accounts>();
            var query12 = conn.Query<Accounts>("SELECT * FROM Accounts WHERE AccountName ='" + AccountSelection + "'");
            var sumProd = query12.AsEnumerable().Sum(o => o.InitialAmount);
            double Total = sumProd;
            return Total;
        }

        private void DebtPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int no = DebtPivot.SelectedIndex;
            if (no == 0)
            {
                AddDebtFooter.Visibility = Visibility.Visible;
                PayDebtFooter.Visibility = Visibility.Collapsed;
                conn.CreateTable<Appointments>();
                var query = conn.Table<Appointments>();
                ApptList1.ItemsSource = query.ToList();
            }
            else
            {
                PayDebtFooter.Visibility = Visibility.Visible;
                AddDebtFooter.Visibility = Visibility.Collapsed;
                conn.CreateTable<Appointments>();
                var query = conn.Table<Appointments>();
                ApptList.ItemsSource = query.ToList();
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection = ((Appointments)ApptList.SelectedItem).ID;
                if (AccSelection == 0)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    var query3 = conn.Query<Appointments>("DELETE FROM Appointments WHERE ID ='" + AccSelection + "'");
                    ApptList.ItemsSource = query1.ToList();
                }

                conn.CreateTable<Appointments>();
                var query = conn.Table<Appointments>();
                ApptList.ItemsSource = query.ToList();

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }
    }
}
