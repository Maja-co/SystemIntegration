Person person = new Person("Marie", 26, "Marie@gmail.com");
string json = System.Text.Json.JsonSerializer.Serialize(person);
Console.WriteLine(json);

public class Person {
    public string Name { get; set; }
    public int Age { get; set; } 
    public string Email { get; set; }

    public Person(string name, int age, string email) {
        Name = name;
        Age = age;
        Email = email;
    }
}