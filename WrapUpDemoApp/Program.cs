// See https://aka.ms/new-console-template for more information

var persons = new List<PersonModel>
{
    new() {FirstName = "Gregory", LastName = "Pereira", Age = 21},
    new() {FirstName = "Name", LastName = "Pereira", Age = 24}
};
var cars = new List<CardModel>
{
    new() {Brand = "Tesla", Name = "Model X"},
    new() {Brand = "Porsche", Name = "GTX"}
};
var personData = new DataAcces<PersonModel>();
personData.BadEntryFound += PersonData_BadEntryFound;
personData.SaveToCSV(persons, @"C:\Temp\SavedFiles\persons.csv");

var carData = new DataAcces<CardModel>();
carData.BadEntryFound += CarData_BadEntryFound;
carData.SaveToCSV(cars, @"C:\Temp\SavedFiles\cars.csv");

void PersonData_BadEntryFound(object sender, PersonModel e)
{
    Console.WriteLine($"Bad entry for {e.FirstName} {e.LastName}.");
}

void CarData_BadEntryFound(object sender, CardModel e)
{
    Console.WriteLine($"Bad entry for the {e.Brand} {e.Name}.");
}

public class DataAcces<T> where T : new()
{
    public EventHandler<T> BadEntryFound;

    public void SaveToCSV(List<T> items, string filePath)
    {
        var rows = new List<string>();
        var entry = new T();
        var cols = entry.GetType().GetProperties();
        var row = "";
        foreach (var col in cols) row += $",{col.Name}";
        rows.Add(row.Substring(1));

        foreach (var item in items)
        {
            row = "";
            var BadValue = false;

            foreach (var col in cols)
            {
                var val = col.GetValue(item).ToString();
                BadValue = BadWordDetector(val);

                if (BadValue == false)
                {
                    row += $",{val}";
                }
                else
                {
                    BadEntryFound?.Invoke(this, item);
                    break;
                }
            }

            if (BadValue == false) rows.Add(row.Substring(1));
        }

        File.WriteAllLines(filePath, rows);
    }

    private bool BadWordDetector(string test)
    {
        var testToLower = test.ToLower();
        var output = testToLower.Contains("name");
        return output;
    }
}