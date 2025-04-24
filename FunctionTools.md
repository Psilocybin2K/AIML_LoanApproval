# AIML Loan Approval System - Kernel Functions Documentation

This document provides detailed information about the Semantic Kernel functions available in the AIML Loan Approval System. These functions serve as the interface between the natural language AI system and the underlying machine learning model.

## 🧩 Available Functions

The system exposes three primary functions through the `MLLightGbmPlugin` class:

1. [UpdateSample](#updatesample)
2. [CreateLightGbmModel](#createlightgbmmodel)
3. [Predict](#predict)

---

## UpdateSample

### Description
Updates the baseline sample values used for predictions. This function allows you to modify the default values of key loan application parameters while keeping other parameters constant.

### Function Signature
```csharp
[KernelFunction("UpdateSample")]
[Description("Update the base sample object.")]
public string UpdateSample(
    int? creditScore = null,
    int? annualIncome = null,
    int? loanAmount = null,
    int? loanDuration = null,
    int? age = null)
```

### Parameters

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `creditScore` | int? | The applicant's credit score (e.g., 300-850 range) | 700 |
| `annualIncome` | int? | Annual income in dollars | 200,000 |
| `loanAmount` | int? | Amount of loan requested in dollars | 120,000 |
| `loanDuration` | int? | Duration of loan in years | 30 |
| `age` | int? | The age of the borrower | 35 |

### Returns
A JSON string containing:
- The updated sample object with all parameters
- A record of which parameters were updated
- A monologue field for reasoning behind the update

### Example Usage
```
> /UpdateSample 750 95000 300000 15 42
```

### Notes
- Parameters not specified will retain their current values
- Update the sample before making predictions to establish baseline values for subsequent predictions
- The sample maintains 50+ financial attributes internally; this function only exposes the most frequently adjusted parameters

---

## CreateLightGbmModel

### Description
Creates and trains a LightGBM machine learning model for loan approval prediction. This function configures the model pipeline, processes features, and trains on the provided dataset.

### Function Signature
```csharp
[KernelFunction(nameof(CreateLightGbmModel))]
[Description("Create LightGbm Machine Learning Model")]
public string CreateLightGbmModel(
    string[] numericalFeatures, 
    string[] categoricalFeatures, 
    string targetPredictedLabel)
```

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `numericalFeatures` | string[] | Array of numerical feature column names to include in the model |
| `categoricalFeatures` | string[] | Array of categorical feature column names to one-hot encode |
| `targetPredictedLabel` | string | Name of the column to predict (target variable) |

### Advanced Configuration
The internal implementation accepts additional parameters not exposed directly:

| Parameter | Description | Default |
|-----------|-------------|---------|
| `numberOfLeaves` | Number of leaves in the tree | 31 |
| `numberOfIterations` | Number of boosting iterations | 100 |
| `minExamplesPerLeaf` | Minimum number of samples required per leaf | 20 |
| `learningRate` | Step size shrinkage used to prevent overfitting | 0.1 |
| `verbose` | Whether to display verbose logging | false |

### Returns
A JSON string with:
- Success status
- Message about model creation
- Termination flag

### Example Usage
```
> /CreateLightGbmModel
```

### Notes
- The function automatically loads data from "loan_approvals.csv"
- Dataset is automatically split 80/20 for training and testing
- Categorical features are one-hot encoded
- Numerical features are normalized using min-max scaling

---

## Predict

### Description
Generates a loan approval prediction based on the current model and provided parameters. Uses the values from the most recent sample unless overridden by the parameters.

### Function Signature
```csharp
[KernelFunction(nameof(Predict))]
[Description("Predict on Machine Learning Model")]
public string Predict(
    int? creditScore = null,
    int? annualIncome = null,
    int? loanAmount = null,
    int? loanDuration = null,
    int? age = null)
```

### Parameters

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `creditScore` | int? | The applicant's credit score (e.g., 300-850 range) | From sample |
| `annualIncome` | int? | Annual income in dollars | From sample |
| `loanAmount` | int? | Amount of loan requested in dollars | From sample |
| `loanDuration` | int? | Duration of loan in years | From sample |
| `age` | int? | The age of the borrower | From sample |

### Returns
A JSON string containing:
- Success status
- Prediction object with:
  - Value (boolean): Approved or denied
  - Probability (float): Confidence score between 0-1
- Monologue: Summary of reasoning behind the decision

### Example Usage
```
> /Predict 720 85000 250000 30
```

### Notes
- Returns an error if no model has been created yet
- Uses default values from the sample for any parameters not specified
- All 50+ loan features from the sample are used in the prediction, not just the specified parameters
- Probability score indicates confidence in the prediction (higher = more confident)

---

## 🔄 Function Workflow

Typical usage flow:
1. Call `CreateLightGbmModel` to create and train the initial model
2. Use `UpdateSample` to set baseline values relevant to your use case
3. Call `Predict` with specific parameters to get loan approval predictions
4. Adjust parameters and continue making predictions to explore outcomes

## ⚙️ Internal Implementation Details

- The functions are implemented as Semantic Kernel functions in the MLLightGbmPlugin class
- Data is loaded from a CSV file specified in the app configuration
- The ML pipeline handles feature engineering automatically:
  - One-hot encoding for categorical features
  - Normalization for numerical features
  - Feature concatenation into a single feature vector
- The LightGBM model is a binary classifier with calibrated probabilities

## 🔒 Security Considerations

- The system maintains a sample applicant object with default values
- No personal data is stored between sessions
- Predictions are made in-memory and not persisted
- The system uses environment variables for API keys

---

*Author: Montray Davis*