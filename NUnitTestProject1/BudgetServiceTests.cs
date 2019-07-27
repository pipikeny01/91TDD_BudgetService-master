using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTestProject_BudgetService;

namespace Tests
{
    public class BudgetServiceTests
    {
        private BudgetService _budgetService;
        private IBudgetRepo _stubBudgetRepo;

        [SetUp]
        public void Setup()
        {
            _stubBudgetRepo = Substitute.For<IBudgetRepo>();
            _budgetService = new BudgetService(_stubBudgetRepo);
        }

        [Test]
        public void QueryTest_Invalid()
        {
            QueryShouldbe(new DateTime(2019, 2, 1),
                new DateTime(2019, 1, 1), 0);
        }

        [Test]
        public void QueryTest_SingleDate()
        {
            _stubBudgetRepo.GetAll().Returns(new List<Budget> { new Budget { YearMonth = "201901", Amount = 31 } });

            QueryShouldbe(new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), 1);
        }

        [Test]
        public void QueryTest_PartialMonth()
        {
            _stubBudgetRepo.GetAll().Returns(new List<Budget> { new Budget{ YearMonth = "201901", Amount = 3100 }});
            QueryShouldbe(new DateTime(2019, 01, 01), new DateTime(2019, 01, 10), 1000);
        }


        [Test]
        public void QueryTest_CrossMonth()
        {
            var budgets = new List<Budget>
            {
                new Budget() {YearMonth = "201901", Amount = 31},
                new Budget {YearMonth = "201902", Amount = 28}
            };
            _stubBudgetRepo.GetAll().Returns(budgets);
            QueryShouldbe(new DateTime(2019, 01, 29), new DateTime(2019, 02, 1), 4);
        }


        [Test]
        public void QueryTest_CrossMonthNoBudget()
        {
            var budgets = new List<Budget>
            {
                new Budget() { YearMonth = "201901", Amount = 0 },
                new Budget() { YearMonth = "201902", Amount = 2800 }
            };

            _stubBudgetRepo.GetAll().Returns(budgets);
            QueryShouldbe(new DateTime(2019, 01, 15), new DateTime(2019, 02, 2), 200);
        }

        [Test]
        public void QueryTest_CrossYear()
        {
            var budgets = new List<Budget>
            {
                new Budget() { YearMonth = "201812", Amount = 31 },
                new Budget() { YearMonth = "201901", Amount = 31 }
            };

            _stubBudgetRepo.GetAll().Returns(budgets);
            QueryShouldbe(new DateTime(2018, 12, 15), new DateTime(2019, 01, 15), 32);
        }


        [Test]
        public void QueryTest_CrossMonthNoData()
        {
            var budgets = new List<Budget>
            {
                new Budget() { YearMonth = "201812", Amount = 310 },
                new Budget() { YearMonth = "201902", Amount = 28 },
                new Budget() { YearMonth = "201903", Amount = 31 }
            };

            _stubBudgetRepo.GetAll().Returns(budgets);

            QueryShouldbe(new DateTime(2018, 12, 31), new DateTime(2019, 02, 2), 12);
        }


        private void QueryShouldbe(DateTime starDateTime, DateTime endDateTime, double expect)
        {
            var result = _budgetService.Query(starDateTime, endDateTime);
            Assert.AreEqual(expect, actual: result);
        }
    }
}