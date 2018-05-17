using System;
using System.Threading;
namespace RaceCondition
{
    public class MultiThread
    {
        AutoResetEvent a = new AutoResetEvent(false);
        public MultiThread()
        {
            A a = new A();
            Thread worker1 = new Thread(a.Work1);
            Thread worker2 = new Thread(a.Work2);
            Thread worker3 = new Thread(a.Work3);
            worker1.Start();
            worker2.Start();
            worker3.Start();
            Console.WriteLine(a.result);
            Console.Read();
        }
    }

    class A
    {
        public int result = 0;
        public void Work1() { result = 1; }
        public void Work2() { result = 2; }
        public void Work3() { result = 3; }
    }
}