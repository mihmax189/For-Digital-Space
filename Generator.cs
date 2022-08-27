namespace Generator
{
	using System;
	using Bogus;
	using Models;
	using System.Linq;
	using System.Collections.Generic;

	public class Generator
	{
		public Generator()
		{
			var childId = 0;
			childGenerator = new Faker<Models.Child>()
				.RuleFor(u => u.Id, f => childId++)
				.RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName((Bogus.DataSets.Name.Gender)u.Gender))
				.RuleFor(u => u.LastName, (f, u) => f.Name.LastName((Bogus.DataSets.Name.Gender)u.Gender))
				.RuleFor(u => u.BirthDate, f => Birthday(f, Age.Child))
				.RuleFor(u => u.Gender, f => f.PickRandom<Models.Gender>());

			var personId = 0;
			personGenerator = new Faker<Models.Person>()
				.RuleFor(u => u.Id, f => personId++)
				.RuleFor(u => u.TransportId, f => Guid.NewGuid())
				.RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName((Bogus.DataSets.Name.Gender)u.Gender))
				.RuleFor(u => u.LastName, (f, u) => f.Name.LastName((Bogus.DataSets.Name.Gender)u.Gender))
				.RuleFor(u => u.SequenceId, (f, u) => u.Id)
				.RuleFor(u => u.CreditCardNumbers, f => Enumerable.Range(1, f.Random.Number(1, 5)).Select(_ => f.Finance.CreditCardNumber(Bogus.DataSets.CardType.Visa)).ToArray())
				.RuleFor(u => u.Age, f => f.Random.Int(18, 100))
				.RuleFor(u => u.Phones, f => Enumerable.Range(1, 2).Select(_ => f.Phone.PhoneNumber()).ToArray())
				.RuleFor(u => u.BirthDate, f => Birthday(f, Age.Parent))
				.RuleFor(u => u.Salary, f => f.Random.Double(10000.0, 100000.0))
				.RuleFor(u => u.IsMarred, f => f.Random.Bool())
				.RuleFor(u => u.Gender, f => f.PickRandom<Models.Gender>())
				.RuleFor(u => u.Children, f => Enumerable.Range(1, f.Random.Number(1, 5)).Select(_ => childGenerator.Generate()).ToArray());
		}

		public List<Models.Person> Generate(int size)
		{
			return personGenerator.Generate(size);
		}

		private Int64 Birthday(Faker f, Age age)
		{
			var begin = (age == Age.Parent ? DateTime.Now.AddYears(-100) : DateTime.Now.AddYears(-18));
			var end = (age == Age.Parent ? DateTime.Now.AddYears(-18) : DateTime.Now.AddYears(0));

			DateTime date = f.Date.Between(begin, end);
			DateTimeOffset offset = new DateTimeOffset(date);

			return offset.ToUnixTimeSeconds();
		}

		private enum Age 
		{
			Child,
			Parent
		}
		private Faker<Models.Person> personGenerator;
		private Faker<Models.Child> childGenerator;
	}
}