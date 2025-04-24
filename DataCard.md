# Loan Data Structure Documentation

## Table of Contents
- [Overview](#overview)
- [Data Schema](#data-schema)
- [Field Descriptions](#field-descriptions)
  - [Borrower Information](#borrower-information)
  - [Loan Details](#loan-details)
  - [Credit Information](#credit-information)
  - [Financial Status](#financial-status)
  - [Insurance Information](#insurance-information)
  - [Employment Information](#employment-information)
  - [Monthly Expenses](#monthly-expenses)
  - [Target Variable](#target-variable)
- [Enumeration Types](#enumeration-types)
- [Data Preprocessing](#data-preprocessing)
- [Feature Importance](#feature-importance)
- [Data Security Considerations](#data-security-considerations)

## Overview

The AIML Loan Approval System uses a comprehensive loan dataset to train its prediction model. This document outlines the complete structure of the loan data used in the system, including all fields, data types, and their significance in the loan approval process.

## Data Schema

The loan data is defined in the `LoanData` class, which serves as both the data loading model and the feature input for the ML.NET pipeline. The dataset contains 51 fields in total, with the 51st field being the target variable (loan approval status).

## Field Descriptions

### Borrower Information

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| Age | float | Age of the loan applicant in years | 4 |
| MaritalStatus | enum | Marital status (Single, Divorce, Widowed, Married) | 6 |
| NumberOfDependents | float | Number of financial dependents | 7 |
| EducationLevel | enum | Highest education completed (HighSchool, Associate, Bachelor, Master, Doctorate) | 8 |

### Loan Details

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| LoanAmount | float | The requested loan amount in dollars | 2 |
| LoanDuration | float | Term of the loan in years | 3 |
| LoanPurpose | string | Purpose of the loan (e.g., "Home Improvement", "Debt Consolidation") | 16 |
| InterestRate | float | Annual interest rate as decimal (0.05 = 5%) | 18 |

### Credit Information

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| CreditScore | float | Credit score (typically 300-850 range) | 0 |
| MonthlyDebtPayments | float | Total monthly debt payments in dollars | 10 |
| CreditCardUtilizationRate | float | Credit utilization ratio (0.0-1.0) | 11 |
| NumberOfOpenCreditLines | float | Count of active credit accounts | 12 |
| NumberOfCreditInquiries | float | Recent credit inquiries count | 13 |
| DebtToIncomeRatio | float | Monthly debt payments divided by monthly income | 14 |
| BankruptcyHistory | float | Bankruptcy flag (0 = no, 1 = yes) | 15 |
| PreviousLoanDefaults | float | Count of previous defaults | 17 |
| PaymentHistory | float | Months of on-time payments | 19 |
| LengthOfCreditHistory | float | Years of credit history | 28 |

### Financial Status

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| AnnualIncome | float | Yearly income in dollars | 1 |
| SavingsAccountBalance | float | Balance in savings accounts | 20 |
| CheckingAccountBalance | float | Balance in checking accounts | 21 |
| InvestmentAccountBalance | float | Balance in investment accounts | 22 |
| RetirementAccountBalance | float | Balance in retirement accounts | 23 |
| EmergencyFundBalance | float | Emergency fund balance | 24 |
| TotalAssets | float | Sum of all assets in dollars | 25 |
| TotalLiabilities | float | Sum of all debts in dollars | 26 |
| NetWorth | float | Total assets minus total liabilities | 27 |
| MortgageBalance | float | Current mortgage balance | 29 |
| RentPayments | float | Monthly rent payments (if applicable) | 30 |
| AutoLoanBalance | float | Current auto loan balance | 31 |
| PersonalLoanBalance | float | Current personal loan balance | 32 |
| StudentLoanBalance | float | Current student loan balance | 33 |
| UtilityBillsPaymentHistory | float | Payment history for utilities (0-1 scale) | 34 |
| MonthlySavings | float | Amount saved monthly | 42 |
| AnnualBonuses | float | Yearly bonus amount | 43 |
| AnnualExpenses | float | Total yearly expenses | 44 |

### Insurance Information

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| HealthInsuranceStatus | string | Current health insurance status | 35 |
| LifeInsuranceStatus | string | Current life insurance status | 36 |
| CarInsuranceStatus | string | Current auto insurance status | 37 |
| HomeInsuranceStatus | string | Current home insurance status | 38 |
| OtherInsurancePolicies | float | Count of other insurance policies | 39 |

### Employment Information

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| EmploymentStatus | enum | Current employment status (Employed, SelfEmployed, Unemployed) | 5 |
| EmployerType | string | Type of employer (e.g., "Private", "Government") | 40 |
| JobTenure | float | Months at current job | 41 |

### Monthly Expenses

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| MonthlyHousingCosts | float | Monthly housing expenses | 45 |
| MonthlyTransportationCosts | float | Monthly transportation costs | 46 |
| MonthlyFoodCosts | float | Monthly food expenses | 47 |
| MonthlyHealthcareCosts | float | Monthly healthcare costs | 48 |
| MonthlyEntertainmentCosts | float | Monthly entertainment expenses | 49 |

### Target Variable

| Field | Data Type | Description | Index |
|-------|-----------|-------------|-------|
| LoanApproved | bool | Target variable (true = approved, false = denied) | 50 |

## Enumeration Types

The system uses several enumeration types to handle categorical data:

### MaritalStatus

```csharp
public enum MaritalStatus
{
    Single,
    Divorce,
    Widowed,
    Married
}
```

### EducationLevel

```csharp
public enum EducationLevel
{
    HighSchool,
    Associate,
    Master,
    Doctorate,
    Bachelor
}
```

### EmploymentStatus

```csharp
public enum EmploymentStatus
{
    SelfEmployed,
    Unemployed,
    Employed
}
```

## Data Preprocessing

The system applies several preprocessing steps to prepare the data for the LightGBM model:

1. **Categorical Encoding**: All categorical fields (those defined as enums or strings) are one-hot encoded
2. **Feature Normalization**: Numerical features are normalized using min-max scaling (0-1 range)
3. **Feature Concatenation**: All transformed features are combined into a single feature vector named "Features"

## Feature Importance

The LightGBM model used for predictions inherently calculates feature importance during training. The most influential features for loan approval typically include:

1. **Credit Score**: The strongest predictor of loan repayment ability
2. **Debt-to-Income Ratio**: Indicates financial burden relative to income
3. **Employment Status & Stability**: Indicates income reliability 
4. **Payment History**: Past payment behavior predicts future behavior
5. **Loan-to-Value Ratio**: Calculated from loan amount relative to asset values

## Data Security Considerations

The loan data contains sensitive personal and financial information. The system implements several security measures:

1. **Internal-Only Fields**: Some fields are marked as internal with private backing fields
2. **Environment Variables**: API keys and sensitive configurations use environment variables
3. **In-Memory Processing**: Data is processed in-memory and not persisted between sessions
4. **Sample-Based Prediction**: The system uses a sample-based approach rather than storing actual applicant data

---

*Created by Montray Davis | Mortgage Financial Services Innovation*