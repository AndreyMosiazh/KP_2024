using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OfficeOpenXml;

namespace PharmacyApp
{
    public partial class MainWindow : Window
    {
        private List<Medicine> _medicines = new List<Medicine>();
        private List<Medicine> _filteredMedicines = new List<Medicine>();
        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установите контекст лицензии
            RecordsDataGrid.ItemsSource = _medicines;
            DataContext = this;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            PlaceholderTextBlock.Visibility = string.IsNullOrEmpty(searchText) ? Visibility.Visible : Visibility.Hidden;

            if (string.IsNullOrEmpty(searchText))
            {
                RecordsDataGrid.ItemsSource = _medicines;
                return;
            }

            _filteredMedicines = _medicines.Where(m =>
                m.Name?.ToLower().Contains(searchText) == true ||
                m.Unit?.ToLower().Contains(searchText) == true ||
                m.Price.ToString().Contains(searchText) ||
                m.Quantity.ToString().Contains(searchText) ||
                m.Total.ToString().Contains(searchText) ||
                m.Index.ToString().Contains(searchText)
            ).ToList();

            if (_filteredMedicines.Count == 0)
            {
                MessageBox.Show("Немає збігів у тексті для пошуку.");
            }

            RecordsDataGrid.ItemsSource = _filteredMedicines;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = Visibility.Hidden;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                PlaceholderTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ImportDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открытие диалогового окна для выбора файла
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    Title = "Оберіть файл з базою даних"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    return;
                }

                var filePath = openFileDialog.FileName;

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не знайдено: " + filePath);
                    return;
                }

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        MessageBox.Show("Файл не містить листів: " + filePath);
                        return;
                    }

                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    _medicines.Clear();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string name = worksheet.Cells[row, 2].Text; // Название лекарства
                        string unit = worksheet.Cells[row, 3].Text; // Единица измерения (Уп.)

                        // Преобразование строки с ценой в число с учетом различных форматов
                        string priceStr = worksheet.Cells[row, 4].Text.Replace(",", "."); // Замена запятой на точку для корректного парсинга
                        if (!decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                        {
                            MessageBox.Show($"Помилка у рядку {row}: невірний формат ціни. Значення: '{worksheet.Cells[row, 4].Text}'");
                            continue; // Пропустить эту запись и перейти к следующей
                        }

                        // Преобразование строки с количеством в число
                        string quantityStr = worksheet.Cells[row, 5].Text.Replace(",", "."); // Замена запятой на точку для корректного парсинга
                        if (!decimal.TryParse(quantityStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal quantity))
                        {
                            MessageBox.Show($"Помилка у рядку {row}: невірний формат кількості. Значення: '{worksheet.Cells[row, 5].Text}'");
                            continue; // Пропустить эту запись и перейти к следующей
                        }

                        // Преобразование строки с общей ценой в число с учетом различных форматов
                        string totalStr = worksheet.Cells[row, 6].Text.Replace(",", "."); // Замена запятой на точку для корректного парсинга
                        if (!decimal.TryParse(totalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal total))
                        {
                            MessageBox.Show($"Помилка у рядку {row}: невшрний формат загальної ціни. Значення: '{worksheet.Cells[row, 6].Text}'");
                            continue; // Пропустить эту запись и перейти к следующей
                        }

                        _medicines.Add(new Medicine
                        {
                            Index = row - 1,
                            Name = name,
                            Price = price,
                            Quantity = quantity,
                            Unit = unit,
                            Total = total
                        });
                    }
                }

                if (_medicines.Count == 0)
                {
                    MessageBox.Show("Файл не містить коректних даних.");
                    return;
                }

                RecordsDataGrid.ItemsSource = null;
                RecordsDataGrid.ItemsSource = _medicines;
                MessageBox.Show("База даних успішно імпортована");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка імпорту бази даних: " + ex.Message);
            }
        }

        private void ExportDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открытие диалогового окна для сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    Title = "Зберігти файл як",
                    FileName = "Liki.xlsx" // Предлагаемое имя файла
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Medicines");

                        // Добавление заголовков
                        worksheet.Cells[1, 1].Value = "Index";
                        worksheet.Cells[1, 2].Value = "Name";
                        worksheet.Cells[1, 3].Value = "Unit";
                        worksheet.Cells[1, 4].Value = "Price";
                        worksheet.Cells[1, 5].Value = "Quantity";                       
                        worksheet.Cells[1, 6].Value = "Total";

                        // Добавление данных
                        for (int row = 0; row < _medicines.Count; row++)
                        {
                            var medicine = _medicines[row];
                            worksheet.Cells[row + 2, 1].Value = medicine.Index;
                            worksheet.Cells[row + 2, 2].Value = medicine.Name;
                            worksheet.Cells[row + 2, 3].Value = medicine.Unit;
                            worksheet.Cells[row + 2, 4].Value = medicine.Price;
                            worksheet.Cells[row + 2, 5].Value = medicine.Quantity;                            
                            worksheet.Cells[row + 2, 6].Value = medicine.Total;
                        }

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                    }
                    MessageBox.Show("База даних успішно експортована!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка експорту бази даних: " + ex.Message);
            }
        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            var newRecordWindow = new NewRecordWindow();
            if (newRecordWindow.ShowDialog() == true)
            {
                var newMedicine = newRecordWindow.NewMedicine;
                _medicines.Add(newMedicine);
                RecordsDataGrid.ItemsSource = null;
                RecordsDataGrid.ItemsSource = _medicines;
            }
        }

        private void EditRecord_Click(object sender, RoutedEventArgs e)
        {
            if (RecordsDataGrid.SelectedItem is Medicine selectedMedicine)
            {
                var editRecordWindow = new NewRecordWindow(selectedMedicine);
                if (editRecordWindow.ShowDialog() == true)
                {
                    // Обновление DataGrid после редактирования
                    RecordsDataGrid.ItemsSource = null;
                    RecordsDataGrid.ItemsSource = _medicines;
                }
            }
            else
            {
                MessageBox.Show("Оберіть поле для редагування.");
            }
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_medicines.Count == 0)
            {
                MessageBox.Show("База даних порожня.");
                return;
            }

            if (RecordsDataGrid.SelectedItem is Medicine selectedMedicine)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити {selectedMedicine.Name}?", "Підтвердження видалення", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _medicines.Remove(selectedMedicine);
                    RecordsDataGrid.ItemsSource = null;
                    RecordsDataGrid.ItemsSource = _medicines;
                }
            }
            else
            {
                MessageBox.Show("оберіть запис для видалення.");
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Price).ToList();
                    break;
                case 1:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Quantity).ToList();
                    break;
                case 2:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Total).ToList();
                    break;
                case 3:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Unit).ToList();
                    break;
                case 4:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Name).ToList();
                    break;
                case 5:
                    RecordsDataGrid.ItemsSource = _medicines.OrderBy(m => m.Index).ToList();
                    break;
            }
        }
    }

    public class Medicine
    {
        public int Index { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal Total { get; set; }
    }
}
