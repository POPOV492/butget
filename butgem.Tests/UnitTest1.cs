using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace butgem.Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Constructor_ValidData_CreatesCorrectObject()
        {
            var date = new DateTime(2026, 5, 12);
            var t = new Transaction("Test", 50000, TransactionType.Доход, date);

            Assert.AreEqual("Test", t.Description);
            Assert.AreEqual(50000, t.Amount);
            Assert.AreEqual(TransactionType.Доход, t.Type);
            Assert.AreEqual(date, t.Date);
        }
    }

    [TestClass]
    public class BudgetManagerTests
    {
        private BudgetManager _manager;
        private string _file = "transactions.txt";

        [TestInitialize]
        public void SetUp()
        {
            if (File.Exists(_file)) File.Delete(_file);
            _manager = new BudgetManager();
        }

        [TestCleanup]
        public void TearDown()
        {
            if (File.Exists(_file)) File.Delete(_file);
        }

        [TestMethod]
        public void AddTransaction_Valid_IncreasesCount()
        {
            _manager.AddTransaction(new Transaction("T", 100, TransactionType.Доход, DateTime.Now));
            Assert.AreEqual(1, _manager.Transactions.Count);
        }

        [TestMethod]
        public void AddTransaction_Null_ThrowsException()
        {
            try
            {
                _manager.AddTransaction(null);
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException || ex is NullReferenceException);
            }
        }

        [TestMethod]
        public void RemoveTransaction_Valid_DecreasesCount()
        {
            var t = new Transaction("T", 100, TransactionType.Доход, DateTime.Now);
            _manager.AddTransaction(t);
            _manager.RemoveTransaction(t);
            Assert.AreEqual(0, _manager.Transactions.Count);
        }

        [TestMethod]
        public void RemoveTransaction_Null_ThrowsException()
        {
            try
            {
                _manager.RemoveTransaction(null);
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException || ex is NullReferenceException);
            }
        }

        [TestMethod]
        public void UpdateTransaction_Valid_UpdatesCorrectly()
        {
            var t = new Transaction("Old", 50, TransactionType.Расход, DateTime.Now);
            _manager.AddTransaction(t);
            _manager.UpdateTransaction(t, "New", 200, TransactionType.Доход);

            Assert.AreEqual("New", t.Description);
            Assert.AreEqual(200, t.Amount);
            Assert.AreEqual(TransactionType.Доход, t.Type);
        }

        [TestMethod]
        public void UpdateTransaction_Null_ThrowsException()
        {
            try
            {
                _manager.UpdateTransaction(null, "", 0, TransactionType.Доход);
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException || ex is NullReferenceException);
            }
        }

        [TestMethod]
        public void TotalBudget_IncomeMinusExpense_Correct()
        {
            _manager.AddTransaction(new Transaction("D", 1000, TransactionType.Доход, DateTime.Now));
            _manager.AddTransaction(new Transaction("R", 300, TransactionType.Расход, DateTime.Now));
            Assert.AreEqual(700, _manager.TotalBudget);
        }

        [TestMethod]
        public void TotalBudget_OnlyIncome_Positive()
        {
            _manager.AddTransaction(new Transaction("D", 500, TransactionType.Доход, DateTime.Now));
            Assert.AreEqual(500, _manager.TotalBudget);
        }

        [TestMethod]
        public void TotalBudget_OnlyExpense_Negative()
        {
            _manager.AddTransaction(new Transaction("R", 500, TransactionType.Расход, DateTime.Now));
            Assert.AreEqual(-500, _manager.TotalBudget);
        }

        [TestMethod]
        public void SaveAndLoad_PersistsData()
        {
            _manager.AddTransaction(new Transaction("Test", 777, TransactionType.Доход, new DateTime(2026, 1, 1)));
            var newManager = new BudgetManager();
            Assert.AreEqual(1, newManager.Transactions.Count);
            Assert.AreEqual("Test", newManager.Transactions[0].Description);
            Assert.AreEqual(777, newManager.Transactions[0].Amount);
        }

        [TestMethod]
        public void Constructor_FileNotExists_EmptyList()
        {
            var newManager = new BudgetManager();
            Assert.AreEqual(0, newManager.Transactions.Count);
        }
    }

    [TestClass]
    public class BoundaryTests
    {
        [TestMethod]
        public void AmountZero_IsAllowed()
        {
            var t = new Transaction("T", 0, TransactionType.Доход, DateTime.Now);
            Assert.AreEqual(0, t.Amount);
        }

        [TestMethod]
        public void NegativeAmount_IsAllowed()
        {
            var t = new Transaction("T", -100, TransactionType.Расход, DateTime.Now);
            Assert.AreEqual(-100, t.Amount);
        }

        [TestMethod]
        public void EmptyDescription_IsAllowed()
        {
            var t = new Transaction("", 100, TransactionType.Доход, DateTime.Now);
            Assert.AreEqual("", t.Description);
        }

        [TestMethod]
        public void LargeAmount_IsAllowed()
        {
            var t = new Transaction("T", 999999999, TransactionType.Доход, DateTime.Now);
            Assert.AreEqual(999999999, t.Amount);
        }
    }
}