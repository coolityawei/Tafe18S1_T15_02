using System;
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
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            conn.CreateTable<PersonalInfo>();
            var query1 = conn.Table<PersonalInfo>();
            PersonalListView.ItemsSource = query1.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AddData_Click(object sender, RoutedEventArgs e)
        {
            string CDay = dpDOB.Date.Day.ToString();
            string CMonth = dpDOB.Date.Month.ToString();
            string CYear = dpDOB.Date.Year.ToString();
            string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;
            try
            {
                if (tbFirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No First Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (tbLastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Last Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (tbEmail.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Gender cannot null", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (tbPhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Phone Number cannot null", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new PersonalInfo()
                    {

                        FirstName = tbFirstName.Text,
                        LastName = tbLastName.Text,
                        Gender = tbGender.Text,
                        DateOfBirth = FinalDate,
                        Email = tbEmail.Text,
                        PhoneNumber = tbPhone.Text

                    });
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
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }

        }
        private async void DeleteData_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Account will delete all Personal info of this account", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    int ID = ((PersonalInfo)PersonalListView.SelectedItem).ID;

                    // Delete in sql
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE ID ==" + ID);
                    Results();
                    //conn.CreateTable<PersonalInfo>();
                    //var querytable = conn.Query<PersonalInfo>("DELETE FROM Transactions WHERE Account='" + AccountsLabel + "'");
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }
    }
}

