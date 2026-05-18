namespace librarymanagementsystem.ModelLib
{
    public enum RuleDescription
    {
        
        FinePerDayLateReturn,
        FinePerMissingPageOnReturn,
        FineOnMissingHardCover,
        MaxPendingFineAmount
       
    }
    public class Rules
    {
        public int RulesId { get; set; }
        public  RuleDescription ruleDescription { get; set; }
        public int Value { get; set; }

        override public string ToString()
        {
            return $"RulesId: {RulesId}, ruleDescription: {ruleDescription}, Value: {Value}";
        }
    }
}