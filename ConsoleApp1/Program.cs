using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            A a = new A();
            A b = new B();
            B c = new C();

            IEnumerable<D> listD = new List<D>() {
                new D() {D_Id=1,D_ParentId=null, D_Description="D1" },
                new D() {D_Id=2,D_ParentId=null, D_Description="D2" },
                new D() {D_Id=3,D_ParentId=null, D_Description="D3" },
                new D() {D_Id=4,D_ParentId=null, D_Description="D4" },
                new D() {D_Id=5,D_ParentId=1, D_Description="D5" },
                new D() {D_Id=6,D_ParentId=1, D_Description="D6" },
                new D() {D_Id=7,D_ParentId=2, D_Description="D7" },
                new D() {D_Id=8,D_ParentId=7, D_Description="D8" },
                new D() {D_Id=9,D_ParentId=2, D_Description="D9" }
            };

            List<D> listD2 = listD.Where(x => x.ParentId == 2).ToList();
            List<D> listD222 = GetChild<D>(2, listD).ToList();
        }

        static IEnumerable<T> GetChild<T>(int id, IEnumerable<T> listD) where T : ITree
        {
            return listD.Where(x => x.Id == id || x.ParentId == id).Union(listD.Where(x => x.ParentId == id).SelectMany(y => GetChild(y.Id, listD)));
        }


}

//public static class ListExt
//{
//    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
//    {
//        var stack = new Stack<T>(items);
//        while (stack.Any())
//        {
//            var next = stack.Pop();
//            yield return next;
//            foreach (var child in childSelector(next))
//                stack.Push(child);
//        }
//    }
//}

interface ITree
{
    int Id { get; }
    int? ParentId { get; }
    string Description { get; }
}

class D : ITree
{
    public int D_Id;
    public int? D_ParentId;
    public string D_Description;

    public int Id { get { return D_Id; } }
    public int? ParentId { get { return D_ParentId; } }
    public string Description { get { return D_Description; } }

    public List<D> Children
    {
        get { return new List<D>(); }

        set { Children = value; }
    }
}

abstract class AA
{
    public abstract void GetStr1();
}

class A
{
    public virtual void GetStr1()
    {


    }
}

class B : A
{
    public override void GetStr1()
    {


    }
}

class C : B
{
    public void GetStr1()
    {


    }
}

}
