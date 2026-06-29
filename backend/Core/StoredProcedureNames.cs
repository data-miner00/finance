using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    internal static class StoredProcedureNames
    {
        public const string AddAccount = "usp_AddAccount";

        public const string AddExpenseWithCategoryName = "usp_AddExpenseWithCategoryName";

        public const string AddExpenseFullParams = "usp_AddExpenseFullParams";

        public const string AddIncome = "usp_AddIncome";

        public const string AddPiggyBank = "usp_AddPiggyBank";

        public const string AddRecurring = "usp_AddRecurringAction";

        public const string AddPerson = "usp_AddPerson";

        public const string AddTax = "usp_AddTax";

        public const string DeleteAccount = "usp_DeleteAccount";

        public const string DeleteExpense = "usp_DeleteExpense";

        public const string DeleteIncome = "usp_DeleteIncome";

        public const string DeletePiggyBank = "usp_DeletePiggyBank";

        public const string DeleteRecurringAction = "usp_DeleteRecurringAction";

        public const string DeleteCategory = "usp_DeleteCategory";

        public const string DeletePerson = "usp_DeletePerson";

        public const string DeleteTax = "usp_DeleteTax";

        public const string AddCategory = "usp_AddCategory";
        
        public const string UpdateAccount = "usp_UpdateAccount";

        public const string UpdateExpenseWithCategoryName = "usp_UpdateExpenseWithCategoryName";

        public const string UpdateIncome = "usp_UpdateIncome";

        public const string UpdatePiggyBank = "usp_UpdatePiggyBank";

        public const string UpdateRecurringAction = "usp_UpdateRecurringAction";

        public const string UpdateCategory = "usp_UpdateCategory";

        public const string UpdatePerson = "usp_UpdatePerson";

        public const string UpdateTax = "usp_UpdateTax";

        public const string IncrementOrDecrementAccountBalance = "usp_IncrementOrDecrementAccountBalance";
    }
}
