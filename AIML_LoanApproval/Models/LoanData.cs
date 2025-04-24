namespace AIML_LoanApproval.Models
{
    using Microsoft.ML.Data;

    // Define data classes
    public class LoanData
    {
        /*
         * MUST BE INTERNAL ONLY !!
         */
        #region INTERNAL_ONLY
        internal EmploymentStatus _EmploymentStatus;
        internal MaritalStatus _MaritalStatus;
        internal EducationLevel _EducationLevel;
        #endregion

        [LoadColumn(0)]
        public float CreditScore { get; set; }

        [LoadColumn(1)]
        public float AnnualIncome { get; set; }

        [LoadColumn(2)]
        public float LoanAmount { get; set; }

        [LoadColumn(3)]
        public float LoanDuration { get; set; }

        [LoadColumn(4)]
        public float Age { get; set; }

        [LoadColumn(5)]
        public string EmploymentStatus => _EmploymentStatus.ToString();

        [LoadColumn(6)]
        public string MaritalStatus => _MaritalStatus.ToString();

        [LoadColumn(7)]
        public float NumberOfDependents { get; set; }

        [LoadColumn(8)]
        public string EducationLevel => _EducationLevel.ToString();

        [LoadColumn(9)]
        public string HomeOwnershipStatus { get; set; }

        [LoadColumn(10)]
        public float MonthlyDebtPayments { get; set; }

        [LoadColumn(11)]
        public float CreditCardUtilizationRate { get; set; }

        [LoadColumn(12)]
        public float NumberOfOpenCreditLines { get; set; }

        [LoadColumn(13)]
        public float NumberOfCreditInquiries { get; set; }

        [LoadColumn(14)]
        public float DebtToIncomeRatio { get; set; }

        [LoadColumn(15)]
        public float BankruptcyHistory { get; set; }

        [LoadColumn(16)]
        public string LoanPurpose { get; set; }

        [LoadColumn(17)]
        public float PreviousLoanDefaults { get; set; }

        [LoadColumn(18)]
        public float InterestRate { get; set; }

        [LoadColumn(19)]
        public float PaymentHistory { get; set; }

        [LoadColumn(20)]
        public float SavingsAccountBalance { get; set; }

        [LoadColumn(21)]
        public float CheckingAccountBalance { get; set; }

        [LoadColumn(22)]
        public float InvestmentAccountBalance { get; set; }

        [LoadColumn(23)]
        public float RetirementAccountBalance { get; set; }

        [LoadColumn(24)]
        public float EmergencyFundBalance { get; set; }

        [LoadColumn(25)]
        public float TotalAssets { get; set; }

        [LoadColumn(26)]
        public float TotalLiabilities { get; set; }

        [LoadColumn(27)]
        public float NetWorth { get; set; }

        [LoadColumn(28)]
        public float LengthOfCreditHistory { get; set; }

        [LoadColumn(29)]
        public float MortgageBalance { get; set; }

        [LoadColumn(30)]
        public float RentPayments { get; set; }

        [LoadColumn(31)]
        public float AutoLoanBalance { get; set; }

        [LoadColumn(32)]
        public float PersonalLoanBalance { get; set; }

        [LoadColumn(33)]
        public float StudentLoanBalance { get; set; }

        [LoadColumn(34)]
        public float UtilityBillsPaymentHistory { get; set; }

        [LoadColumn(35)]
        public string HealthInsuranceStatus { get; set; }

        [LoadColumn(36)]
        public string LifeInsuranceStatus { get; set; }

        [LoadColumn(37)]
        public string CarInsuranceStatus { get; set; }

        [LoadColumn(38)]
        public string HomeInsuranceStatus { get; set; }

        [LoadColumn(39)]
        public float OtherInsurancePolicies { get; set; }

        [LoadColumn(40)]
        public string EmployerType { get; set; }

        [LoadColumn(41)]
        public float JobTenure { get; set; }

        [LoadColumn(42)]
        public float MonthlySavings { get; set; }

        [LoadColumn(43)]
        public float AnnualBonuses { get; set; }

        [LoadColumn(44)]
        public float AnnualExpenses { get; set; }

        [LoadColumn(45)]
        public float MonthlyHousingCosts { get; set; }

        [LoadColumn(46)]
        public float MonthlyTransportationCosts { get; set; }

        [LoadColumn(47)]
        public float MonthlyFoodCosts { get; set; }

        [LoadColumn(48)]
        public float MonthlyHealthcareCosts { get; set; }

        [LoadColumn(49)]
        public float MonthlyEntertainmentCosts { get; set; }

        [LoadColumn(50)]
        public bool LoanApproved { get; set; }
    }
}
