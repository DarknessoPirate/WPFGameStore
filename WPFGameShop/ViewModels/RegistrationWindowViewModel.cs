using Microsoft.IdentityModel.Tokens;
using WPFGameShop.Models;
using WPFGameShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Mail;
using System.Globalization;

namespace WPFGameShop.ViewModels
{
    public class RegistrationWindowViewModel :ViewModelBase
    {
        private const int _minUsernameLen = 5;

        #region private properties

        private string _userEmail;
        private int _isEmailVaild;
        private string _userName;
        private bool _isUserNameValid;
        private string _userMoney;
        private bool _isMoneyValid;
        #endregion

        #region accessors
        public string UserMoney
        {
            get { return _userMoney; }
            set
            {
                _userMoney = value;
                onPropertyChanged(nameof(UserMoney));
                ValidateMoney();
            }
        }

        public bool IsMoneyValid
        {
            get { return _isMoneyValid; }
            set
            {
                _isMoneyValid = value;
                onPropertyChanged(nameof(IsMoneyValid));
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                onPropertyChanged(nameof(UserName));
                ValidateUserName();
            }
        }

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                onPropertyChanged(nameof(UserEmail));
                ValidateEmail();
            }
        }

        public int IsEmailValid
        {
            get { return _isEmailVaild; }
            set
            {
                _isEmailVaild = value;
                onPropertyChanged(nameof(IsEmailValid));
            }
        }

        public bool IsUsernameValid
        {
            get { return _isUserNameValid; }
            set
            {
                _isUserNameValid = value;
                onPropertyChanged(nameof(IsUsernameValid));
            }
        }


        #endregion

        #region commands
        private ICommand registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                    registerCommand = new RelayCommand(
                        parameter => Register(parameter),
                        predicate => !GetPassword(predicate).IsNullOrEmpty() && !UserName.IsNullOrEmpty() && IsEmailValid == 1 && IsUsernameValid && IsMoneyValid
                        );

                return registerCommand;
            }
        }
        #endregion

        #region functions
        private string GetPassword(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                return passwordBox.Password.ToString();
            }
            return "";
        }

        private void Register(object parameter)
        {
            try
            {
                var decimalMoney = new Decimal();
                if(!Decimal.TryParse(_userMoney.Replace(",","."),NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out decimalMoney))
                {
                    MessageBox.Show("Incorrect money format");
                }
                
                var newUser = new User { Name = _userName, Password = GetPassword(parameter), Email = _userEmail, Money = decimalMoney};
                newUser.Password = newUser.GetHashPassword();
                if (RepositoryUser.AddUserToDb(newUser))
                    MessageBox.Show("Account created!");
                OnRequestClose();
            }catch(Microsoft.EntityFrameworkCore.DbUpdateException exception)
            {

                if(exception.InnerException!= null)
                {
                    var innerException = exception.InnerException.Message;
                    
                    if (innerException.Contains("IX_Users_Name"))
                    {
                        MessageBox.Show("Username is already taken");
                    }
                    else if (innerException.Contains("IX_Users_Email"))
                    {
                        MessageBox.Show("Email already taken");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {innerException}");
                    }
                }
                else
                {
                    MessageBox.Show("Database error");
                }
            }
            
           
        }

        private void ValidateEmail()
        {
            try
            {
                var address = new MailAddress(UserEmail).Address;
                // Regular expression to enforce more strict email validation
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (Regex.IsMatch(UserEmail, pattern))
                {
                    IsEmailValid = 1;
                }
                else
                {
                    IsEmailValid = -1;
                }
            }
            catch (FormatException)
            {
                if (UserEmail == null)
                {
                    IsEmailValid = 0;
                }
                else
                {
                    IsEmailValid = -1;
                }
            }
            catch (ArgumentException)
            {
                IsEmailValid = -1;
            }

        }

        private void ValidateUserName()
        {
            try
            {
                string pattern = @"^\w{" + _minUsernameLen + ",}$";
                IsUsernameValid = Regex.IsMatch(UserName, pattern);
            }
            catch
            {
                IsUsernameValid = false;
            }
        }

        private void ValidateMoney()
        {
            try
            {
                string pattern = @"^\d{1,6}([.,]\d{1,2})?$";
                IsMoneyValid = Regex.IsMatch(UserMoney, pattern);
            }
            catch
            {
                IsMoneyValid = false;
            }
           
        }

        #endregion

        #region events
        public event EventHandler RequestClose;

        protected void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        #endregion

    }
}
