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
    }
}