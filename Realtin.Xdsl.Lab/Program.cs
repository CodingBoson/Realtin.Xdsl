using Realtin.Xdsl;

var doc = XdslDocument.CreateFromFile("./Example.xdsl");

Console.WriteLine(doc.WriteToString(writeIndented: true));

var document = new XdslDocument();

document.CreateElement("Person", person => {
    person.CreateElement("FirstName", x => x.Text = "Java");
    person.CreateElement("LastName", x => x.Text = "Script");
    person.CreateElement("Age", x => x.Text = "30");
});

var prettyXdsl = document.WriteToString(writeIndented: true);

Console.WriteLine(prettyXdsl);

Console.ReadLine();