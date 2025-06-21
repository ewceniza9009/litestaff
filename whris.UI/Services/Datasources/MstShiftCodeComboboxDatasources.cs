namespace whris.UI.Services.Datasources
{
    public class MstShiftCodeComboboxDatasources
    {
        public List<Day> Days => new List<Day>
        {
            new Day("Monday"),
            new Day("Tuesday"),
            new Day("Wednesday"),
            new Day("Thursday"),
            new Day("Friday"),
            new Day("Saturday"),
            new Day("Sunday")
        };

        public static MstShiftCodeComboboxDatasources Instance => new MstShiftCodeComboboxDatasources();

        private MstShiftCodeComboboxDatasources()
        {

        }

        public class Day
        {
            public Day(string? dayName)
            {
                DayName = dayName;
            }

            public string? DayName { get; set; }
        }
    }
}
