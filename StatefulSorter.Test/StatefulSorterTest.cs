using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StatefulSorter.Test
{
    public class StatefulSorterTest
    {
        public enum SortTypes { FirstName, LastName, Age };
        public class Person
        {
            public string FirstName { get; private set; }
            public string LastName { get; private set; }
            public int Age { get; private set; }

            public Person(string firstName, string lastName, int age)
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
            }

            public override string ToString()
            {
                return $"{FirstName} {LastName} {Age}";
            }
        }

        private static readonly Person[] TestData = new Person[]
        {
            new Person("aaa", "ccc", 1),
            new Person("aaa", "ccc", 2),
            new Person("aaa", "ccc", 3),
            new Person("aaa", "bbb", 1),
            new Person("aaa", "bbb", 1),
            new Person("aaa", "bbb", 2),
            new Person("aaa", "bbb", 3),
            new Person("bbb", "bbb", 1),
            new Person("bbb", "bbb", 2),
            new Person("bbb", "bbb", 3),
            new Person("ccc", "bbb", 1),
            new Person("or", "and", 2),
        };

        private static readonly StatefulSorter<Person, SortTypes> Target = new StatefulSorter<Person, SortTypes>(new Dictionary<SortTypes, Func<Person, object>>
        {
            { SortTypes.FirstName, d=>d.FirstName },
            { SortTypes.LastName, d=>d.LastName },
            { SortTypes.Age, d=>d.Age }
        });

        [Fact]
        public void SortingWorks()
        {
            var sorted = Target.Sort(TestData, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.LastName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.LastName).ThenByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.LastName).ToArray();
            CompareSorting(TestData.OrderByDescending(d => d.LastName).ThenByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.Age).ToArray();
            CompareSorting(TestData.OrderBy(d => d.Age).ThenByDescending(d => d.LastName).ThenByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.LastName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.LastName).ThenBy(d => d.Age).ThenByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.LastName).ToArray();
            CompareSorting(TestData.OrderByDescending(d => d.LastName).ThenBy(d => d.Age).ThenByDescending(d => d.FirstName), sorted);

            sorted = Target.Sort(sorted, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.FirstName).ThenByDescending(d => d.LastName).ThenBy(d => d.Age), sorted);
        }

        [Fact]
        public void ResetWorks()
        {
            var sorted = Target.Sort(TestData, SortTypes.FirstName).ToArray();
            sorted = Target.Sort(sorted, SortTypes.Age).ToArray();
            sorted = Target.Sort(sorted, SortTypes.LastName).ToArray();

            Target.Reset();
            sorted = Target.Sort(TestData, SortTypes.FirstName).ToArray();
            CompareSorting(TestData.OrderBy(d => d.FirstName), sorted);
        }

        private void CompareSorting<T>(IEnumerable<T> reference, IEnumerable<T> actual)
        {
            Assert.Equal(reference.Count(), actual.Count());
            foreach (var i in reference.Zip(actual, (d, e) => new { First = d, Second = e }))
            {
                Assert.Same(i.First, i.Second);
            }
        }
    }
}