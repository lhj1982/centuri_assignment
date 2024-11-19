using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/**
 * Test program that convert a structured text file into a predefined JSON format
 */
namespace HelloWorld
{
    // Indicator that which node the current processor is on
    enum RootType
    {
        Person,
        Family
    }
    public class JsonRoot
    {
        [JsonPropertyName("people")]
        public People? People { get; set; }
    }
    public class People
    {
        [JsonPropertyName("person")]
        public List<Person> person { get; set; } = new();
    }
    public class Person
    {
        [JsonPropertyName("fistname")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastname")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
        [JsonPropertyName("phone")]
        public Phone? Phone { get; set; }
        [JsonIgnore]
        public List<Family>? Family { get; set; } = new();
        [JsonPropertyName("family")]
        // do not serialize if list is empty
        public List<Family> SerializationExtensions
        {
            get => Family?.Count > 0 ? Family : null;
            set => Family = value ?? new();
        }
    }

    public class Address
    {
        [JsonPropertyName("street")]
        public string? Street { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("zip")]
        public string? Zip { get; set; }
    }

    public class Phone
    {
        [JsonPropertyName("landline")]
        public string Landline { get; set; } = string.Empty;
        [JsonPropertyName("mobile")]
        public string Mobile { get; set; } = string.Empty;
    }

    public class Family
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("born")]
        public string Born { get; set; } = string.Empty;
        [JsonPropertyName("phone")]
        public Phone? Phone { get; set; }
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
    }
    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            var filename = "input.txt";
            var outputFilename = "output.json";
            var lines = File.ReadAllLines(filename);
            var jsonRoot = new JsonRoot();
            var people = new People();
            var persons = new List<Person>();
            // default to Person
            var currentType = RootType.Person;
            foreach (var line in lines)
            {
                // Console.WriteLine(line);
                var type = line.Split("|")[0];
                if (type == "P")
                {
                    Console.WriteLine("Person");
                    // reset current root type to Person
                    currentType = RootType.Person;
                    // create a new person
                    var person = new Person();

                    person.FirstName = line.Split("|")[1];
                    person.LastName = line.Split("|")[2];
                    persons.Add(person);
                }
                else if (type == "F")
                {
                    Console.WriteLine("Family");
                    currentType = RootType.Family;
                    var family = new Family();
                    family.Name = line.Split("|")[1];
                    family.Born = line.Split("|")[2];
                    persons.Last().Family.Add(family);
                }
                else if (type == "A")
                {
                    Console.WriteLine("Address");
                    var address = new Address();
                    address.Street = line.Split("|")[1];
                    address.City = line.Split("|")[2];
                    try { address.Zip = line.Split("|")[3]; } catch { Console.WriteLine("No Zip"); }
                    persons.Last().Address = address;
                }
                else if (type == "T")
                {
                    Console.WriteLine("Phone");
                    var phone = new Phone();
                    phone.Landline = line.Split("|")[1];
                    phone.Mobile = line.Split("|")[2];
                    persons.Last().Phone = phone;
                }
            }
            // set persons to ppl
            people.person = persons;
            jsonRoot.People = people;
            var json = JsonSerializer.Serialize(jsonRoot, GetOptions());
            // write to a file
            File.WriteAllText(@outputFilename, json);
            // customize serialzier option
            JsonSerializerOptions GetOptions() => new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            Console.WriteLine("Output json: " + json);
        }
    }
}