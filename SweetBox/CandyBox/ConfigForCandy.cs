using SweetBox.CandyClasses;
public class RangeConfig {
   public double Min { get; set; }
   public double Max { get; set; }
}

public class JellyConfig {
   public double MinSugar { get; set; }
   public double MaxSugar { get; set; }
}

public class ChocolateConfig {
   public double MinSugar { get; set; }
   public double MaxSugar { get; set; }
   public List<string> AvailableTypes { get; set; }
}

public class LollipopConfig {
   public double MinSugar { get; set; }
   public double MaxSugar { get; set; }
   public List<string> Flavors { get; set; }
}

public class CandyGenerationSettings {
   public Dictionary<CandySize, RangeConfig> WeightRanges { get; set; }
   public JellyConfig JellyConfig { get; set; }
   public ChocolateConfig ChocolateConfig { get; set; }
   public LollipopConfig LollipopConfig { get; set; }
}