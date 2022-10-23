using ClientApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class RegisterEmployeeViewModel : ViewModelBase
    {
        RegisterEmployeeView _registerEmployeeView;
        public RegisterEmployeeViewModel(RegisterEmployeeView registerEmployeeView)
        {
            _registerEmployeeView = registerEmployeeView;
        }

        private string _employeeFirstName = string.Empty;

        public string EmployeeFirstName
        {

            get
            {
                return _employeeFirstName;
            }

            set
            {
                _employeeFirstName = value;
                OnPropertyChanged(nameof(EmployeeFirstName));

            }
        }

        private string _employeeLastName = string.Empty;

        public string EmployeeLastName
        {

            get
            {
                return _employeeLastName;
            }

            set
            {
                _employeeLastName = value;
                OnPropertyChanged(nameof(EmployeeLastName));

            }
        }

        private string _employeeInsuranceCompany = string.Empty;

        public string EmployeeInsuranceCompany
        {

            get
            {
                return _employeeInsuranceCompany;
            }

            set
            {
                _employeeInsuranceCompany = value;
                OnPropertyChanged(nameof(EmployeeInsuranceCompany));

            }
        }

        private string _employeePhoneNumber = string.Empty;

        public string EmployeePhoneNumber
        {

            get
            {
                return _employeePhoneNumber;
            }

            set
            {
                _employeePhoneNumber = value;
                OnPropertyChanged(nameof(EmployeePhoneNumber));

            }
        }

        private string _employeeEmail = string.Empty;

        public string EmployeeEmail
        {

            get
            {
                return _employeeEmail;
            }

            set
            {
                _employeeEmail = value;
                OnPropertyChanged(nameof(EmployeeEmail));

            }
        }

        public void EmployeeRegisterCommand()
        {
            new AdminHomeView().Show();
            _registerEmployeeView.Close();
        }
    }
}
