using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UnSorted-----------------------------------------");
            ObservableCollection<Test> tt = new ObservableCollection<Test>()
            {
                new Test(){  Name="张三",IsReadOnly=false}
                ,new Test(){  Name="张三",IsReadOnly=false}
                ,new Test(){  Name="王五",IsReadOnly=true}
                ,new Test(){  Name="张三",IsReadOnly=false}
                ,new Test(){  Name="哈哈",IsReadOnly=true}
            };
            foreach (var t in tt)
            {
                Console.WriteLine(t.Name);
            }

            List<Test> listChild = tt.ToList();
            listChild.Sort(delegate (Test p1, Test p2) { return Comparer<bool>.Default.Compare(p2.IsReadOnly, p1.IsReadOnly); });
            ObservableCollection<Test> t2 = new ObservableCollection<Test>();


            foreach (var t in listChild)
            {
                t2.Add(t);
            }

            Console.WriteLine("Sorted-----------------------------------------");
            foreach (var t in t2)
            {
                Console.WriteLine(t.Name);
            }
            Console.Read();
            //foreach (var t in tt.Sort<Test,"IsReadOnly">())
            //{
            //}
        }
    }
}
