using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace butgem
{
    public partial class BudgetForm : Form
    {
        private BudgetManager budgetManager;
        private TextBox descriptionTextBox;
        private TextBox amountTextBox;
        private ComboBox typeComboBox;
        private DateTimePicker datePicker;
        private Button addButton;
        private Button removeButton;
        private Button updateButton;
        private ListBox transactionsListBox;
        private Label totalLabel;

        public BudgetForm()
        {
            this.Text = "Управление бюджетом";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            descriptionTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Width = 150
            };

            amountTextBox = new TextBox
            {
                Location = new Point(170, 10),
                Width = 100
            };

            typeComboBox = new ComboBox
            {
                Location = new Point(280, 10),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            typeComboBox.Items.Add("Доход");
            typeComboBox.Items.Add("Расход");
            typeComboBox.SelectedIndex = 0;

            datePicker = new DateTimePicker
            {
                Location = new Point(390, 10),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            addButton = new Button
            {
                Location = new Point(10, 40),
                Text = "Добавить",
                Width = 100,
                BackColor = Color.LightGreen
            };
            addButton.Click += (s, e) => AddTransaction();

            removeButton = new Button
            {
                Location = new Point(120, 40),
                Text = "Удалить",
                Width = 100,
                BackColor = Color.LightCoral
            };
            removeButton.Click += (s, e) => RemoveTransaction();

            updateButton = new Button
            {
                Location = new Point(230, 40),
                Text = "Обновить",
                Width = 100,
                BackColor = Color.LightBlue
            };
            updateButton.Click += (s, e) => UpdateTransaction();

            transactionsListBox = new ListBox
            {
                Location = new Point(10, 80),
                Width = 560,
                Height = 250
            };

            totalLabel = new Label
            {
                Location = new Point(10, 340),
                Width = 300,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = "Общий бюджет: 0 руб."
            };

            this.Controls.AddRange(new Control[] {
                descriptionTextBox, amountTextBox, typeComboBox, datePicker,
                addButton, removeButton, updateButton,
                transactionsListBox, totalLabel
            });

            budgetManager = new BudgetManager();
            RefreshList();
        }

        private void RefreshList()
        {
            transactionsListBox.Items.Clear();
            foreach (var t in budgetManager.Transactions)
            {
                string sign = t.Type == TransactionType.Доход ? "+" : "-";
                transactionsListBox.Items.Add($"{t.Description} | {sign}{t.Amount} руб. | {t.Date:dd.MM.yyyy}");
            }
            totalLabel.Text = $"Общий бюджет: {budgetManager.TotalBudget} руб.";
            totalLabel.ForeColor = budgetManager.TotalBudget >= 0 ? Color.Green : Color.Red;
        }

        private void AddTransaction()
        {
            if (!ValidateInput(out decimal amount, out TransactionType type)) return;
            budgetManager.AddTransaction(new Transaction(descriptionTextBox.Text, amount, type, datePicker.Value));
            RefreshList();
            ClearFields();
            MessageBox.Show("Транзакция добавлена!");
        }

        private void RemoveTransaction()
        {
            if (transactionsListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите транзакцию!");
                return;
            }
            budgetManager.RemoveTransaction(budgetManager.Transactions[transactionsListBox.SelectedIndex]);
            RefreshList();
            MessageBox.Show("Транзакция удалена!");
        }

        private void UpdateTransaction()
        {
            if (transactionsListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите транзакцию!");
                return;
            }
            if (!ValidateInput(out decimal amount, out TransactionType type)) return;
            budgetManager.UpdateTransaction(
                budgetManager.Transactions[transactionsListBox.SelectedIndex],
                descriptionTextBox.Text, amount, type);
            RefreshList();
            ClearFields();
            MessageBox.Show("Транзакция обновлена!");
        }

        private bool ValidateInput(out decimal amount, out TransactionType type)
        {
            amount = 0;
            type = TransactionType.Доход;
            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                MessageBox.Show("Введите описание!");
                return false;
            }
            if (!decimal.TryParse(amountTextBox.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму (>0)!");
                return false;
            }
            type = typeComboBox.SelectedIndex == 0 ? TransactionType.Доход : TransactionType.Расход;
            return true;
        }

        private void ClearFields()
        {
            descriptionTextBox.Clear();
            amountTextBox.Clear();
            typeComboBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Now;
        }
    }
}