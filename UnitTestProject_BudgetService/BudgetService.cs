using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject_BudgetService
{
    public class BudgetService
    {
        private readonly IBudgetRepo _budgetRepo;

        public BudgetService(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
        }

        public double Query(DateTime startDateTime, DateTime endDateTime)
        {
            if (startDateTime > endDateTime)
            {
                return 0;
            }
            else
            {
                var allBudget = _budgetRepo.GetAll();
                return CalculateBudgetAmountTotal(startDateTime, endDateTime, allBudget);
            }
        }

        private double CalculateBudgetAmountTotal(DateTime startDateTime, DateTime endDateTime, List<Budget> allBudget)
        {
            double total = 0;

            int month = GetTotalMonth(startDateTime, endDateTime);

            for (var i = 0; i < month; i++)
            {
                var currentBudget =
                    allBudget.FirstOrDefault(p => p.YearMonth == startDateTime.AddMonths(i).ToString("yyyyMM"));

                total += CalculateBudgetAmountMonth(startDateTime, endDateTime, currentBudget, i);
            }

            return total;
        }

        private int GetTotalMonth(DateTime startDateTime, DateTime endDateTime)
        {
            return (endDateTime.Year - startDateTime.Year) * 12 + (endDateTime.Month - startDateTime.Month) + 1;
        }

        private double CalculateBudgetAmountMonth(DateTime startDateTime, DateTime endDateTime, Budget currentBudget, int addMonth)
        {
            if (currentBudget == null || currentBudget.Amount == 0)
                return 0;

            if (IsLastMonth(startDateTime, endDateTime, addMonth))
            {
                return BudgetAmountByDay(startDateTime, addMonth, currentBudget) * (endDateTime.Day);
            }

            return BudgetAmountByDay(startDateTime, addMonth, currentBudget) *
                   (MonthDays(startDateTime, addMonth) - startDateTime.Day + 1);
        }

        private bool IsLastMonth(DateTime startDateTime, DateTime endDateTime, int addMonth)
        {
            return startDateTime.AddMonths(addMonth).Month == endDateTime.Month;
        }

        private double BudgetAmountByDay(DateTime startDateTime, int addMonth, Budget budget)
        {
            return budget.Amount /
                   MonthDays(startDateTime, addMonth);
        }

        private double MonthDays(DateTime startDateTime, int addMonth)
        {
            return (double)(DateTime.DaysInMonth(startDateTime.Year, startDateTime.AddMonths(addMonth).Month));
        }
    }
}