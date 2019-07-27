using System.Collections.Generic;

namespace UnitTestProject_BudgetService
{
    public interface IBudgetRepo
    {
        List<Budget> GetAll();
    }
}