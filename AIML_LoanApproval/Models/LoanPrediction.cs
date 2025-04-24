namespace AIML_LoanApproval.Models
{
    using Microsoft.ML.Data;

    public class LoanPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }
    }
}
