using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PharmacyApp
{
    public partial class NewRecordWindow : Window
    {
        public Medicine NewMedicine { get; private set; }
        private Medicine _medicineToEdit;
        private bool _isEditMode;

        public NewRecordWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            this.Title = "Додати новий запис";
        }

        public NewRecordWindow(Medicine medicine)
        {
            InitializeComponent();
            _isEditMode = true;
            _medicineToEdit = medicine;

            IndexTextBox.Text = _medicineToEdit.Index.ToString();
            NameTextBox.Text = _medicineToEdit.Name;
            PriceTextBox.Text = _medicineToEdit.Price.ToString(CultureInfo.InvariantCulture);
            QuantityTextBox.Text = _medicineToEdit.Quantity.ToString();
            UnitTextBox.Text = _medicineToEdit.Unit;
            TotalTextBox.Text = _medicineToEdit.Total.ToString(CultureInfo.InvariantCulture);
            this.Title = "Редагувати запис";
        }

        private void AddOrEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                if (_isEditMode)
                {
                    UpdateMedicine();
                }
                else
                {
                    CreateNewMedicine();
                }

                DialogResult = true;
                Close();
            }
        }

        private bool ValidateInputs()
        {
            if (!int.TryParse(IndexTextBox.Text, out int index) || index < 0)
            {
                MessageBox.Show("Індекс повинен бути не від'ємним цілим числом.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введіть назву.");
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Ціна повинна бути не від'ємним числом.");
                return false;
            }

            if (!decimal.TryParse(QuantityTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity) || quantity < 0)
            {
                MessageBox.Show("Кількість повинна бути не від'ємним числом.");
                return false;
            }

            if (!Regex.IsMatch(UnitTextBox.Text, @"^[А-Яа-яЁёІіЇїЄєҐґ.,]+$"))
            {
                MessageBox.Show("Одиниця виміру може містити тільки букви українського алфавіту, крапку та кому.");
                return false;
            }

            return true;
        }

        private void UpdateMedicine()
        {
            _medicineToEdit.Index = int.Parse(IndexTextBox.Text);
            _medicineToEdit.Name = NameTextBox.Text;
            _medicineToEdit.Price = decimal.Parse(PriceTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            _medicineToEdit.Quantity = decimal.Parse(QuantityTextBox.Text);
            _medicineToEdit.Unit = UnitTextBox.Text;
            _medicineToEdit.Total = _medicineToEdit.Price * _medicineToEdit.Quantity;
        }

        private void CreateNewMedicine()
        {
            NewMedicine = new Medicine
            {
                Index = int.Parse(IndexTextBox.Text),
                Name = NameTextBox.Text,
                Price = decimal.Parse(PriceTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                Quantity = decimal.Parse(QuantityTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                Unit = UnitTextBox.Text,
                Total = decimal.Parse(PriceTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture) * 
                decimal.Parse(QuantityTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture)
            };
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            if (decimal.TryParse(PriceTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) &&
                decimal.TryParse(QuantityTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
            {
                decimal total = price * quantity;
                TotalTextBox.Text = total.ToString("0.##", CultureInfo.InvariantCulture); // Output with two decimal places
            }
            else
            {
                TotalTextBox.Text = string.Empty; // Reset value if inputs are incorrect
            }
        }
    }
}
