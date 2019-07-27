using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject_BudgetService
{
    [TestClass]
    public class BudgetServiceTest
    {
        private BudgetService.IBudgetRepo _stubBudgetRepo = new stubBudgetRepo();
        private BudgetService _budgetService;

        public BudgetServiceTest()
        {
            _budgetService = new BudgetService(_stubBudgetRepo);
        }

        [TestMethod]
        public void Invalid()
        {
            QueryShouldbe(new DateTime(2019, 05, 01), new DateTime(2019, 01, 01), 0);
        }


        [TestMethod]
        public void SingleDate()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201901", Amont = 31});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), 1);
        }

        [TestMethod]
        public void PartialMonth()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201901", Amont = 3100});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2019, 01, 01), new DateTime(2019, 01, 10), 1000);
        }

        [TestMethod]
        public void CrossMonth()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201901", Amont = 31});
            budgets.Add(new BudgetService.Budget() {YearMonth = "201902", Amont = 28});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2019, 01, 29), new DateTime(2019, 02, 1), 4);
        }

        [TestMethod]
        public void CrossMonthNoBudget()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201901", Amont = 0});
            budgets.Add(new BudgetService.Budget() {YearMonth = "201902", Amont = 2800});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2019, 01, 15), new DateTime(2019, 02, 2), 200);
        }

        [TestMethod]
        public void CrossYear()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201812", Amont = 31});
            budgets.Add(new BudgetService.Budget() {YearMonth = "201901", Amont = 31});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2018, 12, 15), new DateTime(2019, 01, 15), 32);
        }

        [TestMethod]
        public void CrossMonthNoData()
        {
            var budgets = new List<BudgetService.Budget>();
            budgets.Add(new BudgetService.Budget() {YearMonth = "201812", Amont = 310});
            budgets.Add(new BudgetService.Budget() {YearMonth = "201902", Amont = 28});
            ((stubBudgetRepo) _stubBudgetRepo).SetData(budgets);
            QueryShouldbe(new DateTime(2018, 12, 31), new DateTime(2019, 02, 2), 12);
        }

        private void QueryShouldbe(DateTime startDateTime, DateTime endDateTime, double result)
        {
            Assert.AreEqual(result, _budgetService.Query(startDateTime, endDateTime));
        }
    }

    public class stubBudgetRepo : BudgetService.IBudgetRepo
    {
        private List<BudgetService.Budget> _budgetList = new List<BudgetService.Budget>();

        public List<BudgetService.Budget> getAll()
        {
            return _budgetList;
        }

        public void SetData(List<BudgetService.Budget> budgets)
        {
            _budgetList = budgets;
        }
    }
}