namespace AIML_LoanApproval
{
    using System.ComponentModel;
    using System.Text.Json;
    using AIML_LoanApproval.Models;
    using Microsoft.ML;
    using Microsoft.ML.Calibrators;
    using Microsoft.ML.Data;
    using Microsoft.ML.Trainers.LightGbm;
    using Microsoft.SemanticKernel;

    public class MLLightGbmPlugin
    {

        private MLContext mlContext = new MLContext(seed: 0);
        private IDataView _trainingData;
        private IDataView _testData;
        private LoanData _sample;
        private TransformerChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LightGbmBinaryModelParameters, PlattCalibrator>>> _model;

        public MLLightGbmPlugin()
        {
            // Load data
            IDataView data = mlContext.Data.LoadFromTextFile<LoanData>(
                path: "loan_approvals.csv",
                hasHeader: true,
                separatorChar: ',');

            // Split data into training (80%) and testing (20%) sets
            DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            _trainingData = dataSplit.TrainSet;
            _testData = dataSplit.TestSet;

            _sample = new LoanData
            {
                CreditScore = 700,
                AnnualIncome = 200000,
                LoanAmount = 120000,
                LoanDuration = 30,
                Age = 35,
                _EmploymentStatus = EmploymentStatus.Employed,
                _MaritalStatus = MaritalStatus.Single,
                NumberOfDependents = 1,
                _EducationLevel = EducationLevel.Bachelor,
                HomeOwnershipStatus = "Own",
                MonthlyDebtPayments = 500,
                CreditCardUtilizationRate = 0.2f,
                NumberOfOpenCreditLines = 5,
                NumberOfCreditInquiries = 1,
                DebtToIncomeRatio = 0.3f,
                BankruptcyHistory = 0,
                LoanPurpose = "Home Improvement",
                PreviousLoanDefaults = 0,
                InterestRate = 0.05f,
                PaymentHistory = 24,
                SavingsAccountBalance = 10000,
                CheckingAccountBalance = 5000,
                InvestmentAccountBalance = 20000,
                RetirementAccountBalance = 50000,
                EmergencyFundBalance = 15000,
                TotalAssets = 350000,
                TotalLiabilities = 120000,
                NetWorth = 230000,
                LengthOfCreditHistory = 10,
                MortgageBalance = 100000,
                RentPayments = 0,
                AutoLoanBalance = 10000,
                PersonalLoanBalance = 0,
                StudentLoanBalance = 5000,
                UtilityBillsPaymentHistory = 1.0f,
                HealthInsuranceStatus = "Insured",
                LifeInsuranceStatus = "Insured",
                CarInsuranceStatus = "Insured",
                HomeInsuranceStatus = "Insured",
                OtherInsurancePolicies = 2,
                EmployerType = "Private",
                JobTenure = 60,
                MonthlySavings = 1000,
                AnnualBonuses = 5000,
                AnnualExpenses = 40000,
                MonthlyHousingCosts = 1500,
                MonthlyTransportationCosts = 300,
                MonthlyFoodCosts = 600,
                MonthlyHealthcareCosts = 200,
                MonthlyEntertainmentCosts = 300
            };
        }

        [KernelFunction("UpdateSample")]
        [Description("Update the base sample object.")]
        public string UpdateSample(int? creditScore = null,
            int? annualIncome = null,
            int? loanAmount = null,
            int? loanDuration = null,
            int? age = null)
        {
#if DEBUG
            Console.WriteLine($"""
            Updating Sample
            ===============

            Fico: {creditScore}
            Annual Income: {annualIncome}
            Loan Amount: {loanAmount}
            Loan Duration: {loanDuration}
            Age: {age}

            """);
#endif
            if (creditScore.HasValue)
            {
                _sample.CreditScore = creditScore.Value;
            }

            if (annualIncome != null)
            {
                _sample.AnnualIncome = annualIncome.Value;
            }

            if (loanAmount != null)
            {
                _sample.LoanAmount = loanAmount.Value;
            }

            if (loanDuration != null)
            {
                _sample.LoanDuration = loanDuration.Value;
            }

            if(age != null)
            {
                _sample.Age = age.Value;
            }

            return JsonSerializer.Serialize(new
            {
                Sample = _sample,
                Updates = new
                {
                    creditScore,
                    annualIncome,
                    loanAmount,
                    loanDuration,
                    age
                },
                Monologue = "{.. summary ..}"
            }, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }


        [KernelFunction(nameof(Predict))]
        [Description("Predict on Machine Learning Model")]
        public string Predict(int? creditScore = null,
            int? annualIncome = null,
            int? loanAmount = null,
            int? loanDuration = null,
            int? age = null)
        {
#if DEBUG
            Console.WriteLine($"""
            Predicting on Model
            ===================

            Fico: {creditScore}
            Annual Income: {annualIncome}
            Loan Amount: {loanAmount}
            Loan Duration: {loanDuration}
            Age: {age}

            """);
#endif
            if (_model == null)
            {
                return """
                Respond with the following payload and TERMINATE!

                {
                    "success": false,
                    "message": "No model loaded. Try instructing the LLM to create a model first.",
                    "terminate": true
                }
                """;

                throw new Exception("""
                The model has not yet been trained. You must first create the model before it can be used.
                """);
            }

            PredictionEngine<LoanData, LoanPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<LoanData, LoanPrediction>(_model);

            LoanData sampleData = new LoanData
            {
                CreditScore = creditScore ?? _sample.CreditScore,
                AnnualIncome = annualIncome ?? _sample.AnnualIncome,
                LoanAmount = loanAmount ?? _sample.LoanAmount,
                LoanDuration = loanDuration ?? _sample.LoanDuration,
                Age = age ?? _sample.Age,
                _EmploymentStatus = Enum.Parse<EmploymentStatus>(_sample.EmploymentStatus),
                _MaritalStatus = Enum.Parse<MaritalStatus>(_sample.MaritalStatus),
                NumberOfDependents = _sample.NumberOfDependents,
                _EducationLevel = Enum.Parse<EducationLevel>(_sample.EducationLevel),
                HomeOwnershipStatus = _sample.HomeOwnershipStatus,
                MonthlyDebtPayments = _sample.MonthlyDebtPayments,
                CreditCardUtilizationRate = _sample.CreditCardUtilizationRate,
                NumberOfOpenCreditLines = _sample.NumberOfOpenCreditLines,
                NumberOfCreditInquiries = _sample.NumberOfCreditInquiries,
                DebtToIncomeRatio = _sample.DebtToIncomeRatio,
                BankruptcyHistory = _sample.BankruptcyHistory,
                LoanPurpose = _sample.LoanPurpose,
                PreviousLoanDefaults = _sample.PreviousLoanDefaults,
                InterestRate = _sample.InterestRate,
                PaymentHistory = _sample.PaymentHistory,
                SavingsAccountBalance = _sample.SavingsAccountBalance,
                CheckingAccountBalance = _sample.CheckingAccountBalance,
                InvestmentAccountBalance = _sample.InvestmentAccountBalance,
                RetirementAccountBalance = _sample.RetirementAccountBalance,
                EmergencyFundBalance = _sample.EmergencyFundBalance,
                TotalAssets = _sample.TotalAssets,
                TotalLiabilities = _sample.TotalLiabilities,
                NetWorth = _sample.NetWorth,
                LengthOfCreditHistory = _sample.LengthOfCreditHistory,
                MortgageBalance = _sample.MortgageBalance,
                RentPayments = _sample.RentPayments,
                AutoLoanBalance = _sample.AutoLoanBalance,
                PersonalLoanBalance = _sample.PersonalLoanBalance,
                StudentLoanBalance = _sample.StudentLoanBalance,
                UtilityBillsPaymentHistory = _sample.UtilityBillsPaymentHistory,
                HealthInsuranceStatus = _sample.HealthInsuranceStatus,
                LifeInsuranceStatus = _sample.LifeInsuranceStatus,
                CarInsuranceStatus = _sample.CarInsuranceStatus,
                HomeInsuranceStatus = _sample.HomeInsuranceStatus,
                OtherInsurancePolicies = _sample.OtherInsurancePolicies,
                EmployerType = _sample.EmployerType,
                JobTenure = _sample.JobTenure,
                MonthlySavings = _sample.MonthlySavings,
                AnnualBonuses = _sample.AnnualBonuses,
                AnnualExpenses = _sample.AnnualExpenses,
                MonthlyHousingCosts = _sample.MonthlyHousingCosts,
                MonthlyTransportationCosts = _sample.MonthlyTransportationCosts,
                MonthlyFoodCosts = _sample.MonthlyFoodCosts,
                MonthlyHealthcareCosts = _sample.MonthlyHealthcareCosts,
                MonthlyEntertainmentCosts = _sample.MonthlyEntertainmentCosts
            };

            LoanPrediction prediction = predictionEngine.Predict(sampleData);

            string predictionPayload = JsonSerializer.Serialize(sampleData, new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            return $$$"""
            Respond with the following structure:

                {
                    "success": true,
                    "prediction": {
                        "value": {{{prediction.PredictedLabel}}},
                        "probability": {{{prediction.Probability:F4}}}
                    },
                    "monologue": "{.. summary ..}"
                }

            Replace `monologue` with a summary outlining the reasoning and factors of this decision.

            ## Context

            {{{predictionPayload}}}
            """;
        }

        [KernelFunction(nameof(CreateLightGbmModel))]
        [Description("Create LightGbm Machine Learning Model")]
        public string CreateLightGbmModel(string[] numericalFeatures, string[] categoricalFeatures, string targetPredictedLabel)
        {
#if DEBUG
            Console.WriteLine($"""
            Creating and training model...
            """);
#endif

            EstimatorChain<BinaryPredictionTransformer<Microsoft.ML.Calibrators.CalibratedModelParametersBase<LightGbmBinaryModelParameters, Microsoft.ML.Calibrators.PlattCalibrator>>> pipeline = this.CreateLightGBMPipeline(numericalFeatures, categoricalFeatures, targetPredictedLabel);
            _model = pipeline.Fit(_trainingData);

            return $$$"""
            {
                "success": true,
                "message": "Model created and trained successfully.",
                "terminate": true
            }
            """;
        }

        public EstimatorChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LightGbmBinaryModelParameters, PlattCalibrator>>> CreateLightGBMPipeline(string[] numericalFeatures,
            string[] categoricalFeatures,
            string targetPredictedLabel,
            int numberOfLeaves = 31,
            int numberOfIterations = 100,
            int minExamplesPerLeaf = 20,
            double learningRate = 0.1,
            bool verbose = false
            )
        {
            return this.CreateNormals(numericalFeatures, categoricalFeatures)
                        .Append(mlContext.BinaryClassification.Trainers.LightGbm(
                            new LightGbmBinaryTrainer.Options
                            {
                                NumberOfLeaves = numberOfLeaves,
                                NumberOfIterations = numberOfIterations,
                                MinimumExampleCountPerLeaf = minExamplesPerLeaf,
                                LearningRate = learningRate,
                                LabelColumnName = targetPredictedLabel,
                                FeatureColumnName = "Features",
                                Verbose = verbose
                            }));
        }

        public EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer> CreateNormals(string[] numericalFeatures, string[] categoricalFeatures)
        {
            return this.CreateFeatures(numericalFeatures, categoricalFeatures)
                        .Append(mlContext.Transforms.NormalizeMinMax("Features"));
        }

        public EstimatorChain<ColumnConcatenatingTransformer> CreateFeatures(string[] numericalFeatures, string[] categoricalFeatures)
        {
            string[] features = numericalFeatures.Concat(categoricalFeatures.Select(c => $"{c}Encoded")).ToArray();

            return this.CreateInputOutputPairs(categoricalFeatures)
                .Append(mlContext.Transforms.Concatenate("Features", features));
        }

        public Microsoft.ML.Transforms.OneHotEncodingEstimator CreateInputOutputPairs(string[] categoricalFeatures)
        {
            List<InputOutputColumnPair> encodingPairs = [];

            foreach (string feature in categoricalFeatures)
            {
                encodingPairs.Add(new InputOutputColumnPair($"{feature}Encoded", feature));
            }

            return mlContext.Transforms.Categorical.OneHotEncoding(encodingPairs.ToArray());
        }
    }
}
