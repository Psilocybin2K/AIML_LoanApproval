namespace AIML_LoanApproval
{
    using System.Text;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.OpenAI;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Kernel kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI"))
                .Build();

            // Register our ML plugin
            MLLightGbmPlugin mlPlugin = new MLLightGbmPlugin();
            kernel.Plugins.AddFromObject(mlPlugin);

            // Setup execution settings
            OpenAIPromptExecutionSettings executionSettings = new OpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // Initialize model with required fields
            KernelArguments modelArguments = new KernelArguments(executionSettings)
            {
                ["numericalFields"] = "CreditScore, AnnualIncome, LoanAmount, LoanDuration, Age, NumberOfDependents, MonthlyDebtPayments, CreditCardUtilizationRate, NumberOfOpenCreditLines, NumberOfCreditInquiries, DebtToIncomeRatio, BankruptcyHistory, PreviousLoanDefaults, InterestRate, PaymentHistory, SavingsAccountBalance, CheckingAccountBalance, InvestmentAccountBalance, RetirementAccountBalance, EmergencyFundBalance, TotalAssets, TotalLiabilities, NetWorth, LengthOfCreditHistory, MortgageBalance, RentPayments, AutoLoanBalance, PersonalLoanBalance, StudentLoanBalance, UtilityBillsPaymentHistory, OtherInsurancePolicies, JobTenure, MonthlySavings, AnnualBonuses, AnnualExpenses, MonthlyHousingCosts, MonthlyTransportationCosts, MonthlyFoodCosts, MonthlyHealthcareCosts, MonthlyEntertainmentCosts",
                ["encodedFields"] = "EmploymentStatus, MaritalStatus, EducationLevel, HomeOwnershipStatus, LoanPurpose, HealthInsuranceStatus, LifeInsuranceStatus, CarInsuranceStatus, HomeInsuranceStatus, EmployerType",
                ["targetPredictedLabel"] = "LoanApproved"
            };

            // Create our chat function
            KernelFunction chatFunction = kernel.CreateFunctionFromPromptyFile(@".\PromptTemplates\CreateModel.prompty");

            Console.WriteLine("Welcome to the Loan Approval Chat System");
            Console.WriteLine("Commands:");
            Console.WriteLine("  /CreateLightGbmModel - Create and train the model");
            Console.WriteLine("  /Predict <creditScore> <annualIncome> <loanAmount> <loanDuration> - Make a prediction");
            Console.WriteLine("  /UpdateSample <creditScore> <annualIncome> <loanAmount> <loanDuration> - Update default sample values");
            Console.WriteLine("  /exit - Exit the application");
            Console.WriteLine();

            bool exitRequested = false;

            // Create model on startup
            //Console.WriteLine("Creating initial model...");
            //modelArguments["message"] = "/CreateLightGbmModel";
            //await StreamResponse(kernel, chatFunction, modelArguments);

            while (!exitRequested)
            {
                Console.Write("> ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                    continue;

                if (userInput.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                {
                    exitRequested = true;
                    continue;
                }

                // Parse commands or treat as free-form text
                modelArguments["message"] = userInput;

                await StreamResponse(kernel, chatFunction, modelArguments);
            }
        }

        private static async Task StreamResponse(Kernel kernel, KernelFunction function, KernelArguments arguments)
        {
            Console.WriteLine();

            IAsyncEnumerable<StreamingKernelContent> responseStream =
                function.InvokeStreamingAsync(kernel, arguments);

            StringBuilder responseBuilder = new StringBuilder();

            await foreach (StreamingKernelContent content in responseStream)
            {
                string text = content.ToString();
                Console.Write(text);
                responseBuilder.Append(text);
            }

            Console.WriteLine("\n");
        }
    }
}
