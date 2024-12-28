using WPFGameShop.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFGameShop.Models;
using WPFGameShop.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace WPFGameShop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel() 
        {
            Tabs = new ObservableCollection<TabItem>();
            
        }

        #region properties
        private string _userName;

        public ObservableCollection<TabItem> Tabs { get; set; }


        #endregion

        #region accessors
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                onPropertyChanged(nameof(UserName));
            }
        }

        #endregion

        #region commands
        private ICommand logInCommand;
        public ICommand LogInCommand
        {
            get
            {
                if (logInCommand == null)
                    logInCommand = new AsyncRelayCommand(
                        parameter => LogIn(parameter), 
                        predicate => !GetPassword(predicate).IsNullOrEmpty() && !UserName.IsNullOrEmpty()
                        );
                
                return logInCommand;
            }
        }

        private ICommand openRegistrationWindowCommand;
        public ICommand OpenRegistrationWindowCommand
        {
            get
            {
                if(openRegistrationWindowCommand == null)
                    openRegistrationWindowCommand = new RelayCommand(
                       parameter => OpenRegistrationWindow(),
                       predicate => true
                       ); 
                return openRegistrationWindowCommand;            
            }
        }
        #endregion

        #region functions
        //
        // hasla nie przechowujemy w zmiennych dlatego jest returnowane tylko gdy jest potrzebne bo siedzi zaszyfrowane w passwordboxie
        //
        private string GetPassword(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                return passwordBox.Password.ToString();
            }
            return "";
        }

        private void SetPassword(object parameter, string text)
        {
            if (parameter is PasswordBox passwordBox)
            {
                passwordBox.Password = text;
            }
        }

        private async Task LogIn(object parameter)
        {
            User newUser = new User { Name = _userName, Password = GetPassword(parameter) };
            newUser.Password = newUser.GetHashPassword();
            var foundUser = await RepositoryUser.FindUserInDB(newUser);
            if (foundUser)
            {               
                var currentUser = await RepositoryUser.GetUserFromDb(newUser);
                var homeVM = new HomeViewModel(currentUser);
                var shopVM = new ShopViewModel(currentUser, ref homeVM);
                Tabs.Add(new TabItem { Header = "Home", Content = new HomeView(), DataContext = homeVM});
                Tabs.Add(new TabItem { Header = "Shop", Content = new ShopView(), DataContext = shopVM});
                Tabs.Add(new TabItem { Header = "Reviews", Content = new ReviewsView(), DataContext = new ReviewsViewModel(currentUser) });
                Tabs.Add(new TabItem { Header = "Profile", Content = new ProfileView(), DataContext = new ProfileViewModel(currentUser, Tabs) });     

                if (currentUser.Name == "filip")
                {
                    Tabs.Add(new TabItem { Header = "GameAdd", Content = new GameAddView(), DataContext = new GameAddViewModel(ref shopVM) });
                }

            }
            else
            {
                MessageBox.Show("Credentials not found");
               
            }

            UserName = string.Empty;
            SetPassword(parameter, string.Empty);
        }

        private void OpenRegistrationWindow()
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
        }
  
        #endregion


    }
}
