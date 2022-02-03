using System;

namespace FunctionApp2
{
    public class MyDependency
    {
        public string Message { get; set; }
    }

    public class MyService
    {
        private readonly Action<string> act;
        //private readonly MyDependency dep;

        public MyService(Action<string> act = null)
        {
            this.act = act;
        }

        //public MyService(MyDependency dep)
        //{
        //    this.dep = dep;
        //}

        public void Act(string s)
        {
            if (act != null)
            {
                act(s);
            }

            //if (dep != null)
            //{
            //    Console.WriteLine(dep.Message);
            //}
        }
    }
}